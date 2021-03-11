using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace JazzNotes.Views
{
    public class PhotoViewerView : UserControl
    {
        public PhotoViewerView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}