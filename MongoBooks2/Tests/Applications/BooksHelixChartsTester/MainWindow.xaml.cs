

namespace BooksHelixChartsTester
{
    using BooksHelixChartsTester.ViewModels;
    using System.Windows;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainViewModel mainVieWModel = new MainViewModel();
            DataContext = mainVieWModel;
        }
    }
}
