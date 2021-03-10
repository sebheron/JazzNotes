using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using JazzNotes.Helpers;
using JazzNotes.ViewModels;

namespace JazzNotes.Views
{
    public class NotesEditorView : UserControl
    {
        public NotesEditorView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void OpenImageInViewer(object? sender, RoutedEventArgs e)
        {
            if (sender is Image image)
            {
                var bmp = image.Source as Bitmap;
                var currentVm = (NotesEditorViewModel)this.DataContext;
                var mainVm = (MainWindowViewModel)WindowHelper.MainWindow.DataContext;
                var imageViewer = new PhotoViewerViewModel(bmp, mainVm, currentVm);

                mainVm.Content = imageViewer;
            }
        }

        private void TagKeyUp(object? sender, KeyEventArgs e)
        {
            if (sender is AutoCompleteBox tagsTextBox)
            {
                if (e.Key == Key.Enter)
                {
                    var added = ((NotesEditorViewModel)this.DataContext).AddTag(tagsTextBox.Text.Replace(" ", "").ToLower());
                    if (added)
                    {
                        tagsTextBox.Text = string.Empty;
                    }
                }
                else if (e.Key == Key.Space)
                {
                    tagsTextBox.Text = tagsTextBox.Text.Replace(" ", "-");
                }
            }
        }

        private void TaskKeyUp(object? sender, KeyEventArgs e)
        {
            if (sender is TextBox tasksTextBox)
            {
                if (e.Key == Key.Enter)
                {
                    ((NotesEditorViewModel)this.DataContext).AddTask(tasksTextBox.Text);
                    tasksTextBox.Text = string.Empty;
                }
            }
        }
    }
}