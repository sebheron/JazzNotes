using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using JazzNotes.Models;

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