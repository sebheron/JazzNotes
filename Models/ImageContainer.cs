using Avalonia.Media.Imaging;
using JazzNotes.Helpers;
using ReactiveUI;

namespace JazzNotes.Models
{
    public class ImageContainer : ReactiveObject
    {
        private string name;

        /// <summary>
        /// Create a new image container.
        /// </summary>
        /// <param name="path"></param>
        public ImageContainer(string path, string name)
        {
            this.Image = new Bitmap(path);
            this.Name = name;
            this.FilePath = path;
        }

        /// <summary>
        /// The filepath for the image.
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// The image.
        /// </summary>
        public Bitmap Image { get; }

        /// <summary>
        /// The image.
        /// </summary>
        public string Name
        {
            get => this.name;
            set
            {
                this.RaiseAndSetIfChanged(ref this.name, value);
                FileHelper.SaveLinker();
            }
        }
    }
}