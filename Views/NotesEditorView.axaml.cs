using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using JazzNotes.Models;
using JazzNotes.ViewModels;

namespace JazzNotes.Views
{
    public class NotesEditorView : UserControl
    {
        private TextBox tagsTextBox;

        public NotesEditorView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            this.tagsTextBox = this.FindControl<TextBox>("TagsTextBox");
            this.tagsTextBox.KeyUp += TagKeyUp;
        }

        private void TagKeyUp(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space || e.Key == Key.Enter)
            {
                var added = ((NotesEditorViewModel)this.DataContext).AddTag(this.tagsTextBox.Text.Replace(" ", "").ToLower());
                if (added)
                {
                    this.tagsTextBox.Text = string.Empty;
                }
                else if (e.Key == Key.Space)
                {
                    this.tagsTextBox.Text = this.tagsTextBox.Text.Replace(" ", "-");
                }
            }
        }
    }
}
