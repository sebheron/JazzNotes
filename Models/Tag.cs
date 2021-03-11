using Avalonia.Media;
using JazzNotes.Helpers;

namespace JazzNotes.Models
{
    /// <summary>
    /// The tag class representing different tags used on notes taken.
    /// </summary>
    public class Tag
    {
        /// <summary>
        /// Creates a new tag.
        /// </summary>
        /// <param name="name">The name of the new tag.</param>
        public Tag(string name)
        {
            this.Name = name;
            this.Color = ColorGenHelper.GenerateRandomBrush();
        }

        /// <summary>
        /// Creates a tag.
        /// </summary>
        /// <param name="name">The name of the tag.</param>
        /// <param name="color">The color of the tag.</param>
        public Tag(string name, ISolidColorBrush color)
        {
            this.Name = name;
            this.Color = color;
        }

        /// <summary>
        /// Gets the color of the tag, generated at creation.
        /// </summary>
        public ISolidColorBrush Color { get; }

        /// <summary>
        /// Name of the tag.
        /// </summary>
        public string Name { get; protected set; }
    }
}