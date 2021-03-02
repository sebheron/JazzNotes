using Avalonia.Media;
using System;

namespace JazzNotes.Helpers
{
    public static class ColorGenHelper
    {
        private static Random random = new Random();

        public static SolidColorBrush GenerateRandomBrush()
        {
            return new SolidColorBrush(new Color(255, (byte)random.Next(80, 220), (byte)random.Next(80, 220), (byte)random.Next(80, 220)), 1);
        }
    }
}