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
            var files = GetFiles(model.Masks, model.Paths);

            var sb = new StringBuilder();
            sb.AppendLine($"Total files found:{files.Length}");
            for (int i = 0; i < files.Length; i++)
            {
                sb.AppendLine($"{i + 1}. {files[i]}");
            }

            sb.AppendLine("---------END---------");

            model.ResultDisplay = sb.ToString();
        }

        private string[] GetFiles(string maskText, string pathsText)
        {
            var paths = pathsText.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Distinct().ToArray();

            var masks = maskText.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Distinct().ToArray();
            var list = new List<string>();

            foreach (var path in paths)
            {
                if (masks.Length > 0)
                {
                    foreach (var mask in masks)
                    {
                        var files = Directory.GetFiles(path, mask);
                        list.AddRange(files);
                    }
                }
                else
                {
                    var files = Directory.GetFiles(path);
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

        public async void Search(LogParserModel model)
        {
            Stopwatch elapsedTime = new Stopwatch();
            elapsedTime.Start();

            string searchString = model.SearchLine.ToLower();

            if (string.IsNullOrWhiteSpace(searchString))
            {
                model.ResultDisplay = "Empty search string";
                return;
            }

            var maskText = model.Masks;
            var pathsText = model.Paths;
            var includeFileInfo = model.IncludeFileInfo;

            var result = await Task.Run(() => ProcessFiles(searchString, pathsText, maskText, includeFileInfo));

            model.ResultDisplay = result;

            elapsedTime.Stop();
            model.ElapsedTime = $"Elapsed time: {elapsedTime.Elapsed.ToString()}";
        }

        private string ProcessFiles(string searchString, string pathsText, string maskText, bool includeFileInfo)
        {
            var result = string.Empty;
            var files = GetFiles(maskText, pathsText);

            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            try
            {
                var sb = new StringBuilder();

                var bag = new ConcurrentBag<LineInfo>();

                Parallel.ForEach(files, new ParallelOptions { CancellationToken = token }, file =>
                {
                    ProcessFile(file, searchString, bag, includeFileInfo);
                });

                bag
                    .OrderBy(x => x.Path)
                    .ThenBy(y => y.FileName)
                    .ThenBy(z => z.Line)
                    .Take(bag.Count > 100000 ? 100000 : bag.Count) // show only the first 100k entries.
                    .Select(x => sb.AppendLine(x.Line))
                    .ToList();

                result = $"Found {bag.Count} times\n\n" + sb;

            }

            catch (OperationCanceledException)
            {
                result = $"Search has been canceled";
            }

            catch (Exception ex)
            {
                result = $"ERROR: {ex}";
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
            using (var file = File.OpenRead(filePath))
            {
                var fileExtension = Path.GetExtension(filePath);

                var isSupportedArchive =
                    fileExtension.Equals(".rar") || fileExtension.Equals(".zip") || fileExtension.Equals(".tar")
                    || fileExtension.Equals(".gz");

                if (isSupportedArchive)
                {
                    using (var reader = ReaderFactory.Open(file))
                    {
                        reader.MoveToNextEntry();
                        using (var entryStream = reader.OpenEntryStream())
                        {
                            ProcessStream(entryStream, bag, filePath, searchString, includeFileInfo);
                        }
                    }
                }

                else
                    ProcessStream(file, bag, filePath, searchString, includeFileInfo);
            }
        }

        private void ProcessStream(Stream stream, ConcurrentBag<LineInfo> bag, string filePath, string searchString, bool includeFileInfo)
        {
            using (BufferedStream bs = new BufferedStream(stream))
            {
                using (StreamReader sr = new StreamReader(bs))
                {
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
    }
}
