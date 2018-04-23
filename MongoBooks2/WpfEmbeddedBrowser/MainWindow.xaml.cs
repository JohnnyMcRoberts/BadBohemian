
namespace WpfEmbeddedBrowser
{
    using CefSharp;
    using WpfEmbeddedBrowser.ViewModels;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly WpfEmbeddedBrowserMainViewModel _mainViewModel;

        public MainWindow()
        {
            CefSettings settings = new CefSettings {BrowserSubprocessPath = @"x86\CefSharp.BrowserSubprocess.exe"};
            Cef.Initialize(settings, performDependencyCheck: false, browserProcessHandler: null);
            InitializeComponent();
            WpfEmbeddedBrowserMainViewModel.RegisterTestResources(Browser);
            _mainViewModel = new WpfEmbeddedBrowserMainViewModel();
            DataContext = _mainViewModel;
        }

        private void Browser_OnLoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            _mainViewModel?.LoadingStateChanged(e);
        }
    }
}
