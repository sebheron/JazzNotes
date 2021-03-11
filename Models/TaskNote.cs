using JazzNotes.Helpers;

namespace JazzNotes.Models
{
    public class TaskNote
    {
        private bool check;

        /// <summary>
        /// Creates a task note.
        /// </summary>
        public TaskNote(Note note)
        {
            this.Note = note;
            this.Checked = false;
        }

        /// <summary>
        /// Creates a task note.
        /// </summary>
        public TaskNote(Note note, bool check)
        {
            this.Note = note;
            this.Checked = check;
        }

        /// <summary>
        /// Whether the task note is checked or not.
        /// </summary>
        public bool Checked
        {
            get => this.check;
            set
            {
                this.check = value;
                FileHelper.SaveLinker();
            }
        }

        /// <summary>
        /// The note.
        /// </summary>
        public Note Note { get; }
    }
}