using LogParser.Managers;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogParser.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel(LogParserManager manager)
        {
            LogParser = new LogParserViewModel(manager);
        }
        public LogParserViewModel LogParser { get; }
    }
}
