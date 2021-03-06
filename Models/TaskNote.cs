using JazzNotes.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// The note.
        /// </summary>
        public Note Note { get; }

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
    }
}
