using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JazzNotes.Models
{
    public class ImageContainer
    {
        /// <summary>
        /// Create a new image container.
        /// </summary>
        /// <param name="path"></param>
        public ImageContainer(string path)
        {
            this.Image = new Bitmap(path);
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
    }
}
