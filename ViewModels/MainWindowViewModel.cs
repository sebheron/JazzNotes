using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using JazzNotes.Helpers;
using JazzNotes.Models;
using ReactiveUI;
using System;
using System.Diagnostics;
using System.Linq;

namespace JazzNotes.ViewModels
{
    public class MainWindowViewModel : ReactiveObject
    {
        private readonly PdfHelper pdfHelper;

        private ViewModelBase content;

        /// <summary>
        /// Create new main viewmodel.
        /// </summary>
        public MainWindowViewModel()
        {
            this.pdfHelper = new PdfHelper();

            this.Linker = FileHelper.LoadLinker();

            this.Content = this.StartupVM = new StartupViewModel(this.Linker);
        }

        /// <summary>
        /// Create new transcription from file.
        /// </summary>
        public async void NewTranscription()
        {
            try
            {
                this.LoadedFile = await this.pdfHelper.ShowDialog();
            }
            catch
            {
                Debug.WriteLine("Error occured showing/closing OpenFileDialog.");
            }
        }

        /// <summary>
        /// Open a note.
        /// </summary>
        /// <param name="note">The note to open.</param>
        public void OpenNote(Note note)
        {
            this.LoadedFile = note.Transcription.FilePath;
            this.Content = this.TranscriptionVM.GetNoteViewModel(note);
            this.TranscriptionVM = null;
        }

        /// <summary>
        /// Open a transcription.
        /// </summary>
        /// <param name="transcription">The transcription to open.</param>
        public void OpenTranscription(Transcription transcription)
        {
            this.LoadedFile = transcription.FilePath;
        }

        /// <summary>
        /// The current displayed viewmodel.
        /// </summary>
        public ViewModelBase Content
        {
            get => this.content;
            set => this.RaiseAndSetIfChanged(ref this.content, value);
        }

        /// <summary>
        /// The loaded file.
        /// </summary>
        public string LoadedFile
        {
            set
            {
                if (String.IsNullOrEmpty(value)) return;

                var transcription = this.Linker.GetOrAddTranscription(value);
                this.pdfHelper.Clean();
                this.pdfHelper.LoadPDF(value);

                if (this.pdfHelper.Image != null)
                {
                    this.Content = this.TranscriptionVM = new TranscriptionViewModel(this, transcription, this.pdfHelper);
                    this.TranscriptionVM.Image = this.pdfHelper.Image;
                }
            }
        }

        /// <summary>
        /// The startup viewmodel.
        /// </summary>
        public StartupViewModel StartupVM { get; }

        /// <summary>
        /// The current transcription viewmodel.
        /// </summary>
        public TranscriptionViewModel TranscriptionVM { get; set; }

        /// <summary>
        /// The linker for the entire application.
        /// </summary>
        public Linker Linker { get; }
    }
}
