using Avalonia.Collections;
using JazzNotes.Models;
using ReactiveUI;
using System.Linq;

namespace JazzNotes.ViewModels
{
    public class StartupViewModel : ViewModelBase
    {
        private readonly Linker linker;
        private string search;
        private int selectedIndex;

        /// <summary>
        /// The inital startup viewmodel.
        /// </summary>
        /// <param name="items">The list of transcriptions.</param>
        public StartupViewModel(Linker linker)
        {
            this.linker = linker;
            this.Tags = new AvaloniaList<Tag>();
        }

        /// <summary>
        /// Gets all the tags for autocomplete.
        /// </summary>
        public AvaloniaList<string> AutoCompleteItems => new AvaloniaList<string>(this.linker.AllTags
            .Where(x => !this.Tags.Contains(x)).Select(x => x.Name));

        /// <summary>
        /// The list of notes to display.
        /// </summary>
        public AvaloniaList<Note> NoteItems
        {
            get
            {
                var allNotes = this.linker.Transcriptions.SelectMany(x => x.Notes);

                if (this.Tags.Count > 0)
                {
                    var notes = new AvaloniaList<Note>();
                    var tagCount = 0;

                    foreach (var note in allNotes)
                    {
                        foreach (var tag in this.Tags)
                        {
                            if (!note.Tags.Contains(tag)) continue;
                            tagCount++;
                        }
                        if (tagCount == this.Tags.Count)
                        {
                            notes.Add(note);
                        }
                        tagCount = 0;
                    }
                    return notes;
                }

                return new AvaloniaList<Note>(allNotes);
            }
        }

        /// <summary>
        /// The count for notes.
        /// </summary>
        public string NotesCount => $"Notes: {this.NoteItems.Count}";

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
        /// Tags to search for.
        /// </summary>
        public AvaloniaList<Tag> Tags { get; }

        /// <summary>
        /// The list of tasks to display.
        /// </summary>
        public AvaloniaList<TaskNote> TaskItems
        {
            get
            {
                return this.linker.Tasks;
            }
        }

        /// <summary>
        /// The list of transcriptions to display.
        /// </summary>
        public AvaloniaList<Transcription> TranscriptionItems
        {
            get
            {
                if (this.Tags.Count > 0)
                {
                    var transcriptions = new AvaloniaList<Transcription>();
                    var tagCount = 0;

                    foreach (var transcription in this.linker.Transcriptions)
                    {
                        foreach (var tag in this.Tags)
                        {
                            if (!transcription.Tags.Contains(tag)) continue;
                            tagCount++;
                        }
                        if (tagCount == this.Tags.Count)
                        {
                            transcriptions.Add(transcription);
                        }
                        tagCount = 0;
                    }
                    return transcriptions;
                }

                return this.linker.Transcriptions;
            }
        }

        /// <summary>
        /// The count for items.
        /// </summary>
        public string TranscriptionsCount => $"Transcriptions: {this.TranscriptionItems.Count}";

        /// <summary>
        /// Add a tag to search.
        /// </summary>
        /// <param name="name">Name of tag.</param>
        /// <returns>Whether the tag was added or not.</returns>
        public bool AddTag(string name)
        {
            var tag = this.linker.AllTags.FirstOrDefault(x => x.Name == name);
            if (tag != null && !this.Tags.Contains(tag))
            {
                this.Tags.Add(tag);
                this.RaiseListChanged();
                return true;
            }
            return false;
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
            this.RaisePropertyChanged(nameof(this.TaskItems));
            this.RaisePropertyChanged(nameof(this.AutoCompleteItems));
        }

        /// <summary>
        /// Remove a tag.
        /// </summary>
        public void RemoveTag(Tag tag)
        {
            this.Tags.Remove(tag);
            this.RaiseListChanged();
        }
    }
}