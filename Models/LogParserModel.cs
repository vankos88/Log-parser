using ReactiveUI;

namespace LogParser.Models
{
    public class LogParserModel : ReactiveObject
    {
        public string Paths { get; set; }
        public string Masks { get; set; }
        public string SearchLine { get; set; }
        public bool IncludeFileInfo { get; set; }

        private string resultDisplay;
        public string ResultDisplay 
        {
            get => resultDisplay;
            set => this.RaiseAndSetIfChanged(ref resultDisplay, value);
        }

        private string elapsedTime;
        public string ElapsedTime
        {
            get => elapsedTime;
            set => this.RaiseAndSetIfChanged(ref elapsedTime, value);
        }

        public LogParserModel()
        {
            IncludeFileInfo = true;
            elapsedTime = "Elapsed time: -/-";
        }
    }
}
