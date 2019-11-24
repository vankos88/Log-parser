using System;
using System.Collections.Generic;
using System.Text;

namespace LogParser.Models
{
    public class LogParserModel
    {
        public string Paths { get; set; }
        public string Mask { get; set; }
        public string SearchLine { get; set; }
        public bool IncludeFileInfo { get; set; }
        public string ResultDisplay { get; set; }

        public LogParserModel()
        {
            IncludeFileInfo = true;
        }
    }
}
