using ReactiveUI;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        public string ResultDisplaySelectedItem { get; set; }

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

        public void CleanDisplay()
        {
            ResultDisplay = new List<string> { string.Empty };
            ElapsedTime = "Elapsed time: -/-";
        }

        public bool Validate()
        {
            var result = new List<string>();

            if (string.IsNullOrEmpty(Paths))
            {
                result.Add("Empty paths");
            }

            if (string.IsNullOrWhiteSpace(SearchLine))
            {
                result.Add("Empty search string");
            }

            ResultDisplay = result;

            return result.Count == 0;
        }
    }
}
