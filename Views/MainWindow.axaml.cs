using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using JazzNotes.Helpers;
using JazzNotes.ViewModels;
using System;
using System.ComponentModel;

namespace JazzNotes.Views
{
    public class MainWindow : Window
    {
        private MainWindowViewModel viewModel;

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void OnMainWindowOpened(object sender, EventArgs e)
        {
            this.viewModel = this.DataContext as MainWindowViewModel;
        }

        private void OnMainWindowClosing(object sender, CancelEventArgs e)
        {
            if (this.viewModel != null && this.viewModel.Content is TranscriptionViewModel)
            {
                e.Cancel = true;
                this.viewModel.GoBackToStartup();
            }
            else if (this.viewModel != null && this.viewModel.Content is NotesEditorViewModel)
            {
                e.Cancel = true;
                this.viewModel.GoBackToTranscription();
            }
            else if (this.viewModel != null)
            {
                FileHelper.SaveLinker(this.viewModel.Linker);
            }
        }
    }
}
