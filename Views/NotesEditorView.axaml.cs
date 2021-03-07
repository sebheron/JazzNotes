using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using JazzNotes.ViewModels;

namespace JazzNotes.Views
{
    public class NotesEditorView : UserControl
    {
        private AutoCompleteBox tagsTextBox;
        private TextBox tasksTextBox;

        public NotesEditorView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            this.tagsTextBox = this.FindControl<AutoCompleteBox>("TagsTextBox");
            this.tagsTextBox.KeyUp += TagKeyUp;
            this.tasksTextBox = this.FindControl<TextBox>("TasksTextBox");
            this.tasksTextBox.KeyUp += TaskKeyUp;
        }

        private void TagKeyUp(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var added = ((NotesEditorViewModel)this.DataContext).AddTag(this.tagsTextBox.Text.Replace(" ", "").ToLower());
                if (added)
                {
                    this.tagsTextBox.Text = string.Empty;
                }
            }
            else if (e.Key == Key.Space)
            {
                this.tagsTextBox.Text = this.tagsTextBox.Text.Replace(" ", "-");
            }
        }

        private void TaskKeyUp(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ((NotesEditorViewModel)this.DataContext).AddTask(this.tasksTextBox.Text);
                this.tasksTextBox.Text = string.Empty;
            }
        }
    }
}