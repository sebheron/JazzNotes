using Avalonia.Collections;
using JazzNotes.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JazzNotes.Models
{
    public class Linker : Saveable
    {
        /// <summary>
        /// Create new linker
        /// </summary>
        public Linker()
        {
            this.AllTags = new List<Tag>();
            this.Transcriptions = new List<Transcription>();
            this.Tasks = new AvaloniaList<TaskNote>();
        }

        /// <summary>
        /// Tasks for each note.
        /// </summary>
        public AvaloniaList<TaskNote> Tasks { get; }

        /// <summary>
        /// All of the transcriptions available.
        /// </summary>
        public IList<Transcription> Transcriptions { get; }

        /// <summary>
        /// Gets all tags for all notes and transcriptions.
        /// </summary>
        public readonly IList<Tag> AllTags;

        /// <summary>
        /// Gets a tag by name.
        /// </summary>
        /// <returns></returns>
        public Tag GetTag(string name)
        {
            return this.AllTags.FirstOrDefault(x => x.Name == name);
        }

        /// <summary>
        /// Gets or adds a new tag.
        /// </summary>
        /// <param name="name">Name of tag.</param>
        /// <returns>The tag recieved or added.</returns>
        public Tag GetOrAddTag(string name)
        {
            var getTag = this.AllTags.FirstOrDefault(x => x.Name == name);
            if (getTag == null)
            {
                getTag = new Tag(name);
                this.AllTags.Add(getTag);
                FileHelper.SaveLinker();
            }
            return getTag;
        }

        /// <summary>
        /// Remove a tag if its no longer being used by any notes.
        /// </summary>
        /// <param name="tag">The tag to remove.</param>
        public void RemoveTagIfNotInuse(Tag tag)
        {
            var allTagsComplete = this.Transcriptions.SelectMany(x => x.Notes).SelectMany(x => x.Tags);
            if (!allTagsComplete.Contains(tag))
            {
                this.AllTags.Remove(tag);
                FileHelper.SaveLinker();
            }
        }

        /// <summary>
        /// Gets or adds a transcription.
        /// </summary>
        /// <param name="filePath">Filepath for transcription.</param>
        /// <returns>The transcription recieved or added.</returns>
        public Transcription GetOrAddTranscription(string filePath)
        {
            var getTranscription = this.Transcriptions.FirstOrDefault(x => x.FilePath == filePath);
            if (getTranscription == null)
            {
                getTranscription = new Transcription(this, filePath);
                this.Transcriptions.Add(getTranscription);
                FileHelper.SaveLinker();
            }
            return getTranscription;
        }

        /// <summary>
        /// Gets a list of used tags and omits unused ones.
        /// </summary>
        /// <returns></returns>
        public IList<Tag> GetUsedTags()
        {
            return this.Transcriptions.SelectMany(x => x.Tags)
                    .GroupBy(x => x.Name)
                    .Select(x => x.First())
                    .ToList();
        }

        /// <summary>
        /// Gets whether an image is in use.
        /// </summary>
        /// <returns>If the image is in use.</returns>
        public bool IsImageInUse(ImageContainer imageContainer)
        {
            return this.Transcriptions.SelectMany(x => x.Notes).SelectMany(x => x.Images).Any(x => x.FilePath == imageContainer.FilePath);
        }
    }
}