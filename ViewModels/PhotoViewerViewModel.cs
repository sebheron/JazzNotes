using Avalonia.Media.Imaging;
using ReactiveUI;

namespace JazzNotes.ViewModels
{
    public class PhotoViewerViewModel : ViewModelBase
    {
        private Bitmap image;
        private MainWindowViewModel mainViewModel;
        private ViewModelBase navigateBackViewModel;

        /// <summary>
        /// Create photo viewer viewmodel.
        /// </summary>
        public PhotoViewerViewModel(Bitmap image, MainWindowViewModel mainViewModel, ViewModelBase navigateBackViewModel)
        {
            this.Image = image;
            this.mainViewModel = mainViewModel;
            this.navigateBackViewModel = navigateBackViewModel;
        }

        /// <summary>
        /// The image to display.
        /// </summary>
        public Bitmap Image
        {
            get => this.image;
            set => this.RaiseAndSetIfChanged(ref this.image, value);
        }

        /// <summary>
        /// Navigate back to previous viewmodel.
        /// </summary>
        public void GoBackToNote()
        {
            this.mainViewModel.Content = this.navigateBackViewModel;
        }
    }
}