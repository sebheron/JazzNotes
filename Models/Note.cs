﻿using Avalonia.Collections;
using Avalonia.Media;
using JazzNotes.Helpers;
using System;
using System.IO;
using System.Linq;

namespace JazzNotes.Models
{
    public class Note
    {
        private string title, text;

        /// <summary>
        /// Creates a new note.
        /// </summary>
        public Note(Transcription transcription, AvaloniaList<Snip> snips)
        {
            this.ID = Guid.NewGuid();
            this.Title = string.Empty;
            this.Text = string.Empty;
            this.Transcription = transcription;
            this.Tasks = new AvaloniaList<Task>();
            this.Tags = new AvaloniaList<Tag>();
            this.Images = new AvaloniaList<ImageContainer>();
            this.Snips = new AvaloniaList<Snip>();
            foreach (var snip in snips)
            {
                this.Snips.Add(snip);
            }

            this.Color = ColorGenHelper.GenerateRandomBrush();

            this.Tags.CollectionChanged += (s, e) => FileHelper.SaveLinker();
            this.Tasks.CollectionChanged += (s, e) => FileHelper.SaveLinker();
            this.Images.CollectionChanged += (s, e) => FileHelper.SaveLinker();
        }

        /// <summary>
        /// Creates a note.
        /// </summary>
        public Note(Guid id, Transcription transcription, string title, string text, SolidColorBrush color, AvaloniaList<Snip> snips)
        {
            this.ID = id;
            this.Title = title;
            this.Text = text;
            this.Transcription = transcription;
            this.Tasks = new AvaloniaList<Task>();
            this.Tags = new AvaloniaList<Tag>();
            this.Images = new AvaloniaList<ImageContainer>();
            this.Snips = new AvaloniaList<Snip>();
            foreach (var snip in snips)
            {
                this.Snips.Add(snip);
            }
            this.Color = color;

            this.Tags.CollectionChanged += (s, e) => FileHelper.SaveLinker();
            this.Tasks.CollectionChanged += (s, e) => FileHelper.SaveLinker();
            this.Images.CollectionChanged += (s, e) => FileHelper.SaveLinker();
        }

        /// <summary>
        /// The color for the note.
        /// </summary>
        public SolidColorBrush Color { get; }

        /// <summary>
        /// The id for the note.
        /// </summary>
        public Guid ID { get; }

        /// <summary>
        /// The list of bitmaps.
        /// </summary>
        public AvaloniaList<ImageContainer> Images { get; protected set; }

        /// <summary>
        /// The list of snips for the note.
        /// </summary>
        public AvaloniaList<Snip> Snips { get; }

        /// <summary>
        /// The list of tags.
        /// </summary>
        public AvaloniaList<Tag> Tags { get; protected set; }

        /// <summary>
        /// The list of tasks.
        /// </summary>
        public AvaloniaList<Task> Tasks { get; protected set; }

        /// <summary>
        /// Gets and sets the content of the note.
        /// </summary>
        public string Text
        {
            get => this.text;
            set
            {
                this.text = value;
                FileHelper.SaveLinker();
            }
        }

        /// <summary>
        /// Gets and sets the title of the note.
        /// </summary>
        public string Title
        {
            get => this.title;
            set
            {
                this.title = value;
                FileHelper.SaveLinker();
            }
        }

        /// <summary>
        /// The transcription the note is for.
        /// </summary>
        public Transcription Transcription { get; }

        /// <summary>
        /// Add an image.
        /// </summary>
        /// <param name="path">Path to the image.</param>
        public void AddImage(string path, string name)
        {
            var image = new ImageContainer(path, name);
            this.Images.Add(image);
        }

        /// <summary>
        /// Add a snip.
        /// </summary>
        /// <param name="snip">The snip.</param>
        public void AddSnip(Snip snip)
        {
            this.Snips.Add(snip);
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
        /// Add a new task to the note.
        /// </summary>
        /// <param name="name">Name of task.</param>
        public void AddTask(string name)
        {
            var task = new Task(name);
            this.Tasks.Add(task);
        }

        /// <summary>
        /// Add a new task to the note.
        /// </summary>
        /// <param name="name">Name of task.</param>
        /// <param name="check">Name of check.</param>
        public void AddTask(string name, bool check)
        {
            var task = new Task(name, check);
            this.Tasks.Add(task);
        }

        /// <summary>
        /// Removes an image.
        /// </summary>
        public void RemoveImage(ImageContainer image)
        {
            if (File.Exists(image.FilePath) && !Transcription.Linker.IsImageInUse(image))
            {
                File.Delete(image.FilePath);
            }
            this.Images.Remove(image);
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

        /// <summary>
        /// Removes a task from the note.
        /// </summary>
        /// <param name="task">The task to be removed.</param>
        public void RemoveTask(Task task)
        {
            this.Tasks.Remove(task);
        }
    }
}