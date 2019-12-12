using LogParser.Managers;
using LogParser.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive;

namespace LogParser.ViewModels
{
    public class LogParserViewModel : ViewModelBase
    {
        public LogParserManager _manager { get; }
        public LogParserViewModel(LogParserManager manager)
        {
            _manager = manager;
            Model = new LogParserModel();
            Search = ReactiveCommand.CreateFromTask(() => _manager.Search(Model));;
        }

        public LogParserModel Model { get; set; }
        public ReactiveCommand<Unit, Unit> Search { get; }
        public void FindFiles()
        {
            CleanDisplay();

            try
            {
                _manager.FindFiles(Model);
            }

            catch (Exception ex)
            {
                Model.ResultDisplay = new List<string> { ex.ToString() };
            }
        }

        public void CleanDisplay()
        {
            Model.ResultDisplay = new List<string> { string.Empty };
            Model.ElapsedTime = "Elapsed time: -/-";
        }

        public void Cancel()
        {
            _manager.Cancel();
        }
    }
}
