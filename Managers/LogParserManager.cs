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

        public void FindFiles(LogParserModel model)
        {
            var files = GetFiles(model);

            var result = files.ToList();
            result.Insert(0, $"Total files found:{files.Length}");
            result.Add("---------END---------");

            model.ResultDisplay = result;
        }

        private string[] GetFiles(LogParserModel model)
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

            return list.Distinct().ToArray();
        }

        public void Cancel()
        {
            if (_cts != null)
                _cts.Cancel();
        }

        public async Task Search(LogParserModel model)
        {
            Stopwatch elapsedTime = new Stopwatch();
            elapsedTime.Start();

            if (string.IsNullOrWhiteSpace(model.SearchLine))
            {
                model.ResultDisplay = new List<string> { "Empty search string" };
                elapsedTime.Stop();
                return;
            }

            string searchString = model.SearchLine.ToLower();

            var result = await Task.Run(() => ProcessFiles(searchString, model));

            model.ResultDisplay = result;

            elapsedTime.Stop();
            model.ElapsedTime = $"Elapsed time: {elapsedTime.Elapsed.ToString()}";
        }

        private List<string> ProcessFiles(string searchString, LogParserModel model)
        {
            var includeFileInfo = model.IncludeFileInfo;

            var result = new List<string>();
            var files = GetFiles(model);

            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            try
            {
                var bag = new ConcurrentBag<LineInfo>();

                Parallel.ForEach(files, new ParallelOptions { CancellationToken = token }, file =>
                {
                    ProcessFile(file, searchString, bag, includeFileInfo);
                });

                result = bag
                      .OrderBy(x => x.Path)
                      .ThenBy(y => y.FileName)
                      .ThenBy(z => z.Line)
                      .Select(g => g.Line)
                      .ToList();

                result.Insert(0, $"Found {bag.Count} times\n\n");


                return result;

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

                reader.MoveToNextEntry();
                using var entryStream = reader.OpenEntryStream();

                ProcessStream(entryStream, bag, filePath, searchString, includeFileInfo);
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
                var path = Path.GetDirectoryName(filePath);
                var fileName = Path.GetFileName(filePath);

                if (line.ToLower().Contains(searchString))
                {
                    var str = string.Empty;
                    if (includeFileInfo)
                    {
                        str += $"File: {filePath}\n";
                        str += $"Line: {counter}\n";
                    }

                    str += line + "\n";
                    bag.Add(new LineInfo { Path = path, FileName = fileName, Line = str });
                }
            }
        }
    }
}
