using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using JazzNotes.Helpers;
using JazzNotes.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JazzNotes.ViewModels
{
    public class TranscriptionViewModel : ViewModelBase
    {
        private MainWindowViewModel mainViewModel;
        private Bitmap image;
        private AvaloniaList<NotesEditorViewModel> noteVMs;
        private Transcription transcription;
        private PdfHelper pdfHelper;
        private bool showNotes;

        /// <summary>
        /// Create new transcription viewmodel.
        /// </summary>
        /// <param name="mainViewModel">Main viewmodel.</param>
        /// <param name="transcription">The transcription.</param>
        /// <param name="pdfHelper">PDFhelper.</param>
        public TranscriptionViewModel(MainWindowViewModel mainViewModel, Transcription transcription, PdfHelper pdfHelper)
        {
            this.mainViewModel = mainViewModel;
            this.noteVMs = new AvaloniaList<NotesEditorViewModel>();
            this.transcription = transcription;
            this.pdfHelper = pdfHelper;
            this.ShowNotes = true;
            foreach (var note in transcription.Notes)
            {
                var bmp = this.pdfHelper.GetSnip(note.Snip);
                this.noteVMs.Add(new NotesEditorViewModel(bmp, note));
            }
        }

        /// <summary>
        /// The current image for the transcription.
        /// </summary>
        public Bitmap Image
        {
            get => this.image;
            set => this.RaiseAndSetIfChanged(ref this.image, value);
        }

        /// <summary>
        /// The screen width.
        /// </summary>
        public double GridWidth => WindowHelper.MainWindow.Screens.Primary.WorkingArea.Width;

        /// <summary>
        /// The notes viewmodels.
        /// </summary>
        public AvaloniaList<NotesEditorViewModel> NoteVMs
        {
            get => this.noteVMs;
            set => this.RaiseAndSetIfChanged(ref this.noteVMs, value);
        }

        /// <summary>
        /// Whether notes should be shown or not.
        /// </summary>
        public bool ShowNotes
        {
            get => this.showNotes;
            set => this.RaiseAndSetIfChanged(ref this.showNotes, value);
        }

        /// <summary>
        /// Add a new note.
        /// </summary>
        /// <param name="bounds">Size for the note.</param>
        /// <param name="visual">Visual bounds for note.</param>
        /// <param name="actual">Actual bounds for note.</param>
        public void AddNote(Rect bounds, Rect visual, Rect actual)
        {
            var widthD = pdfHelper.Width / bounds.Width;
            var heightD = pdfHelper.Height / bounds.Height;

            var snip = new Rect(actual.Left * widthD, actual.Top * heightD, actual.Width * widthD, actual.Height * heightD);
            var bmp = this.pdfHelper.GetSnip(snip);

            var note = new Note(this.transcription, snip, new Thickness(visual.Left, visual.Top, 0, 0));

            this.transcription.AddNote(note);

            var noteVm = new NotesEditorViewModel(bmp, note);

            this.NoteVMs.Add(noteVm);

            this.mainViewModel.Content = noteVm;
        }

        /// <summary>
        /// The note to delete.
        /// </summary>
        /// <param name="sender">The note</param>
        public void DeleteNote(NotesEditorViewModel vm)
        {
            this.NoteVMs.Remove(vm);
        }

        /// <summary>
        /// Open edit for note.
        /// </summary>
        /// <param name="vm">The note view model to open.</param>
        public void EditNote(NotesEditorViewModel vm)
        {
            this.mainViewModel.Content = vm;
        }

        /// <summary>
        /// Get noteditor viewmodel for note.
        /// </summary>
        /// <param name="note">The note to find.</param>
        /// <returns>The note viewmodel.</returns>
        public NotesEditorViewModel GetNoteViewModel(Note note)
        {
            foreach (var notevm in this.noteVMs)
            {
                if (notevm.Text == note.Text
                    && notevm.TranscriptionName == note.Transcription.Name
                    && notevm.Width == note.Snip.Width
                    && notevm.Height == note.Snip.Height
                    && notevm.Margin == note.Margin
                    && notevm.Color == note.Color)
                {
                    return notevm;
                }
            }
            return null;
        }
    }
}
