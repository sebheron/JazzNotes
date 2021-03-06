﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JazzNotes.Models
{
    public class Task : Saveable
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
        /// Name of the task.
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Whether the task is checked or not.
        /// </summary>
        public bool Checked { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Display => $"• {this.Name} " + (this.Checked ? "✔️" : string.Empty);
    }
}