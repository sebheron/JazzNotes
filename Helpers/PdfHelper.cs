using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media.Imaging;
using ImageMagick;
using MessageBox.Avalonia;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JazzNotes.Helpers
{
    public class PdfHelper : IDisposable
    {
        private Window window;

        private readonly string GhostscriptDirectoryWindows = Path.Combine(System.AppContext.BaseDirectory, "Ghostscript");

        private readonly string imgPath = System.IO.Directory.GetCurrentDirectory() + "/temp{0}.jpg";

        private readonly List<string> imgPaths;

        public PdfHelper()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                MagickNET.SetGhostscriptDirectory(GhostscriptDirectoryWindows);
            }
            this.window = ((IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime).MainWindow;
            this.imgPaths = new List<string>();
        }

        public async Task<string> ShowDialog()
        {
            OpenFileDialog myDialog = new OpenFileDialog();
            myDialog.Filters.Add(new FileDialogFilter() { Name = "PDF files (*.PDF)", Extensions = new List<string> { "pdf" } });
            myDialog.AllowMultiple = false;

            var result = await myDialog.ShowAsync(this.window);

            return result[0];
        }

        public void LoadPDF(string path)
        {
            using var collection = new MagickImageCollection();
            using var stream = new MemoryStream();

            if (!File.Exists(path))
            {
                var messageBoxStandardWindow = MessageBoxManager
                    .GetMessageBoxStandardWindow("JazzNotes", "PDF file is missing and cannot be loaded.");
                messageBoxStandardWindow.Show();
                this.Image = null;
            }

            collection.Read(path);

            using (IMagickImage vertical = collection.AppendVertically())
            {
                vertical.Format = MagickFormat.Png;

                var newPath = string.Format(this.imgPath, imgPaths.Count + 1);
                imgPaths.Add(newPath);

                this.Height = vertical.Height;
                this.Width = vertical.Width;

                vertical.Write(newPath);

                this.FilePath = path;
                this.Image = new Bitmap(newPath);
            }
        }

        public Bitmap GetSnip(Rect draw)
        {
            var image = new MagickImage();

            image.Read(this.imgPaths[0]);
            image.Crop(new MagickGeometry((int)draw.Left, (int)draw.Top, (int)draw.Width, (int)draw.Height), Gravity.Northwest);

            image.Format = MagickFormat.Png;

            var newPath = string.Format(this.imgPath, imgPaths.Count + 1);
            imgPaths.Add(newPath);

            image.Write(newPath);

            return new Bitmap(newPath);
        }

        public void Dispose()
        {
            this.Clean();
        }

        public void Clean()
        {
            this.Image = null;
            imgPaths.Clear();
            var paths = Directory.GetFiles(Directory.GetCurrentDirectory()).Where(x => x.EndsWith(".png"));
            try
            {
                foreach (var path in paths)
                {
                    File.Delete(path);
                }
            }
            catch
            {
                Debug.WriteLine("Error deleting file, may be in use");
            }
        }

        public Bitmap Image { get; private set; }

        public double Width { get; private set; }

        public double Height { get; private set; }

        public string FilePath { get; private set; }
    }
}