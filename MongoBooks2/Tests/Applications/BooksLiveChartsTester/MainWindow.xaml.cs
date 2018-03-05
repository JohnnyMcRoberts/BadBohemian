namespace BooksLiveChartsTester
{
    using BooksLiveChartsTester.ViewModels;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            TesterViewModel vieWModel = new TesterViewModel();
            DataContext = vieWModel;
        }
    }
}
