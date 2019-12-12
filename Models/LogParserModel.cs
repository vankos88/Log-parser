using ReactiveUI;
using System.Collections.Generic;

namespace LogParser.Models
{
    public class LogParserModel : ReactiveObject
    {
        private string elapsedTime;
        private List<string> resultDisplay;


        public LogParserModel()
        {
            IncludeFileInfo = true;
            elapsedTime = "Elapsed time: -/-";
            IncludeSubdirectories = true;
        }

        public string Paths { get; set; }
        public string Masks { get; set; }
        public string SearchLine { get; set; }
        public bool IncludeFileInfo { get; set; }
        public bool IncludeSubdirectories { get; set; }

        public List<string> ResultDisplay 
        {
            get => resultDisplay;
            set => this.RaiseAndSetIfChanged(ref resultDisplay, value);
        }


        public string ElapsedTime
        {
            get => elapsedTime;
            set => this.RaiseAndSetIfChanged(ref elapsedTime, value);
        }
    }
}
