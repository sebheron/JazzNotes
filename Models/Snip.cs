﻿using Avalonia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JazzNotes.Models
{
    public class Snip
    {
        /// <summary>
        /// Create new snip.
        /// </summary>
        public Snip(Rect rect, Size size, Thickness margin)
        {
            this.Rect = rect;
            this.Size = size;
            this.Margin = margin;
        }

        /// <summary>
        /// The width of the snip.
        /// </summary>
        public Rect Rect { get; }

        /// <summary>
        /// The height of the snip.
        /// </summary>
        public Size Size { get; }

        /// <summary>
        /// The margin of the snip.
        /// </summary>
        public Thickness Margin { get; }
    }
}