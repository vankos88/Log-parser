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
            CleanDisplay();

            _manager.FindFiles(Model);
        }

        public void CleanDisplay()
        {
            Model.ResultDisplay = string.Empty;
            Model.ElapsedTime = "Elapsed time: -/-";
        }

        public void Cancel()
        {
            _manager.Cancel();
        }

        public void Search()
        {
            CleanDisplay();
            FreezeUI();

            _manager.Search(Model);

            UnfreezeUI();
        }

        public void FreezeUI()
        {

        }

        public void UnfreezeUI()
        {

        }
    }
}
