
namespace LogParser.Models
{
    public struct LineInfo
    {
        public string FilePath { get; set; }
        public long RowNumber { get; set; }
        public string Line { get; set; }
    }
}
