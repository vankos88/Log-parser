using System;
using System.Collections.Generic;
using System.Text;

namespace LogParser.Models
{
    public struct LineInfo
    {
        public string Path { get; set; }
        public string FileName { get; set; }
        public string Line { get; set; }
    }
}
