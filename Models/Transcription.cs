using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace JazzNotes.Models
{
    public class Transcription : Named
    {
        /// <summary>
        /// Gets the filepath for the transcription.
        /// </summary>
        public readonly string FilePath;

        /// <summary>
        /// The notes for the transcription.
        /// </summary>
        public readonly IList<Note> Notes;

        /// <summary>
        /// The linker for the application.
        /// </summary>
        public readonly Linker Linker;

        /// <summary>
        /// Get the tags in the notes.
        /// </summary>
        public List<Tag> Tags
        {
            get
            {
                return this.Notes.SelectMany(x => x.Tags)
                    .GroupBy(x => x.Name)
                    .Select(x => x.First())
                    .ToList();
            }
        }

        /// <summary>
        /// Creates a new transcription.
        /// </summary>
        public Transcription(Linker linker, string filePath, IList<Note> notes = null)
        {
            this.FilePath = filePath;
            this.Name = Path.GetFileNameWithoutExtension(filePath);
            this.Linker = linker;
            this.Notes = notes ?? new List<Note>();
        }

        /// <summary>
        /// Add a new note.
        /// </summary>
        public void AddNote(Note note)
        {
            this.Notes.Add(note);
        }
    }
}