using Avalonia;
using Avalonia.Media;
using JazzNotes.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace JazzNotes.Helpers
{
    public static class FileHelper
    {
        private static bool loading;

        /// <summary>
        /// Gets the loaded linker.
        /// </summary>
        public static Linker Linker { get; private set; }

        /// <summary>
        /// Loads in a linker.
        /// </summary>
        /// <returns>The loaded linker.</returns>
        public static Linker LoadLinker()
        {
            loading = true;

            var linker = new Linker();

            if (File.Exists(PathHelper.DataFilePath))
            {
                var root = XElement.Parse(File.ReadAllText(PathHelper.DataFilePath));

                if (root != null)
                {
                    var tags = root.Element("tags");
                    var transcriptions = root.Element("transcriptions");
                    var tasks = root.Element("tasks");

                    if (tags != null)
                    {
                        foreach (var tag in tags.Elements())
                        {
                            var name = tag.Attribute("name");
                            var color = tag.Attribute("color");

                            if (name == null | color == null) continue;

                            var brush = new SolidColorBrush(Color.Parse(color.Value), 1);
                            var newTag = new Tag(name.Value, brush);

                            linker.AllTags.Add(newTag);
                        }
                    }
                    if (transcriptions != null)
                    {
                        foreach (var transcription in transcriptions.Elements())
                        {
                            var path = transcription.Attribute("path");
                            var transcriptionName = transcription.Attribute("name");
                            var notes = transcription.Elements();
                            if (path == null) continue;

                            var nameValue = Path.GetFileNameWithoutExtension(path.Value);
                            if (transcriptionName != null)
                            {
                                nameValue = transcriptionName.Value;
                            }

                            var newTranscription = new Transcription(linker, path.Value, nameValue);

                            if (notes != null)
                            {
                                foreach (var note in transcription.Elements())
                                {
                                    var id = note.Attribute("id");
                                    var title = note.Attribute("title");
                                    var text = note.Attribute("text");
                                    var snip = note.Attribute("snip");
                                    var size = note.Attribute("size");
                                    var margin = note.Attribute("margin");
                                    var color = note.Attribute("color");
                                    var noteElements = note.Elements();

                                    if (snip == null | size == null | margin == null | color == null) continue;

                                    var titleValue = string.Empty;
                                    if (title != null)
                                    {
                                        titleValue = title.Value;
                                    }

                                    var textValue = string.Empty;
                                    if (text != null)
                                    {
                                        textValue = text.Value;
                                    }

                                    var idValue = Guid.NewGuid();
                                    if (id != null)
                                    {
                                        idValue = Guid.Parse(id.Value);
                                    }

                                    var rect = Rect.Parse(snip.Value);
                                    var thickness = Thickness.Parse(margin.Value);
                                    var brush = new SolidColorBrush(Color.Parse(color.Value), 1);
                                    var sizeValue = Size.Parse(size.Value);

                                    var newNote = new Note(idValue, newTranscription, rect, sizeValue, thickness, titleValue, textValue, brush);

                                    if (noteElements != null)
                                    {
                                        foreach (var element in noteElements)
                                        {
                                            if (element.Name == "tag")
                                            {
                                                var name = element.Attribute("name");
                                                if (name == null) continue;
                                                newNote.AddTag(name.Value);
                                            }
                                            else if (element.Name == "task")
                                            {
                                                var name = element.Attribute("name");
                                                var check = element.Attribute("check");
                                                if (name == null || check == null) continue;
                                                newNote.AddTask(name.Value, bool.Parse(check.Value));
                                            }
                                            else if (element.Name == "image")
                                            {
                                                var imagePath = element.Attribute("path");
                                                if (imagePath == null) continue;
                                                newNote.AddImage(imagePath.Value);
                                            }
                                        }
                                    }

                                    newTranscription.AddNote(newNote);
                                }
                            }

                            linker.Transcriptions.Add(newTranscription);
                        }
                    }
                    if (tasks != null)
                    {
                        foreach (var task in tasks.Elements())
                        {
                            var id = task.Attribute("id");
                            var check = task.Attribute("check");

                            if (id == null) continue;

                            var checkValue = false;
                            if (check != null)
                            {
                                checkValue = bool.Parse(check.Value);
                            }

                            var idValue = Guid.Parse(id.Value);
                            var note = linker.Transcriptions.SelectMany(x => x.Notes).First(x => x.ID == idValue);

                            var taskNote = new TaskNote(note, checkValue);

                            linker.Tasks.Add(taskNote);
                        }
                    }
                }
            }
            Linker = linker;
            loading = false;
            return linker;
        }

        /// <summary>
        /// Saves the loaded linker.
        /// </summary>
        public static void SaveLinker()
        {
            if (loading) return;

            Debug.WriteLine("Requested save.");

            if (File.Exists(PathHelper.DataFilePath))
            {
                File.Delete(PathHelper.DataFilePath);
            }

            using var stream = File.OpenWrite(PathHelper.DataFilePath);
            var root = new XElement("link");

            var tags = new XElement("tags");
            var usedTags = Linker.GetUsedTags();

            foreach (var tag in usedTags)
            {
                var ele = new XElement("tag");
                ele.SetAttributeValue("name", tag.Name);
                ele.SetAttributeValue("color", tag.Color.Color.ToString());
                tags.Add(ele);
            }
            root.Add(tags);

            var transcriptions = new XElement("transcriptions");
            foreach (var transcription in Linker.Transcriptions)
            {
                var ele = new XElement("transcription");
                ele.SetAttributeValue("path", transcription.FilePath);
                ele.SetAttributeValue("name", transcription.Name);
                foreach (var note in transcription.Notes)
                {
                    var ele2 = new XElement("note");
                    ele2.SetAttributeValue("id", note.ID.ToString());
                    ele2.SetAttributeValue("title", note.Title);
                    ele2.SetAttributeValue("text", note.Text);
                    ele2.SetAttributeValue("snip", note.Snip.ToString());
                    ele2.SetAttributeValue("size", note.Size.ToString());
                    ele2.SetAttributeValue("margin", note.Margin.ToString());
                    ele2.SetAttributeValue("color", note.Color.Color.ToString());
                    foreach (var tag in note.Tags)
                    {
                        var ele3 = new XElement("tag");
                        ele3.SetAttributeValue("name", tag.Name);
                        ele2.Add(ele3);
                    }
                    foreach (var task in note.Tasks)
                    {
                        var ele3 = new XElement("task");
                        ele3.SetAttributeValue("name", task.Name);
                        ele3.SetAttributeValue("check", task.Checked.ToString());
                        ele2.Add(ele3);
                    }
                    foreach (var image in note.Images)
                    {
                        var ele3 = new XElement("image");
                        ele3.SetAttributeValue("path", image.FilePath);
                        ele2.Add(ele3);
                    }
                    ele.Add(ele2);
                }
                transcriptions.Add(ele);
            }
            root.Add(transcriptions);

            var tasks = new XElement("tasks");
            foreach (var task in Linker.Tasks)
            {
                var ele = new XElement("task");
                ele.SetAttributeValue("id", task.Note.ID.ToString());
                ele.SetAttributeValue("check", task.Checked.ToString());
                tasks.Add(ele);
            }
            root.Add(tasks);

            root.Save(stream);

            Debug.WriteLine("Successfully saved.");
        }
    }
}