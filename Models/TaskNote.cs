using JazzNotes.Helpers;
using System.Linq;

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
        }

        /// <summary>
        /// Whether the task note is checked or not.
        /// </summary>
        public bool Checked => this.Note.Tasks.All(x => x.Checked);

        /// <summary>
        /// The note.
        /// </summary>
        public Note Note { get; }
    }
}