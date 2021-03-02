using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using ImageMagick;
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
        /// Directory for GhostScript.
        /// </summary>
        private readonly string GhostscriptDirectoryWindows = Path.Combine(System.AppContext.BaseDirectory, "Ghostscript");

        /// <summary>
        /// Directory for Transcriptions.
        /// </summary>
        private readonly string TranscriptionsDirectory = Path.Combine(System.AppContext.BaseDirectory, "Transcriptions");

        /// <summary>
        /// Paths for images created temporarily.
        /// </summary>
        private readonly string imgPath = System.IO.Directory.GetCurrentDirectory() + "/temp{0}.jpg";

        /// <summary>
        /// All of the current paths.
        /// </summary>
        private readonly List<string> imgPaths;

        public PdfHelper()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                MagickNET.SetGhostscriptDirectory(GhostscriptDirectoryWindows);
            }
            this.imgPaths = new List<string>();
            Directory.CreateDirectory(TranscriptionsDirectory);
        }

        public async Task<string> ShowDialog()
        {
            OpenFileDialog myDialog = new OpenFileDialog();
            myDialog.Filters.Add(new FileDialogFilter() { Name = "PDF files (*.PDF)", Extensions = new List<string> { "pdf" } });
            myDialog.Filters.Add(new FileDialogFilter() { Name = "PNG Image files (*.PNG)", Extensions = new List<string> { "png" } });
            myDialog.AllowMultiple = false;

            var result = await myDialog.ShowAsync(WindowHelper.MainWindow);

            return result[0];
        }

        public void LoadPDF(string path)
        {
            var title = Path.GetFileNameWithoutExtension(path);
            var newPath = Path.Combine(TranscriptionsDirectory, title);

            if (File.Exists(newPath))
            {
                this.LoadImage(newPath);
                return;
            }
            else if (Path.GetExtension(path) == ".png")
            {
                File.Copy(path, newPath);
                this.LoadImage(newPath);
                return;
            }

            using var collection = new MagickImageCollection();

            collection.Read(path);

            using (IMagickImage vertical = collection.AppendVertically())
            {
                vertical.Format = MagickFormat.Png;

                this.Height = vertical.Height;
                this.Width = vertical.Width;

                vertical.Write(newPath);

                this.FilePath = path;
                this.Image = new Bitmap(newPath);
            }
        }

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

        public Bitmap GetSnip(Rect draw)
        {
            var image = new MagickImage();

            image.Read(this.FilePath);
            image.Crop(new MagickGeometry((int)draw.Left, (int)draw.Top, (int)draw.Width, (int)draw.Height), Gravity.Northwest);

            image.Format = MagickFormat.Png;

            var newPath = string.Format(this.imgPath, imgPaths.Count + 1);
            imgPaths.Add(newPath);

            image.Write(newPath);

            return new Bitmap(newPath);
        }

        public void Clean()
        {
            this.Image = null;
            imgPaths.Clear();
            var paths = Directory.GetFiles(Directory.GetCurrentDirectory()).Where(x => Path.GetFileName(x).Contains("temp") && x.EndsWith(".png"));
            foreach (var path in paths)
            {
                File.Delete(path);
            }
        }

        public Bitmap Image { get; private set; }

        public double Width { get; private set; }

        public double Height { get; private set; }

        public string FilePath { get; private set; }
    }
}