using Avalonia.Collections;
using JazzNotes.Helpers;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace JazzNotes.Models
{
    public class Transcription
    {
        private string name;

        /// <summary>
        /// Creates a new transcription.
        /// </summary>
        public Transcription(Linker linker, string filePath, string name = null, AvaloniaList<Note> notes = null)
        {
            this.FilePath = filePath;
            this.Name = name ?? Path.GetFileNameWithoutExtension(filePath);
            this.Linker = linker;
            this.Notes = notes ?? new AvaloniaList<Note>();

            this.Notes.CollectionChanged += (s, e) => FileHelper.SaveLinker();
        }

        /// <summary>
        /// Gets the filepath for the transcription.
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// The linker for the application.
        /// </summary>
        public Linker Linker { get; }

        /// <summary>
        /// Name of the transcription.
        /// </summary>
        public string Name
        {
            get => this.name;
            set
            {
                this.name = value;
                FileHelper.SaveLinker();
            }
        }

        /// <summary>
        /// The notes for the transcription.
        /// </summary>
        public AvaloniaList<Note> Notes { get; }

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
        /// Add a new note.
        /// </summary>
        public void AddNote(Note note)
        {
            this.Notes.Add(note);
        }
    }
}