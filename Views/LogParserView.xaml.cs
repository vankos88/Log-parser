using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace LogParser.Views
{
    public class LogParserView : UserControl
    {
        public LogParserView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
