using Avalonia;
using Avalonia.Media;
using JazzNotes.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace JazzNotes.Helpers
{
    public static class FileHelper
    {
        private static readonly string DataFile = Path.Combine(AppContext.BaseDirectory, "AppData.dat");

        public static Linker LoadLinker()
        {
            var linker = new Linker();

            if (File.Exists(DataFile))
            {
                var root = XElement.Parse(File.ReadAllText(DataFile));

                if (root != null)
                {
                    var tags = root.Element("tags");
                    var transcriptions = root.Element("transcriptions");

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
                            var notes = transcription.Elements();
                            if (path == null) continue;

                            var newTranscription = new Transcription(linker, path.Value);

                            if (notes != null)
                            {
                                foreach (var note in transcription.Elements())
                                {
                                    var text = note.Attribute("text");
                                    var snip = note.Attribute("snip");
                                    var margin = note.Attribute("margin");
                                    var color = note.Attribute("color");
                                    var noteTags = note.Elements();

                                    if (text == null | snip == null | margin == null | color == null) continue;

                                    var rect = Rect.Parse(snip.Value);
                                    var thickness = Thickness.Parse(margin.Value);
                                    var brush = new SolidColorBrush(Color.Parse(color.Value), 1);

                                    var newNote = new Note(newTranscription, rect, thickness, text.Value, brush);

                                    if (noteTags != null)
                                    {
                                        foreach (var tag in noteTags)
                                        {
                                            var name = tag.Attribute("name");
                                            if (name == null) continue;
                                            newNote.AddTag(name.Value);
                                        }
                                    }

                                    newTranscription.AddNote(newNote);
                                }
                            }

                            linker.Transcriptions.Add(newTranscription);
                        }
                    }
                }
            }

            return linker;
        }

        public static void SaveLinker(Linker linker)
        {
            using (var stream = File.OpenWrite(DataFile))
            {
                var root = new XElement("link");

                var tags = new XElement("tags");
                var usedTags = linker.GetUsedTags();

                foreach (var tag in usedTags)
                {
                    var ele = new XElement("tag");
                    ele.SetAttributeValue("name", tag.Name);
                    ele.SetAttributeValue("color", tag.Color.Color.ToString());
                    tags.Add(ele);
                }
                root.Add(tags);

                var transcriptions = new XElement("transcriptions");
                foreach (var transcription in linker.Transcriptions)
                {
                    var ele = new XElement("transcription");
                    ele.SetAttributeValue("path", transcription.FilePath);
                    foreach (var note in transcription.Notes)
                    {
                        var ele2 = new XElement("note");
                        ele2.SetAttributeValue("text", note.Text);
                        ele2.SetAttributeValue("snip", note.Snip.ToString());
                        ele2.SetAttributeValue("margin", note.Margin.ToString());
                        ele2.SetAttributeValue("color", note.Color.Color.ToString());
                        foreach (var tag in note.Tags)
                        {
                            var ele3 = new XElement("tag");
                            ele3.SetAttributeValue("name", tag.Name);
                            ele2.Add(ele3);
                        }
                        ele.Add(ele2);
                    }
                    transcriptions.Add(ele);
                }
                root.Add(transcriptions);

                root.Save(stream);
            }
        }
    }
}
