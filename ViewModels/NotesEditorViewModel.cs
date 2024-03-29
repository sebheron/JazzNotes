﻿using Avalonia;
using Avalonia.Collections;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using JazzNotes.Helpers;
using JazzNotes.Models;
using ReactiveUI;
using System;
using System.IO;
using System.Linq;

namespace JazzNotes.ViewModels
{
    public class NotesEditorViewModel : ViewModelBase
    {
        private readonly Note note;
        private readonly PdfHelper pdfHelper;

        /// <summary>
        /// Create a new note editor view model
        /// </summary>
        /// <param name="image">The image for the note.</param>
        /// <param name="note">The note model.</param>
        public NotesEditorViewModel(Bitmap image, Note note, PdfHelper pdfHelper)
        {
            this.note = note;
            this.Shown = true;
            this.Image = image;
            this.pdfHelper = pdfHelper;
        }

        /// <summary>
        /// Gets all the tags for autocomplete.
        /// </summary>
        public AvaloniaList<string> AutoCompleteItems => new AvaloniaList<string>(this.note.Transcription.Linker.AllTags
            .Where(x => !this.Tags.Contains(x)).Select(x => x.Name));

        /// <summary>
        /// Color for the note.
        /// </summary>
        public SolidColorBrush Color => this.note.Color;

        /// <summary>
        /// The display title.
        /// </summary>
        public string DisplayTitle => this.note.Snips.Count > 1 ? "..." : string.Empty;

        /// <summary>
        /// The height.
        /// </summary>
        public double Height => this.note.Snips[0].Size.Height;

        /// <summary>
        /// The id.
        /// </summary>
        public Guid ID => this.note.ID;

        /// <summary>
        /// The image for the note.
        /// </summary>
        public Bitmap Image { get; set; }

        /// <summary>
        /// Images for the note.
        /// </summary>
        public AvaloniaList<ImageContainer> Images => this.note.Images;

        /// <summary>
        /// The margin.
        /// </summary>
        public Thickness Margin => this.note.Snips[0].Margin;

        /// <summary>
        /// Whether the note is to be shown or not.
        /// </summary>
        public bool Shown { get; set; }

        /// <summary>
        /// Tags for the note.
        /// </summary>
        public AvaloniaList<Tag> Tags => this.note.Tags;

        /// <summary>
        /// Tags for the note.
        /// </summary>
        public AvaloniaList<Task> Tasks => this.note.Tasks;

        /// <summary>
        /// The text for the note.
        /// </summary>
        public string Text
        {
            get => this.note.Text;
            set
            {
                this.note.Text = value;
                this.RaisePropertyChanged(nameof(this.Text));
            }
        }

        /// <summary>
        /// The title for the note.
        /// </summary>
        public string Title
        {
            get => this.note.Title;
            set
            {
                this.note.Title = value;
                this.RaisePropertyChanged(nameof(this.Title));
            }
        }

        /// <summary>
        /// The transcription for the note.
        /// </summary>
        public Transcription Transcription => this.note.Transcription;

        /// <summary>
        /// The transcription name.
        /// </summary>
        public string TranscriptionName => this.note.Transcription.Name;

        /// <summary>
        /// The width.
        /// </summary>
        public double Width => this.note.Snips[0].Size.Width;

        /// <summary>
        /// Adds an image.
        /// </summary>
        public async void AddImage()
        {
            var path = await pdfHelper.ShowAddImageDialog();
            if (string.IsNullOrEmpty(path)) return;
            var newPath = pdfHelper.LoadExternalImage(path);
            this.note.AddImage(newPath, Path.GetFileNameWithoutExtension(path));
        }

        /// <summary>
        /// Add a new tag to the note.
        /// </summary>
        /// <param name="name">Name of tag.</param>
        /// <returns>Whether the tag was added or not.</returns>
        public bool AddTag(string name)
        {
            var tag = this.note.AddTag(name);
            this.RaisePropertyChanged(nameof(this.AutoCompleteItems));
            return tag;
        }

        /// <summary>
        /// Add a new task to the note.
        /// </summary>
        /// <param name="name">Name of task.</param>
        public void AddTask(string name)
        {
            this.note.AddTask(name);
        }

        /// <summary>
        /// Deletes this note.
        /// </summary>
        public async void DeleteNote()
        {
            var mvm = (MainWindowViewModel)WindowHelper.MainWindow.DataContext;
            var delete = await mvm.DeleteNote(note);
            if (delete && mvm.GoBackToTranscription() is TranscriptionViewModel tvm)
            {
                tvm.DeleteNote(this);
            }
        }

        /// <summary>
        /// Deletes an image.
        /// </summary>
        public void RemoveImage(ImageContainer image)
        {
            this.note.RemoveImage(image);
        }

        /// <summary>
        /// Remove a tag.
        /// </summary>
        public void RemoveTag(Tag tag)
        {
            this.note.RemoveTag(tag);
            this.RaisePropertyChanged(nameof(this.AutoCompleteItems));
        }

        /// <summary>
        /// Remove a task.
        /// </summary>
        public void RemoveTask(Task task)
        {
            this.note.RemoveTask(task);
        }
    }
}