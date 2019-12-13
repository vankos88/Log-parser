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
        public LogParserModel Model { get; set; }

        public CombinedReactiveCommand<Unit, Unit> Search { get; }
        public CombinedReactiveCommand<Unit, Unit> FindFiles { get; }
        public ReactiveCommand<Unit, Unit> Cancel { get; }
        

        public LogParserViewModel(LogParserManager manager)
        {
            _manager = manager;
            Model = new LogParserModel();

            var cleanDisplay = ReactiveCommand.Create(() =>
            {
                Model.ResultDisplay = new List<string> { string.Empty };
                Model.ElapsedTime = "Elapsed time: -/-";
            });

            var search = ReactiveCommand.CreateFromTask(() => _manager.Search(Model));;
            var findFiles = ReactiveCommand.CreateFromTask(() => _manager.FindFiles(Model)); 

            Search = ReactiveCommand.CreateCombined(new[] { search, cleanDisplay });
            FindFiles = ReactiveCommand.CreateCombined(new[] { findFiles, cleanDisplay });
            Cancel = ReactiveCommand.Create(() => _manager.Cancel());
        }
    }
}
