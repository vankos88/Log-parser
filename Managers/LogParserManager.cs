using LogParser.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LogParser.Managers
{
    public class LogParserManager
    {
        public string FindFiles(LogParserModel model)
        {
            var files = GetFiles(model);

            var sb = new StringBuilder();
            sb.AppendLine($"Total files found:{files.Length}");
            for (int i = 0; i < files.Length; i++)
            {
                sb.AppendLine($"{i + 1}. {files[i]}");
            }

            sb.AppendLine("---------END---------");

            return sb.ToString();
        }

        private string[] GetFiles(LogParserModel model)
        {
            var paths = model.Paths.Split('\n').Where(x => !String.IsNullOrEmpty(x)).ToArray();

            var masks = model.Masks.Split('\n');
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
    }
}
