using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using JazzNotes.ViewModels;
using System;

namespace JazzNotes.Views
{
    public class TranscriptionView : UserControl
    {
        private ScrollViewer scrollViewer;

        private Image image;

        private Control cover, grid;

        private Point start, visualStart;

        private bool pressed;

        public TranscriptionView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            this.cover = this.FindControl<Rectangle>("Cover");
            this.grid = this.FindControl<Grid>("PointGrid");
            this.image = this.FindControl<Image>("Image");
            this.scrollViewer = this.FindControl<ScrollViewer>("ScrollViewer");

            this.scrollViewer.Offset = new Vector(this.scrollViewer.Extent.Width / 2, 0);
        }

        private void OnImagePressed(object sender, PointerPressedEventArgs e)
        {
            var pointerPoint = e.GetCurrentPoint(this.image);
            if (pointerPoint.Properties.IsLeftButtonPressed
                || !pointerPoint.Properties.IsRightButtonPressed
                || !((TranscriptionViewModel)this.DataContext).ShowNotes) return;
            this.start = pointerPoint.Position;
            this.visualStart = e.GetPosition(this.grid);
            this.pressed = true;
            this.cover.IsVisible = true;
            this.cover.Margin = new Thickness(this.visualStart.X, this.visualStart.Y, 0, 0);
            this.cover.Width = 0;
            this.cover.Height = 0;
        }

        private void OnImageReleased(object sender, PointerReleasedEventArgs e)
        {
            if (this.pressed && this.cover.Width > 2 && this.cover.Height > 2 && this.DataContext is TranscriptionViewModel vm)
            {
                this.pressed = false;
                this.cover.IsVisible = false;

                var currentPos = e.GetPosition(this.image);
                var checkMargin = this.GetMargin(currentPos, this.start);
                vm.AddNote(this.image.Bounds, new Rect(this.cover.Margin.Left, this.cover.Margin.Top, this.cover.Width, this.cover.Height),
                    new Rect(checkMargin.Left, checkMargin.Top, this.cover.Width, this.cover.Height));
            }
        }

        private void OnImageMoved(object sender, PointerEventArgs e)
        {
            var currentPos = e.GetPosition(this.grid);
            if (this.pressed)
            {
                this.cover.Margin = this.GetMargin(currentPos, this.visualStart);
                this.cover.Width = Math.Abs(currentPos.X - this.visualStart.X);
                this.cover.Height = Math.Abs(currentPos.Y - this.visualStart.Y);
            }
        }

        private Thickness GetMargin(Point currentPosition, Point otherPosition)
        {
            bool left = currentPosition.X >= otherPosition.X;
            bool top = currentPosition.Y >= otherPosition.Y;

            var diff = this.GetDifference(currentPosition, otherPosition);

            return new Thickness(left ? otherPosition.X : otherPosition.X - diff.X, top ? otherPosition.Y: otherPosition.Y - diff.Y, 0, 0);
        }

        private Point GetDifference(Point currentPosition, Point otherPosition)
        {
            return new Point(Math.Abs(currentPosition.X - otherPosition.X), Math.Abs(currentPosition.Y - otherPosition.Y));
        }
    }
}
