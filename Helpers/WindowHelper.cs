using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JazzNotes.Helpers
{
    public static class WindowHelper
    {
        private static Window window;

        /// <summary>
        /// Gets the current main window for the application.
        /// </summary>
        public static Window MainWindow
        {
            get
            {
                if (window == null)
                {
                    window = ((IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime).MainWindow;
                }
                return window;
            }
        }
    }
}
