using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JazzNotes.Models
{
    public class TaskNote
    {
        /// <summary>
        /// Creates a task note.
        /// </summary>
        /// <param name="note">The note.</param>
        public TaskNote(Note note)
        {
            this.Note = note;
            this.Checked = false;
        }

        /// <summary>
        /// The note.
        /// </summary>
        public Note Note { get; }

        /// <summary>
        /// Whether the task note is checked or not.
        /// </summary>
        public bool Checked { get; set; }
    }
}
