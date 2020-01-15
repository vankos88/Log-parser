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

        public ReactiveCommand<Unit, Unit> Search { get; }
        public ReactiveCommand<Unit, Unit> FindFiles { get; }
        public ReactiveCommand<Unit, Unit> Cancel { get; }
        public ReactiveCommand<Unit, Unit> Copy { get; }



        public LogParserViewModel(LogParserManager manager)
        {
            _manager = manager;
            Model = new LogParserModel();

            var cleanDisplay = ReactiveCommand.Create(() =>
            {
                Model.ResultDisplay = new List<string> { string.Empty };
                Model.ElapsedTime = "Elapsed time: -/-";
            });

            Search = ReactiveCommand.CreateFromTask(async  () => { Model.CleanDisplay(); await _manager.Search(Model); }); 
            FindFiles = ReactiveCommand.CreateFromTask(async () => {  Model.CleanDisplay(); await _manager.FindFiles(Model); });
            Cancel = ReactiveCommand.Create(() => _manager.Cancel());
            Copy = ReactiveCommand.CreateFromTask(() => _manager.CopyToClipboard(Model.ResultDisplaySelectedItem));
        }
    }
}
