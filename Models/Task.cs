using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JazzNotes.Models
{
    public class Task : Named
    {
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
        public bool Checked { get; set; }
    }
}
