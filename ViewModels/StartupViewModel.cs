using Avalonia.Collections;
using JazzNotes.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JazzNotes.ViewModels
{
    public class StartupViewModel : ViewModelBase
    {
        private int selectedIndex;
        private string search;
        private readonly Linker linker;

        /// <summary>
        /// The inital startup viewmodel.
        /// </summary>
        /// <param name="items">The list of transcriptions.</param>
        public StartupViewModel(Linker linker)
        {
            this.linker = linker;
        }

        /// <summary>
        /// The list of notes to display.
        /// </summary>
        public AvaloniaList<Note> NoteItems
        {
            get
            {
                var keywords = this.GetKeyWords();
                if (keywords != null)
                {
                    var tags = this.GetApplicableTags(keywords);
                    int count = tags.Count();

                    var allNotes = this.linker.Transcriptions.SelectMany(x => x.Notes);

                    var notes = new AvaloniaList<Note>();

                    foreach (var note in allNotes)
                    {
                        var noteCount = note.Tags.Select(x => x.Name).Intersect(tags.Select(x => x.Name)).Count();
                        if (noteCount >= count)
                        {
                            notes.Add(note);
                        }
                    }

                    return notes;
                }
                return new AvaloniaList<Note>(this.linker.Transcriptions.SelectMany(x => x.Notes));
            }
        }

        /// <summary>
        /// The list of transcriptions to display.
        /// </summary>
        public AvaloniaList<Transcription> TranscriptionItems
        {
            get
            {
                var keywords = this.GetKeyWords();
                if (keywords != null)
                {
                    var tags = this.GetApplicableTags(keywords);
                    int count = tags.Count();

                    var transcriptions = new AvaloniaList<Transcription>();

                    foreach (var transcription in this.linker.Transcriptions)
                    {
                        var transcriptionCount = transcription.Tags.Select(x => x.Name).Intersect(tags.Select(x => x.Name)).Count();
                        if (transcriptionCount >= count)
                        {
                            transcriptions.Add(transcription);
                        }
                    }

                    return transcriptions;
                }
                return new AvaloniaList<Transcription>(this.linker.Transcriptions);
            }
        }

        /// <summary>
        /// The list of tasks to display.
        /// </summary>
        public AvaloniaList<TaskNote> TaskItems
        {
            get
            {
                var keywords = this.GetKeyWords();
                if (keywords != null)
                {
                    var tags = this.GetApplicableTags(keywords);
                    int count = tags.Count();

                    var allTasks = this.linker.Tasks;

                    var tasks = new AvaloniaList<TaskNote>();

                    foreach (var task in allTasks)
                    {
                        var noteCount = task.Note.Tags.Select(x => x.Name).Intersect(tags.Select(x => x.Name)).Count();
                        if (noteCount >= count)
                        {
                            tasks.Add(task);
                        }
                    }

                    return tasks;
                }
                return this.linker.Tasks;
            }
        }

        /// <summary>
        /// The search query.
        /// </summary>
        public string Search
        {
            get => this.search;
            set
            {
                this.RaiseAndSetIfChanged(ref this.search, value);
                this.RaiseListChanged();
            }
        }

        /// <summary>
        /// The selected tab index.
        /// </summary>
        public int SelectedIndex
        {
            get => this.selectedIndex;
            set => this.RaiseAndSetIfChanged(ref this.selectedIndex, value);
        }

        /// <summary>
        /// Raises that the lists have been changed.
        /// </summary>
        public void RaiseListChanged()
        {
            this.RaisePropertyChanged(nameof(this.NoteItems));
            this.RaisePropertyChanged(nameof(this.NotesCount));
            this.RaisePropertyChanged(nameof(this.TranscriptionItems));
            this.RaisePropertyChanged(nameof(this.TranscriptionsCount));
        }

        /// <summary>
        /// The count for notes.
        /// </summary>
        public string NotesCount => $"Notes: {this.NoteItems.Count}";

        /// <summary>
        /// The count for items.
        /// </summary>
        public string TranscriptionsCount => $"Transcriptions: {this.TranscriptionItems.Count}";

        /// <summary>
        /// Return the key words.
        /// </summary>
        /// <returns>Returns a list of key words in the search.</returns>
        private List<string> GetKeyWords()
        {
            if (String.IsNullOrEmpty(this.search)) return null;
            var split = this.search.Split(" ");
            return split.Where(x => x.Length > 0).ToList();
        }

        /// <summary>
        /// Gets a list of applicable tags from keywords.
        /// </summary>
        /// <returns>Returns a list of tags.</returns>
        private IEnumerable<Tag> GetApplicableTags(List<string> keyWords)
        {
            for (int i = 0; i < keyWords.Count; i++)
            {
                foreach (var tag in this.linker.AllTags)
                {
                    if (tag.Name.ToLower().Contains(keyWords[i].ToLower()))
                    {
                        yield return tag;
                    }
                }
            }
        }
    }
}