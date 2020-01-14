using LogParser.Models;
using SharpCompress.Readers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LogParser.Managers
{
    public class LogParserManager
    {
        CancellationTokenSource _cts;

        public async Task FindFiles(LogParserModel model)
        {
            if (!string.IsNullOrEmpty(model.Paths))
            {
                try
                {
                    var result = await Task.Run(() => GetFiles(model));

                    result.Insert(0, $"Total files found:{result.Count}");
                    result.Add("---------END---------");
                    model.ResultDisplay = result;
                }

                catch (Exception ex)
                {
                    model.ResultDisplay = new List<string> { ex.ToString() };
                }
            }

            else
                model.ResultDisplay = new List<string> { "Empty paths" };

        }

        private List<string> GetFiles(LogParserModel model)
        {
            var paths = model.Paths.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Distinct().ToArray();

            var masks = model.Masks?.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Distinct().ToArray();

            var list = new List<string>();

            foreach (var path in paths)
            {
                var searchOption = model.IncludeSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

                if (masks != null && masks.Length > 0)
                {
                    foreach (var mask in masks)
                    {
                        var files = Directory.GetFiles(path, mask, searchOption);
                        list.AddRange(files);
                    }
                }

                else
                {
                    var files = Directory.GetFiles(path, "*", searchOption);
                    list.AddRange(files);
                }
            }

            return list.Distinct().ToList();
        }

        public void Cancel()
        {
            if (_cts != null)
                _cts.Cancel();
        }

        public async Task Search(LogParserModel model)
        {
            if (model.Validate())
            {
                Stopwatch elapsedTime = new Stopwatch();
                elapsedTime.Start();

                var result = await Task.Run(() => ProcessFiles(model));

                model.ResultDisplay = result;

                elapsedTime.Stop();
                model.ElapsedTime = $"Elapsed time: {elapsedTime.Elapsed.ToString()}";
            }
        }

        private List<string> ProcessFiles(LogParserModel model)
        {
            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            var includeFileInfo = model.IncludeFileInfo;

            var result = new List<string>();

            try
            {
                var files = GetFiles(model);

                string searchString = model.SearchLine.ToLower();

                var bag = new ConcurrentBag<LineInfo>();

                Parallel.ForEach(files, new ParallelOptions { CancellationToken = token }, file =>
                {
                    ProcessFile(file, searchString, bag, includeFileInfo);
                });

                result = bag
                      .OrderBy(x => x.FilePath)
                      .ThenBy(y => y.RowNumber)
                      .Select(g => g.Line)
                      .ToList();

                result.Insert(0, $"Found {bag.Count} times\n\n");
            }

            catch (OperationCanceledException)
            {
                result = new List<string> { $"Search has been canceled" };
            }

            catch (Exception ex)
            {
                result = new List<string> { $"ERROR: {ex}" };
            }

            finally
            {
                if (_cts != null)
                {
                    _cts.Dispose();
                    _cts = null;
                }
            }

            return result;
        }

        private void ProcessFile(string filePath, string searchString, ConcurrentBag<LineInfo> bag, bool includeFileInfo)
        {
            using var file = File.OpenRead(filePath);

            var fileExtension = Path.GetExtension(filePath);

            var isSupportedArchive =
                fileExtension.Equals(".rar") || fileExtension.Equals(".zip") || fileExtension.Equals(".tar")
                || fileExtension.Equals(".gz");

            if (isSupportedArchive)
            {
                using var reader = ReaderFactory.Open(file);

                while(reader.MoveToNextEntry())
                {
                    using var entryStream = reader.OpenEntryStream();
                    var filePathInsideArchive = $"{filePath}\\{reader.Entry}";

                    ProcessStream(entryStream, bag, filePathInsideArchive, searchString, includeFileInfo);
                }
            }

            else
                ProcessStream(file, bag, filePath, searchString, includeFileInfo);
        }

        private void ProcessStream(Stream stream, ConcurrentBag<LineInfo> bag, string filePath, string searchString, bool includeFileInfo)
        {
            using var bs = new BufferedStream(stream);
            using var sr = new StreamReader(bs);

            string line;
            int counter = 0;
            while ((line = sr.ReadLine()) != null)
            {
                counter++;

                if (line.ToLower().Contains(searchString))
                {
                    var str = string.Empty;
                    if (includeFileInfo)
                    {
                        str += $"File: {filePath}\n";
                        str += $"Line: {counter}\n";
                    }

                    str += line + "\n";
                    bag.Add(new LineInfo { FilePath = filePath, RowNumber = counter, Line = str });
                }
            }
        }
    }
}
