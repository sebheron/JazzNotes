using Avalonia;
using Avalonia.Collections;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using JazzNotes.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;

namespace JazzNotes.Models
{
    public class Note
    {
        /// <summary>
        /// Gets and sets the content of the note.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// The transcription the note is for.
        /// </summary>
        public Transcription Transcription { get; }

        /// <summary>
        /// The size of the snip for the note.
        /// </summary>
        public Rect Snip { get; }

        /// <summary>
        /// The margin for the snip.
        /// </summary>
        public Thickness Margin { get; }

        /// <summary>
        /// The list of tags.
        /// </summary>
        public ObservableCollection<Tag> Tags { get; protected set; }

        /// <summary>
        /// The color for the note.
        /// </summary>
        public SolidColorBrush Color { get; }

        /// <summary>
        /// Creates a new note.
        /// </summary>
        public Note(Transcription transcription, Rect snip, Thickness margin)
        {
            this.Text = string.Empty;
            this.Transcription = transcription;
            this.Tags = new ObservableCollection<Tag>();
            this.Snip = snip;
            this.Margin = margin;
            this.Color = ColorGenHelper.GenerateRandomBrush();
        }

        /// <summary>
        /// Creates a note.
        /// </summary>
        public Note(Transcription transcription, Rect snip, Thickness margin, string text, SolidColorBrush color)
        {
            this.Text = text;
            this.Transcription = transcription;
            this.Tags = new ObservableCollection<Tag>();
            this.Snip = snip;
            this.Margin = margin;
            this.Color = color;
        }

        /// <summary>
        /// Add a new tag to the note.
        /// </summary>
        /// <param name="name">Name of tag.</param>
        /// <returns>Whether the tag was added or not.</returns>
        public bool AddTag(string name)
        {
            var tag = new Tag(name);
            var contains = this.Tags.Any(x => x.Name == name);
            if (!contains)
            {
                this.Tags.Add(this.Transcription.Linker.GetOrAddTag(name));
            }
            return !contains;
        }

        /// <summary>
        /// Removes a tag from the note.
        /// </summary>
        /// <param name="tag">The tag to be removed.</param>
        public void RemoveTag(Tag tag)
        {
            this.Tags.Remove(tag);
            this.Transcription.Linker.RemoveTagIfNotInuse(tag);
        }
    }
}