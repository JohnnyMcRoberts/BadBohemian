namespace BooksEditors.Views.Editors
{
    using System.Windows;
    using BooksEditors.ViewModels.Editors;
    using CefSharp;

    /// <summary>
    /// Interaction logic for ImageSelectionWindowView.xaml
    /// </summary>
    public partial class ImageSelectionWindowView : Window
    {
        private readonly ImageSelectionWindowViewModel _imageSelectionWindowViewModel;

        private static bool _initialised = false;

        public ImageSelectionWindowView(ImageSelectionWindowViewModel imageSelectionWindowViewModel)
        {
            _imageSelectionWindowViewModel = imageSelectionWindowViewModel;
            if (!_initialised)
            {
                CefSettings settings = new CefSettings { BrowserSubprocessPath = @"x86\CefSharp.BrowserSubprocess.exe" };
                Cef.Initialize(settings, performDependencyCheck: false, browserProcessHandler: null);
                _initialised = true;
            }
            InitializeComponent();
            ImageSelectionWindowViewModel.RegisterTestResources(Browser);

            DataContext = _imageSelectionWindowViewModel;
        }

        private void Browser_OnLoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            _imageSelectionWindowViewModel?.LoadingStateChanged(e);
        }
    }
}
