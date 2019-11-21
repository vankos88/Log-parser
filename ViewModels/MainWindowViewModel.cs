using System;
using System.Collections.Generic;
using System.Text;

namespace LogParser.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            LogParser = new LogParserViewModel();
        }
        public LogParserViewModel LogParser { get; }
    }
}
