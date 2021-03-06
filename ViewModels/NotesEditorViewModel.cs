using Avalonia;
using Avalonia.Collections;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using JazzNotes.Helpers;
using JazzNotes.Models;
using ReactiveUI;

namespace JazzNotes.ViewModels
{
    public class NotesEditorViewModel : ViewModelBase
    {
        private readonly Note note;
        private readonly PdfHelper pdfHelper;

        /// <summary>
        /// Create a new note editor view model
        /// </summary>
        /// <param name="image">The image for the note.</param>
        /// <param name="note">The note model.</param>
        public NotesEditorViewModel(Bitmap image, Note note, PdfHelper pdfHelper)
        {
            this.note = note;
            this.Shown = true;
            this.Image = image;
            this.pdfHelper = pdfHelper;
        }

        /// <summary>
        /// The width of the notes image.
        /// </summary>
        public double Width => this.note.Size.Width;

        /// <summary>
        /// The height of the notes image.
        /// </summary>
        public double Height => this.note.Size.Height;

        /// <summary>
        /// The margin for the note (used when positioning the note on the transcription page).
        /// </summary>
        public Thickness Margin => this.note.Margin;

        /// <summary>
        /// The transcription name.
        /// </summary>
        public string TranscriptionName => this.note.Transcription.Name;

        /// <summary>
        /// The transcription for the note.
        /// </summary>
        public Transcription Transcription => this.note.Transcription;

        /// <summary>
        /// Tags for the note.
        /// </summary>
        public AvaloniaList<Tag> Tags => this.note.Tags;

        /// <summary>
        /// Tags for the note.
        /// </summary>
        public AvaloniaList<Task> Tasks => this.note.Tasks;

        /// <summary>
        /// Images for the note.
        /// </summary>
        public AvaloniaList<ImageContainer> Images => this.note.Images;

        /// <summary>
        /// Color for the note.
        /// </summary>
        public SolidColorBrush Color => this.note.Color;

        /// <summary>
        /// The image for the note.
        /// </summary>
        public Bitmap Image { get; set; }

        /// <summary>
        /// Whether the note is to be shown or not.
        /// </summary>
        public bool Shown { get; set; }

        /// <summary>
        /// The title for the note.
        /// </summary>
        public string Title
        {
            get => this.note.Title;
            set
            {
                this.note.Title = value;
                this.RaisePropertyChanged(nameof(this.Title));
            }
        }

        /// <summary>
        /// The text for the note.
        /// </summary>
        public string Text
        {
            get => this.note.Text;
            set
            {
                this.note.Text = value;
                this.RaisePropertyChanged(nameof(this.Text));
            }
        }

        /// <summary>
        /// Add a new task to the note.
        /// </summary>
        /// <param name="name">Name of task.</param>
        public void AddTask(string name)
        {
            this.note.AddTask(name);
        }

        /// <summary>
        /// Remove a task.
        /// </summary>
        public void RemoveTask(Task task)
        {
            this.note.RemoveTask(task);
        }

        /// <summary>
        /// Add a new tag to the note.
        /// </summary>
        /// <param name="name">Name of tag.</param>
        /// <returns>Whether the tag was added or not.</returns>
        public bool AddTag(string name)
        {
            var tag = this.note.AddTag(name);
            return tag;
        }

        /// <summary>
        /// Remove a tag.
        /// </summary>
        public void RemoveTag(Tag tag)
        {
            this.note.RemoveTag(tag);
        }

        /// <summary>
        /// Deletes this note.
        /// </summary>
        public async void DeleteNote()
        {
            var mvm = (MainWindowViewModel)WindowHelper.MainWindow.DataContext;
            var delete = await mvm.DeleteNote(note);
            if (delete && mvm.GoBackToTranscription() is TranscriptionViewModel tvm)
            {
                tvm.DeleteNote(this);
            }
        }

        /// <summary>
        /// Adds an image.
        /// </summary>
        public async void AddImage()
        {
            var path = await pdfHelper.ShowAddImageDialog();
            if (string.IsNullOrEmpty(path)) return;
            var newPath = pdfHelper.LoadExternalImage(path);
            this.note.AddImage(newPath);
        }

        /// <summary>
        /// Deletes an image.
        /// </summary>
        public void RemoveImage(ImageContainer image)
        {
            this.note.RemoveImage(image);
        }
    }
}