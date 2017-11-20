using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MongoDbBooks.Views
{
    using MongoDbBooks.ViewModels;

    /// <summary>
    /// Interaction logic for BookReportsView.xaml
    /// </summary>
    public partial class BookReportsView : UserControl
    {
        public BookReportsView()
        {
            InitializeComponent();
        }

        private void MonthlyReportDocument_OnLoaded(object sender, RoutedEventArgs e)
        {
            ReportsViewModel reports = DataContext as ReportsViewModel;
            FlowDocument doc = sender as FlowDocument;
            if (reports != null && doc != null)
            {
                reports.MonthlyReportDocument = doc;
            }
        }
    }
}
