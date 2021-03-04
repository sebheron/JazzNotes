using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using JazzNotes.Helpers;
using JazzNotes.ViewModels;
using System;

namespace JazzNotes.Views
{
    public class TranscriptionView : UserControl
    {
        private TranscriptionViewModel viewmodel;

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
        }

        protected override void OnDataContextChanged(EventArgs e)
        {
            if (this.viewmodel == null)
            {
                this.viewmodel = (TranscriptionViewModel)this.DataContext;
            }
            base.OnDataContextChanged(e);
        }

        private void OnImagePressed(object sender, PointerPressedEventArgs e)
        {
            var pointerPoint = e.GetCurrentPoint(this.image);
            if (pointerPoint.Properties.IsRightButtonPressed
                || !pointerPoint.Properties.IsLeftButtonPressed
                || !this.viewmodel.ShowNotes) return;
            this.viewmodel.CurrentCursor = CursorHelper.CrossCursor;
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
            if (this.pressed && this.cover.Width > 2 && this.cover.Height > 2)
            {
                this.pressed = false;
                this.cover.IsVisible = false;
                this.viewmodel.CurrentCursor = CursorHelper.ArrowCursor;

                var currentPos = e.GetPosition(this.image);
                var checkMargin = this.GetMargin(currentPos, this.start);
                this.viewmodel.AddNote(this.image.Bounds, new Rect(this.cover.Margin.Left, this.cover.Margin.Top, this.cover.Width, this.cover.Height),
                    new Rect(checkMargin.Left, checkMargin.Top, this.cover.Width, this.cover.Height));
            }
        }

        private void OnImageMoved(object sender, PointerEventArgs e)
        {
            var currentPos = e.GetPointerPoint(this.grid);
            if (this.pressed)
            {
                this.cover.Margin = this.GetMargin(currentPos.Position, this.visualStart);
                this.cover.Width = Math.Abs(currentPos.Position.X - this.visualStart.X);
                this.cover.Height = Math.Abs(currentPos.Position.Y - this.visualStart.Y);
            }
            this.pressed = currentPos.Properties.IsLeftButtonPressed;
            if (!pressed && this.viewmodel.CurrentCursor != CursorHelper.ArrowCursor) this.viewmodel.CurrentCursor = CursorHelper.ArrowCursor;
        }

        private Thickness GetMargin(Point currentPosition, Point otherPosition)
        {
            bool left = currentPosition.X >= otherPosition.X;
            bool top = currentPosition.Y >= otherPosition.Y;

            var diff = this.GetDifference(currentPosition, otherPosition);

            return new Thickness(left ? otherPosition.X : otherPosition.X - diff.X, top ? otherPosition.Y : otherPosition.Y - diff.Y, 0, 0);
        }

        private Point GetDifference(Point currentPosition, Point otherPosition)
        {
            return new Point(Math.Abs(currentPosition.X - otherPosition.X), Math.Abs(currentPosition.Y - otherPosition.Y));
        }
    }
}