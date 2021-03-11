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
        private TranscriptionViewModel viewmodel;

        private Image image;

        private Control cover, grid;

        private Point start, visualStart;

        private bool snipping, scrolled;

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

            if (pointerPoint.Properties.IsLeftButtonPressed
                && !pointerPoint.Properties.IsRightButtonPressed
                && this.viewmodel.ClickMode == 1)
            {
                this.snipping = !this.snipping;
                if (this.snipping)
                {
                    this.start = pointerPoint.Position;
                    this.visualStart = e.GetPosition(this.grid);
                    this.cover.IsVisible = true;
                    this.cover.Margin = new Thickness(this.visualStart.X, this.visualStart.Y, 0, 0);
                    this.cover.Width = 0;
                    this.cover.Height = 0;
                }
                else if (e.KeyModifiers == KeyModifiers.Shift)
                {
                    this.snipping = false;
                    this.cover.IsVisible = false;
                    var currentPos = e.GetPosition(this.image);
                    var checkMargin = this.GetMargin(currentPos, this.start);
                    this.viewmodel.AddDisplayRect(this.image.Bounds, new Rect(this.cover.Margin.Left, this.cover.Margin.Top, this.cover.Width, this.cover.Height),
                        new Rect(checkMargin.Left, checkMargin.Top, this.cover.Width, this.cover.Height));
                }
                else
                {
                    this.snipping = false;
                    this.cover.IsVisible = false;
                    var currentPos = e.GetPosition(this.image);
                    var checkMargin = this.GetMargin(currentPos, this.start);
                    this.viewmodel.AddDisplayRect(this.image.Bounds, new Rect(this.cover.Margin.Left, this.cover.Margin.Top, this.cover.Width, this.cover.Height),
                        new Rect(checkMargin.Left, checkMargin.Top, this.cover.Width, this.cover.Height));
                    this.viewmodel.AddNote();
                }
            }
        }

        private void TranscriptionScrollChanged(object? sender, ScrollChangedEventArgs e)
        {
            if (sender is ScrollViewer scrollViewer)
            {
                if (!scrolled)
                {
                    scrollViewer.Offset = this.viewmodel.ScrollPosition;
                    scrolled = true;
                }
                else
                {
                    this.viewmodel.ScrollPosition = scrollViewer.Offset;
                }
            }
        }

        private void OnImageMoved(object sender, PointerEventArgs e)
        {
            if (this.snipping)
            {
                var currentPos = e.GetPosition(this.grid);
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

            return new Thickness(left ? otherPosition.X : otherPosition.X - diff.X, top ? otherPosition.Y : otherPosition.Y - diff.Y, 0, 0);
        }

        private Point GetDifference(Point currentPosition, Point otherPosition)
        {
            return new Point(Math.Abs(currentPosition.X - otherPosition.X), Math.Abs(currentPosition.Y - otherPosition.Y));
        }
    }
}