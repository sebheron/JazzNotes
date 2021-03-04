using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JazzNotes.Helpers
{
    public static class PathHelper
    {
        /// <summary>
        /// Main JazzNotes directory.
        /// </summary>
        public static readonly string JazzNotesDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "JazzNotes");

        /// <summary>
        /// Main JazzNotes directory.
        /// </summary>
        public static readonly string DataFilePath = Path.Combine(JazzNotesDirectory, "Data.xml");

        /// <summary>
        /// Directory for Transcriptions.
        /// </summary>
        public static readonly string TranscriptionsDirectory = Path.Combine(JazzNotesDirectory, "Transcriptions");

        /// <summary>
        /// Directory for Images.
        /// </summary>
        public static readonly string ImagesDirectory = Path.Combine(JazzNotesDirectory, "Images");

        /// <summary>
        /// Paths for images created temporarily.
        /// </summary>
        public static readonly string ImgPath = Path.Combine(JazzNotesDirectory, "temp{0}.png");

        /// <summary>
        /// Directory for GhostScript on Windows.
        /// </summary>
        public static readonly string GhostscriptDirectoryWindows = Path.Combine(Directory.GetCurrentDirectory(), "Ghostscript");
    }
}
