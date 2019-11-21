using System;
using System.Collections.Generic;
using System.Text;

namespace LogParser.ViewModels
{
    public class LogParserViewModel : ViewModelBase
    {
        public LogParserViewModel()
        {
            IncludeFileInfo = true;
        }
        public bool IncludeFileInfo { get; set; }

    }
}
