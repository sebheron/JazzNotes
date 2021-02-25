using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace JazzNotes.Views
{
    public class StartupView : UserControl
    {
        public StartupView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
