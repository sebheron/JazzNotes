using JazzNotes.Helpers;
using System.ComponentModel;

namespace JazzNotes.Models
{
    public class Saveable
    {
        /// <summary>
        /// Property changed
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Method for properties being changed.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">EventArgs</param>
        protected void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            FileHelper.SaveLinker();
        }
    }
}