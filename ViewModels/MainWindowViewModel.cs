using JazzNotes.Helpers;
using JazzNotes.Models;
using MessageBox.Avalonia;
using MessageBox.Avalonia.Enums;
using ReactiveUI;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace JazzNotes.ViewModels
{
    public class MainWindowViewModel : ReactiveObject
    {
        private ViewModelBase content;

        /// <summary>
        /// Create new main viewmodel.
        /// </summary>
        public MainWindowViewModel()
        {
            this.PdfHelper = new PdfHelper();
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
                this.LoadedFile = await this.PdfHelper.ShowNewDialog();
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
        /// Deletes a note.
        /// </summary>
        /// <param name="note">The note to delete.</param>
        public async Task<bool> DeleteNote(Note note)
        {
            var name = note.Title.Length > 20 ? note.Title.Substring(0, 20) + "..." : note.Title;
            var messageBoxStandardWindow = MessageBoxManager
                    .GetMessageBoxStandardWindow("JazzNotes", $"Are you sure you want to delete the note: {name}?", ButtonEnum.YesNo);
            var delete = await messageBoxStandardWindow.ShowDialog(WindowHelper.MainWindow);

            if (delete == ButtonResult.Yes)
            {
                this.Linker.Tasks.RemoveAll(this.Linker.Tasks.Where(x => x.Note.ID == note.ID));

                var transcription = note.Transcription;
                transcription.Notes.Remove(note);
                this.StartupVM.RaiseListChanged();

                return true;
            }
            return false;
        }

        /// <summary>
        /// Deletes a transcription.
        /// </summary>
        /// <param name="transcription">The transcription to delete.</param>
        public async void DeleteTranscription(Transcription transcription)
        {
            var name = transcription.Name.Length > 20 ? transcription.Name.Substring(0, 20) + "..." : transcription.Name;
            var messageBoxStandardWindow = MessageBoxManager
                    .GetMessageBoxStandardWindow("JazzNotes", $"Are you sure you want to delete the transcription: {name}?", ButtonEnum.YesNo);
            var delete = await messageBoxStandardWindow.ShowDialog(WindowHelper.MainWindow);

            if (delete == ButtonResult.Yes)
            {
                var path = transcription.FilePath;

                foreach (var note in transcription.Notes)
                {
                    this.Linker.Tasks.RemoveAll(this.Linker.Tasks.Where(x => x.Note.ID == note.ID));
                }

                this.Linker.Transcriptions.Remove(transcription);
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                this.StartupVM.RaiseListChanged();
            }
        }

        /// <summary>
        /// Adds a note to the tasks list.
        /// </summary>
        /// <param name="note">The note to add.</param>
        public void AddNoteToTasks(Note note)
        {
            var taskNote = new TaskNote(note);
            this.Linker.Tasks.Add(taskNote);
            this.StartupVM.SelectedIndex = 2;
        }

        /// <summary>
        /// Deletes a task.
        /// </summary>
        /// <param name="task">The task to delete.</param>
        public async void DeleteTask(TaskNote task)
        {
            var name = task.Note.Title.Length > 20 ? task.Note.Title.Substring(0, 20) + "..." : task.Note.Title;
            var messageBoxStandardWindow = MessageBoxManager
                    .GetMessageBoxStandardWindow("JazzNotes", $"Are you sure you want to remove the task for note: {name}?", ButtonEnum.YesNo);
            var delete = await messageBoxStandardWindow.ShowDialog(WindowHelper.MainWindow);

            if (delete == ButtonResult.Yes)
            {
                this.Linker.Tasks.Remove(task);
                this.StartupVM.RaiseListChanged();
            }
        }

        /// <summary>
        /// Go back to the transcription or if no transcription is open, go to startup.
        /// </summary>
        public void GoBackToTranscription()
        {
            if (this.TranscriptionVM != null)
            {
                this.Content = this.TranscriptionVM;
            }
            else
            {
                this.Content = this.StartupVM;
            }
        }

        /// <summary>
        /// Go back to the startup.
        /// </summary>
        public void GoBackToStartup()
        {
            this.Content = this.StartupVM;
            this.TranscriptionVM = null;
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

                this.PdfHelper.Clean();
                this.PdfHelper.LoadPDF(value);

                if (this.PdfHelper.Image == null) return;

                var transcription = this.Linker.GetOrAddTranscription(this.PdfHelper.FilePath);

                this.Content = this.TranscriptionVM = new TranscriptionViewModel(this, transcription, this.PdfHelper.Image);
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

        /// <summary>
        /// The pdf helper for the entire application.
        /// </summary>
        public PdfHelper PdfHelper { get; }
    }
}