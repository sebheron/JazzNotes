﻿using Avalonia;
using Avalonia.Collections;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using JazzNotes.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace JazzNotes.Models
{
    public class Note : Saveable
    {
        /// <summary>
        /// Creates a new note.
        /// </summary>
        public Note(Transcription transcription, Rect snip, Size size, Thickness margin)
        {
            this.ID = Guid.NewGuid();
            this.Title = string.Empty;
            this.Text = string.Empty;
            this.Transcription = transcription;
            this.Tasks = new AvaloniaList<Task>();
            this.Tags = new AvaloniaList<Tag>();
            this.Images = new AvaloniaList<ImageContainer>();
            this.Snip = snip;
            this.Size = size;
            this.Margin = margin;
            this.Color = ColorGenHelper.GenerateRandomBrush();
        }

        /// <summary>
        /// Creates a note.
        /// </summary>
        public Note(Guid id, Transcription transcription, Rect snip, Size size, Thickness margin, string title, string text, SolidColorBrush color)
        {
            this.ID = id;
            this.Title = title;
            this.Text = text;
            this.Transcription = transcription;
            this.Tasks = new AvaloniaList<Task>();
            this.Tags = new AvaloniaList<Tag>();
            this.Images = new AvaloniaList<ImageContainer>();
            this.Snip = snip;
            this.Size = size;
            this.Margin = margin;
            this.Color = color;
        }

        /// <summary>
        /// The id for the note.
        /// </summary>
        public Guid ID { get; }

        /// <summary>
        /// Gets and sets the title of the note.
        /// </summary>
        public string Title { get; set; }

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
        /// The size of the rect for the note.
        /// </summary>
        public Size Size { get; }

        /// <summary>
        /// The margin for the snip.
        /// </summary>
        public Thickness Margin { get; }

        /// <summary>
        /// The list of tags.
        /// </summary>
        public AvaloniaList<Tag> Tags { get; protected set; }

        /// <summary>
        /// The list of tasks.
        /// </summary>
        public AvaloniaList<Task> Tasks { get; protected set; }

        /// <summary>
        /// The list of bitmaps.
        /// </summary>
        public AvaloniaList<ImageContainer> Images { get; protected set; }

        /// <summary>
        /// The color for the note.
        /// </summary>
        public SolidColorBrush Color { get; }

        /// <summary>
        /// Add a new task to the note.
        /// </summary>
        /// <param name="name">Name of task.</param>
        public void AddTask(string name)
        {
            var task = new Task(name);
            this.Tasks.Add(task);
            FileHelper.SaveLinker();
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
            FileHelper.SaveLinker();
        }

        /// <summary>
        /// Removes a task from the note.
        /// </summary>
        /// <param name="task">The task to be removed.</param>
        public void RemoveTask(Task task)
        {
            this.Tasks.Remove(task);
            FileHelper.SaveLinker();
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
                FileHelper.SaveLinker();
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
            FileHelper.SaveLinker();
        }

        /// <summary>
        /// Add an image.
        /// </summary>
        /// <param name="path">Path to the image.</param>
        public void AddImage(string path)
        {
            var image = new ImageContainer(path);
            this.Images.Add(image);
            FileHelper.SaveLinker();
        }

        /// <summary>
        /// Removes an image.
        /// </summary>
        /// <param name="bitmap">The image to remove.</param>
        public void RemoveImage(ImageContainer image)
        {
            if (File.Exists(image.FilePath) && !Transcription.Linker.IsImageInUse(image))
            {
                File.Delete(image.FilePath);
            }
            this.Images.Remove(image);
            FileHelper.SaveLinker();
        }
    }
}