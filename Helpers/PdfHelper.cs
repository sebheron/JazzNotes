using Avalonia.Controls;
using Avalonia.Media.Imaging;
using ImageMagick;
using JazzNotes.Models;
using MessageBox.Avalonia;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace JazzNotes.Helpers
{
    public class PdfHelper
    {
        /// <summary>
        /// All of the current paths.
        /// </summary>
        private readonly List<string> imgPaths;

        /// <summary>
        /// Create new pdf helper.
        /// </summary>
        public PdfHelper()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                MagickNET.SetGhostscriptDirectory(PathHelper.GhostscriptDirectoryWindows);
            }
            this.imgPaths = new List<string>();

            Directory.CreateDirectory(PathHelper.TranscriptionsDirectory);
            Directory.CreateDirectory(PathHelper.ImagesDirectory);
        }

        /// <summary>
        /// Show the new dialog.
        /// </summary>
        /// <returns>The path retrieved.</returns>
        public async Task<string[]> ShowNewDialog()
        {
            OpenFileDialog myDialog = new OpenFileDialog()
            {
                Title = "Add New Transcription",
                AllowMultiple = true
            };
            myDialog.Filters.Add(new FileDialogFilter() { Name = "Supported files", Extensions = new List<string> { "pdf", "png" } });

            var result = await myDialog.ShowAsync(WindowHelper.MainWindow);

            return result;
        }

        /// <summary>
        /// Show the add image dialog.
        /// </summary>
        /// <returns>The path retrieved.</returns>
        public async Task<string> ShowAddImageDialog()
        {
            OpenFileDialog myDialog = new OpenFileDialog()
            {
                Title = "Add Image To Note",
                AllowMultiple = false
            };
            myDialog.Filters.Add(new FileDialogFilter() { Name = "Image files (*.PNG)", Extensions = new List<string> { "png" } });

            var result = await myDialog.ShowAsync(WindowHelper.MainWindow);

            return result[0];
        }

        /// <summary>
        /// Load a pdf from path.
        /// </summary>
        /// <param name="path">The new image path.</param>
        public void LoadPDF(string path)
        {
            var title = Path.GetFileNameWithoutExtension(path);
            var newPath = Path.Combine(PathHelper.TranscriptionsDirectory, $"{title}.png");

            int i = 0;
            while (File.Exists(newPath))
            {
                newPath = Path.Combine(PathHelper.TranscriptionsDirectory, $"{title} {i}.png");
                i++;
            }

            if (Path.GetExtension(path) == ".png")
            {
                using var loaded = new MagickImage(path);
                loaded.Alpha(AlphaOption.Remove);
                loaded.Write(newPath);
                this.LoadImage(newPath);
                return;
            }

            var settings = new MagickReadSettings()
            {
                Density = new Density(120)
            };
            using var collection = new MagickImageCollection();

            collection.Read(path, settings);

            using var vertical = collection.AppendVertically();

            vertical.Alpha(AlphaOption.Remove);
            vertical.Format = MagickFormat.Png;

            this.Height = vertical.Height;
            this.Width = vertical.Width;

            vertical.Write(newPath);

            this.FilePath = newPath;
            this.Image = new Bitmap(newPath);
        }

        /// <summary>
        /// Appends a pdf (to the currently loaded image) from path.
        /// </summary>
        /// <param name="path">The path for the image to append.</param>
        public void AppendPDF(string path)
        {
            if (string.IsNullOrEmpty(this.FilePath)) return;

            if (Path.GetExtension(path) == ".png")
            {
                using var joins = new MagickImageCollection();
                joins.Add(this.FilePath);
                joins.Add(path);

                if (joins[0].Width > joins[1].Width)
                {
                    joins[1].Resize(new Percentage((joins[0].Width / joins[1].Width) * 100));
                }
                else
                {
                    joins[0].Resize(new Percentage((joins[1].Width / joins[0].Width) * 100));
                }

                using var joined = joins.AppendVertically();
                joined.Alpha(AlphaOption.Remove);
                joined.Write(this.FilePath);

                this.LoadImage(this.FilePath);
                return;
            }

            var settings = new MagickReadSettings()
            {
                Density = new Density(120)
            };
            using var collection = new MagickImageCollection();

            collection.Read(path, settings);
            collection.Add(this.FilePath);

            using var vertical = collection.AppendVertically();

            vertical.Alpha(AlphaOption.Remove);
            vertical.Format = MagickFormat.Png;

            this.Height = vertical.Height;
            this.Width = vertical.Width;

            vertical.Write(this.FilePath);
            this.Image = new Bitmap(this.FilePath);
        }

        /// <summary>
        /// Load an image from path.
        /// </summary>
        /// <param name="path">The path to the image.</param>
        public void LoadImage(string path)
        {
            if (File.Exists(path))
            {
                this.FilePath = path;
                this.Image = new Bitmap(path);
                this.Height = this.Image.Size.Height;
                this.Width = this.Image.Size.Width;
            }
            else
            {
                var messageBoxStandardWindow = MessageBoxManager
                    .GetMessageBoxStandardWindow("JazzNotes", "Image file is missing and cannot be loaded.");
                messageBoxStandardWindow.Show();
                this.Image = null;
            }
        }

        /// <summary>
        /// Load an external image and get it.
        /// </summary>
        /// <param name="path">The path to the image.</param>
        /// <returns>The new path.</returns>
        public string LoadExternalImage(string path)
        {
            var title = Path.GetFileNameWithoutExtension(path);
            var newPath = Path.Combine(PathHelper.ImagesDirectory, title + ".png");

            if (!File.Exists(newPath))
            {
                using IMagickImage loaded = new MagickImage(path);
                loaded.Format = MagickFormat.Png;
                loaded.Alpha(AlphaOption.Remove);
                loaded.Write(newPath);
            }

            return newPath;
        }

        /// <summary>
        /// Get a snip from the image.
        /// </summary>
        /// <param name="draw">The snip.</param>
        /// <returns>A bitmap of the snip.</returns>
        public Bitmap GetSnip(IEnumerable<Snip> snips, string path)
        {
            if (snips.Count() < 1) return null;

            using var collection = new MagickImageCollection();

            var newPath = path;

            foreach (var snip in snips)
            {
                using var image = new MagickImage();

                image.Read(path);
                image.Crop(new MagickGeometry((int)snip.Rect.Left, (int)snip.Rect.Top, (int)snip.Rect.Width, (int)snip.Rect.Height), Gravity.Northwest);

                newPath = string.Format(PathHelper.ImgPath, imgPaths.Count + 1);
                imgPaths.Add(newPath);

                image.Write(newPath);

                collection.Add(newPath);
            }

            using var horizontal = collection.AppendHorizontally();
            horizontal.Write(newPath);

            return new Bitmap(newPath);
        }

        /// <summary>
        /// Clean the image directory.
        /// </summary>
        public void Clean()
        {
            this.Image = null;
            imgPaths.Clear();
            var paths = Directory.GetFiles(PathHelper.JazzNotesDirectory).Where(x => x.EndsWith(".png"));
            foreach (var path in paths)
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }

        /// <summary>
        /// The loaded image.
        /// </summary>
        public Bitmap Image { get; private set; }

        /// <summary>
        /// The width of the image.
        /// </summary>
        public double Width { get; private set; }

        /// <summary>
        /// The height of the image.
        /// </summary>
        public double Height { get; private set; }

        /// <summary>
        /// The file path for the loaded image.
        /// </summary>
        public string FilePath { get; private set; }
    }
}