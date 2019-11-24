using ReactiveUI;

namespace LogParser.Models
{
    public class LogParserModel : ReactiveObject
    {
        public string Paths { get; set; }
        public string Mask { get; set; }
        public string SearchLine { get; set; }
        public bool IncludeFileInfo { get; set; }

        private string resultDisplay;
        public string ResultDisplay 
        {
            get => resultDisplay;
            set => this.RaiseAndSetIfChanged(ref resultDisplay, value);
        }

        public LogParserModel()
        {
            IncludeFileInfo = true;
        }
    }
}
