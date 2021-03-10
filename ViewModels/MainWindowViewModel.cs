using Avalonia;
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
                this.LoadedFiles = await this.PdfHelper.ShowNewDialog();
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
            this.LoadSelectedInternalFile(note.Transcription.FilePath);
            this.OpenLoadedFile();
            this.Content = this.TranscriptionVM.GetNoteViewModel(note);
            this.TranscriptionVM = null;
        }

        /// <summary>
        /// Open a transcription.
        /// </summary>
        /// <param name="transcription">The transcription to open.</param>
        public void OpenTranscription(Transcription transcription)
        {
            this.LoadSelectedInternalFile(transcription.FilePath);
            this.OpenLoadedFile();
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
                    var tasks = this.Linker.Tasks.Where(x => x.Note.ID == note.ID).ToList();
                    this.Linker.Tasks.RemoveAll(tasks);
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
        public ViewModelBase GoBackToTranscription()
        {
            if (this.TranscriptionVM != null)
            {
                this.Content = this.TranscriptionVM;
            }
            else
            {
                this.Content = this.StartupVM;
            }

            return this.Content;
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
        /// Loads the selected file.
        /// </summary>
        /// <param name="path">Selected file.</param>
        public void LoadSelectedInternalFile(string path)
        {
            if (String.IsNullOrEmpty(path)) return;

            this.PdfHelper.Clean();
            this.PdfHelper.LoadImage(path);
        }

        /// <summary>
        /// Loads the selected file.
        /// </summary>
        /// <param name="path">Selected file.</param>
        public void LoadSelectedFile(string path)
        {
            if (String.IsNullOrEmpty(path)) return;

            this.PdfHelper.Clean();
            this.PdfHelper.LoadPDF(path);
        }

        /// <summary>
        /// Appends the selected file.
        /// </summary>
        /// <param name="path">Selected file.</param>
        public void AppendSelectedFile(string path)
        {
            if (String.IsNullOrEmpty(path)) return;
            this.PdfHelper.AppendPDF(path);
        }

        /// <summary>
        /// Opens the loaded file.
        /// </summary>
        public void OpenLoadedFile()
        {
            if (this.PdfHelper.Image == null) return;

            var transcription = this.Linker.GetOrAddTranscription(this.PdfHelper.FilePath);

            this.TranscriptionVM = new TranscriptionViewModel(this, transcription, this.PdfHelper.Image);

            if (this.Content is NotesEditorViewModel vm)
            {
                this.TranscriptionVM.ScrollPosition = new Vector(0, vm.Margin.Top);
            }

            this.Content = this.TranscriptionVM;
        }

        /// <summary>
        /// Sets the loaded files.
        /// </summary>
        public string[] LoadedFiles
        {
            set
            {
                this.LoadSelectedFile(value[0]);
                if (value.Length > 1)
                {
                    for (int i = 1; i < value.Length; i++){
                        this.AppendSelectedFile(value[i]);
                    }
                }
                this.OpenLoadedFile();
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