﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

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

        /// <summary>
        /// Get and set the max width of the window.
        /// </summary>
        public static double MaxWidth => MainWindow.Screens.Primary.WorkingArea.Width;
    }
}