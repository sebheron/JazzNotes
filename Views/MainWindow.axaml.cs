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
            FileHelper.SaveLinker(this.viewModel.Linker);
        }
    }
}