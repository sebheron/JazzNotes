using JazzNotes.Helpers;

namespace JazzNotes.Models
{
    public class Task
    {
        private bool check;

        /// <summary>
        /// Create a new task.
        /// </summary>
        public Task(string name)
        {
            this.Name = name;
            this.Checked = false;
        }

        /// <summary>
        /// Create a new task.
        /// </summary>
        public Task(string name, bool check)
        {
            this.Name = name;
            this.Checked = check;
        }

        /// <summary>
        /// Whether the task is checked or not.
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
        /// The display string.
        /// </summary>
        public string Display => $"• {this.Name} " + (this.Checked ? "☑️" : string.Empty);

        /// <summary>
        /// Name of the task.
        /// </summary>
        public string Name { get; protected set; }
    }
}