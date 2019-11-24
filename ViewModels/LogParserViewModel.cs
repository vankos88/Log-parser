using LogParser.Managers;
using LogParser.Models;

namespace LogParser.ViewModels
{
    public class LogParserViewModel : ViewModelBase
    {
        public LogParserManager _manager { get; }
        public LogParserViewModel(LogParserManager manager)
        {
            _manager = manager;
            Model = new LogParserModel();
        } 

        public LogParserModel Model { get; set; }

        public void FindFiles()
        {
        }
    }
}
