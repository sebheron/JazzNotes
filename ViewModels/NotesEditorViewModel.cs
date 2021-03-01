using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using JazzNotes.Helpers;
using JazzNotes.Models;
using ReactiveUI;
using System.Collections.ObjectModel;

namespace JazzNotes.ViewModels
{
    public class NotesEditorViewModel : ViewModelBase
    {
        /// <summary>
        /// The note.
        /// </summary>
        private Note note;

        /// <summary>
        /// Create a new note editor view model
        /// </summary>
        /// <param name="image">The image for the note.</param>
        /// <param name="note">The note model.</param>
        public NotesEditorViewModel(Bitmap image, Note note)
        {
            this.note = note;
            this.Shown = true;
            this.Image = image;
        }

        /// <summary>
        /// The width of the notes image.
        /// </summary>
        public double Width => this.note.Snip.Width;

        /// <summary>
        /// The height of the notes image.
        /// </summary>
        public double Height => this.note.Snip.Height;

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
        public ObservableCollection<Tag> Tags => this.note.Tags;

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
        /// Add a new tag to the note.
        /// </summary>
        /// <param name="name">Name of tag.</param>
        /// <returns>Whether the tag was added or not.</returns>
        public bool AddTag(string name)
        {
            return this.note.AddTag(name);
        }

        /// <summary>
        /// Remove a tag.
        /// </summary>
        /// <returns></returns>
        public void RemoveTag(Tag tag)
        {
            this.note.RemoveTag(tag);
        }

        /// <summary>
        /// Deletes this note.
        /// </summary>
        public void DeleteNote()
        {
            var mvm = (MainWindowViewModel)WindowHelper.MainWindow.DataContext;
            mvm.TranscriptionVM.DeleteNote(this);
            mvm.DeleteNote(note);
        }
    }
}
