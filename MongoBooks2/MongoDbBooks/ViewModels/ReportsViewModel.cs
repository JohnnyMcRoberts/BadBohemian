namespace MongoDbBooks.ViewModels
{
    using System.Windows.Input;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.ComponentModel;
    using System.IO;
    using System.Linq.Expressions;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Markup;
    using System.Windows.Threading;
    using System.Xml;

    using MongoDbBooks.Models;
    using MongoDbBooks.ViewModels.Utilities;
    using MongoDbBooks.ViewModels.PlotGenerators;
    using OxyPlot.Wpf;

    public class ReportsViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Members

        void OnPropertyChanged<T>(Expression<Func<T>> sExpression)
        {
            if (sExpression == null) throw new ArgumentNullException("sExpression");

            MemberExpression body = sExpression.Body as MemberExpression;
            if (body == null)
            {
                throw new ArgumentException("Body must be a member expression");
            }
            OnPropertyChanged(body.Member.Name);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion // INotifyPropertyChanged Members

        #region Private Data

        private MainWindow _mainWindow;
        private log4net.ILog _log;
        private readonly MainBooksModel _mainModel;
        private MainViewModel _parent;
        private DateTime _selectedMonth;
        private DateTime _firstMonth;
        private DateTime _lastMonth;
        //private TalliedMonth _selectedMonthTally;

        /// <summary>
        /// The print command.
        /// </summary>
        private ICommand _printCommand;

        /// <summary>
        /// The month report has loaded command.
        /// </summary>
        private ICommand _monthlyReportLoadedCommand;

        #endregion

        #region Constants


        #endregion

        #region Public Data

        public string Title => "Reports";

        public ObservableCollection<TalliedMonth> TalliedMonths { get; set; }

        public DateTime SelectedMonth
        {
            get { return _selectedMonth; }
            set
            {
                if (_selectedMonth.Year != value.Year || _selectedMonth.Month != value.Month)
                {
                    _selectedMonth = value;
                    _mainModel.SelectedMonthTally = GetSelectedMonthTally();
                    OnPropertyChanged(() => SelectedMonth);
                    OnPropertyChanged(() => SelectedMonthTally);
                    OnPropertyChanged(() => SelectedMonthBooksRead);
                    PlotCurrentMonthPagesReadByLanguage.UpdateData(_mainModel);
                    PlotCurrentMonthPagesReadByCountry.UpdateData(_mainModel);
                    PlotCurrentMonthDocumentPagesReadByLanguage.UpdateData(_mainModel);
                    PlotCurrentMonthDocumentPagesReadByCountry.UpdateData(_mainModel);
                    PlotCurrentMonthPrintPagesReadByLanguage.UpdateData(_mainModel);
                    PlotCurrentMonthPrintPagesReadByCountry.UpdateData(_mainModel);
                }
            }
        }

        public DateTime FirstMonth
        {
            get { return _firstMonth; }
            set
            {
                _firstMonth = value;
                OnPropertyChanged(() => FirstMonth);
            }
        }

        public DateTime LastMonth
        {
            get { return _lastMonth; }
            set
            {
                _lastMonth = value;
                OnPropertyChanged(() => LastMonth);
            }
        }

        public TalliedMonth SelectedMonthTally => _mainModel.SelectedMonthTally;

        public List<BookRead> SelectedMonthBooksRead => _mainModel.SelectedMonthTally.BooksRead;

        public OxyPlotPair PlotCurrentMonthPagesReadByLanguage { get; private set; }

        public OxyPlotPair PlotCurrentMonthPagesReadByCountry { get; private set; }

        public OxyPlotPair PlotCurrentMonthDocumentPagesReadByLanguage { get; private set; }

        public OxyPlotPair PlotCurrentMonthDocumentPagesReadByCountry { get; private set; }

        public OxyPlotPair PlotCurrentMonthPrintPagesReadByLanguage { get; private set; }

        public OxyPlotPair PlotCurrentMonthPrintPagesReadByCountry { get; private set; }

        public FlowDocument MonthlyReportDocument { get; set; }

        public string ReportTitle => "Report for " + SelectedMonth.ToString("MMMM yyyy");

        #endregion

        #region Constructor

        public ReportsViewModel(
            MainWindow mainWindow, log4net.ILog log,
            MainBooksModel mainModel, MainViewModel parent)
        {
            _mainWindow = mainWindow;
            _log = log;
            _mainModel = mainModel;
            _parent = parent;

            FirstMonth = _mainModel.TalliedMonths.First().MonthDate;
            var lastMonth = _mainModel.TalliedMonths.Last().MonthDate;
            lastMonth = lastMonth.AddMonths(1);
            lastMonth = lastMonth.AddDays(-1);
            LastMonth = lastMonth;
            _selectedMonth = lastMonth.AddMonths(-1);
            TalliedMonths = new ObservableCollection<TalliedMonth>();
            _mainModel.SelectedMonthTally = GetSelectedMonthTally();

            PlotCurrentMonthPagesReadByLanguage =
                new OxyPlotPair(new CurrentMonthPagesReadByLanguagePlotGenerator(), "CurrentMonthPagesReadByLanguage");
            PlotCurrentMonthPagesReadByCountry =
                new OxyPlotPair(new CurrentMonthPagesReadByCountryPlotGenerator(), "CurrentMonthPagesReadByCountry");

            PlotCurrentMonthDocumentPagesReadByLanguage =
                new OxyPlotPair(new CurrentMonthPagesReadByLanguagePlotGenerator(), "CurrentMonthPagesReadByLanguage");
            PlotCurrentMonthDocumentPagesReadByCountry =
                new OxyPlotPair(new CurrentMonthPagesReadByCountryPlotGenerator(), "CurrentMonthPagesReadByCountry");

            PlotCurrentMonthPrintPagesReadByLanguage =
                new OxyPlotPair(new CurrentMonthPagesReadByLanguagePlotGenerator(), "CurrentMonthPagesReadByLanguage");
            PlotCurrentMonthPrintPagesReadByCountry =
                new OxyPlotPair(new CurrentMonthPagesReadByCountryPlotGenerator(), "CurrentMonthPagesReadByCountry");
        }

        // should think about a flow doc and then html ....
        // https://msdn.microsoft.com/en-us/library/aa972129.aspx

        #endregion

        #region Public Methods

        public void UpdateData()
        {
            // if no data don't waste time trying to create plots
            if (_mainModel == null || _mainModel.BooksRead == null || _mainModel.BooksRead.Count == 0)
                return;

            PlotCurrentMonthPagesReadByLanguage.UpdateData(_mainModel);
            PlotCurrentMonthPagesReadByCountry.UpdateData(_mainModel);

            PlotCurrentMonthDocumentPagesReadByLanguage.UpdateData(_mainModel);
            PlotCurrentMonthDocumentPagesReadByCountry.UpdateData(_mainModel);

            PlotCurrentMonthPrintPagesReadByLanguage.UpdateData(_mainModel);
            PlotCurrentMonthPrintPagesReadByCountry.UpdateData(_mainModel);
            OnPropertyChanged("");
        }

        #endregion

        #region Utility Functions

        private TalliedMonth GetSelectedMonthTally()
        {
            var selected = _mainModel.TalliedMonths.First();
            foreach(var month in _mainModel.TalliedMonths)
            {
                if (month.MonthDate.Year == _selectedMonth.Year && month.MonthDate.Month == _selectedMonth.Month)
                {
                    selected = month;
                    break;
                }
            }
            TalliedMonths.Clear();
            TalliedMonths.Add(selected);
            OnPropertyChanged(() => TalliedMonths);
            return selected;
        }

        #endregion

        #region Commands

        /// <summary>
        /// Gets the select image for nation command.
        /// </summary>
        public ICommand PrintCommand => _printCommand ??
                                            (_printCommand =
                                             new RelayCommandHandler(PrintCommandAction) { IsEnabled = true });

        public ICommand MonthlyReportLoadedCommand => _monthlyReportLoadedCommand ??
                                                      (_monthlyReportLoadedCommand =
                                                          new RelayCommandHandler(MonthlyReportLoadedCommandAction) { IsEnabled = true });

        #endregion

        #region Command Handlers

        /// <summary>
        /// The command action to add a new book from the email to the database.
        /// </summary>
        public void MonthlyReportLoadedCommandAction(object parameter)
        {
        }

        private static string ForceRenderFlowDocumentXaml =
@"<Window xmlns=""http://schemas.microsoft.com/netfx/2007/xaml/presentation""
        xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
    <FlowDocumentScrollViewer Name=""viewer""/>
</Window>";

        public static void ForceRenderFlowDocument(FlowDocument document)
        {
            using (XmlTextReader reader = new XmlTextReader(new StringReader(ForceRenderFlowDocumentXaml)))
            {
                Window window = XamlReader.Load(reader) as Window;
                if (window == null)
                    return;
                FlowDocumentScrollViewer viewer = LogicalTreeHelper.FindLogicalNode(window, "viewer") as FlowDocumentScrollViewer;
                if (viewer == null)
                    return;
                viewer.Document = document;
                // Show the window way off-screen
                window.WindowStartupLocation = WindowStartupLocation.Manual;
                window.Top = Int32.MaxValue;
                window.Left = Int32.MaxValue;
                window.ShowInTaskbar = false;
                window.Show();
                // Ensure that dispatcher has done the layout and render passes
                Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Loaded, new Action(() => { }));
                viewer.Document = null;
                window.Close();
            }
        }

        private void DoThePrint(FlowDocument document)
        {
            // Clone the source document's content into a new FlowDocument.
            // This is because the pagination for the printer needs to be
            // done differently than the pagination for the displayed page.
            // We print the copy, rather that the original FlowDocument.
            MemoryStream s = new MemoryStream();
            TextRange source = new TextRange(document.ContentStart, document.ContentEnd);
            source.Save(s, DataFormats.Xaml);

            FlowDocument doc = GetPrintFlowDocument();

            ForceRenderFlowDocument(doc);
            
            // get information about the dimensions of the seleted printer+media.
            System.Printing.PrintDocumentImageableArea ia = null;
            System.Windows.Xps.XpsDocumentWriter docWriter = System.Printing.PrintQueue.CreateXpsDocumentWriter(ref ia);

            if (docWriter != null && ia != null)
            {
                DocumentPaginator paginator = ((IDocumentPaginatorSource)doc).DocumentPaginator;

                // Change the PageSize and PagePadding for the document to match the CanvasSize for the printer device.
                paginator.PageSize = new Size(ia.MediaSizeWidth, ia.MediaSizeHeight);
                Thickness t = new Thickness(72);  // copy.PagePadding;
                doc.PagePadding = new Thickness(
                                 Math.Max(ia.OriginWidth, t.Left),
                                   Math.Max(ia.OriginHeight, t.Top),
                                   Math.Max(ia.MediaSizeWidth - (ia.OriginWidth + ia.ExtentWidth), t.Right),
                                   Math.Max(ia.MediaSizeHeight - (ia.OriginHeight + ia.ExtentHeight), t.Bottom));

                doc.ColumnWidth = double.PositiveInfinity;
                //copy.PageWidth = 528; // allow the page to be the natural with of the output device

                // Send content to the printer.
                docWriter.Write(paginator);
            }

        }

        private FlowDocument GetPrintFlowDocument()
        {
            FlowDocument doc = new FlowDocument();

            Paragraph p = new Paragraph(new Run("Monthly Report")) { FontSize = 36, FontWeight = FontWeights.Bold };
            doc.Blocks.Add(p);

            BlockUIContainer titleBlockContainer = GetTitleBlockContainer();
            doc.Blocks.Add(titleBlockContainer);


            BlockUIContainer chartBlockUiContainerBlockContainer = GetChartBlockUiContainerBlockContainer();
            doc.Blocks.Add(chartBlockUiContainerBlockContainer);

            return doc;
        }

        private BlockUIContainer GetTitleBlockContainer()
        {
            BlockUIContainer titleBlockContainer = new BlockUIContainer();

            StackPanel stackPanel = new StackPanel {Orientation = Orientation.Vertical, Width = 1000};

            Label testLabel = new Label();
            Binding reportTitleBinding = new Binding
            {
                Source = this,
                Path = new PropertyPath("ReportTitle"),
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };

            BindingOperations.SetBinding(testLabel, ContentControl.ContentProperty, reportTitleBinding);

            stackPanel.Children.Add(testLabel);

            titleBlockContainer.Child = stackPanel;
            return titleBlockContainer;
        }

        private BlockUIContainer GetChartBlockUiContainerBlockContainer()
        {
            BlockUIContainer chartBlockUiContainerBlockContainer = new BlockUIContainer();

            StackPanel chartsStackPanel = new StackPanel {Orientation = Orientation.Vertical, Width = 800};

            PlotView plotViewByCountry = new PlotView {Height = 400, Width = 600, Padding = new Thickness(10)};
            Binding chartModelByCountryBinding = new Binding
            {
                Source = this,
                Path = new PropertyPath("PlotCurrentMonthPrintPagesReadByCountry.Model"),
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            BindingOperations.SetBinding(plotViewByCountry, PlotView.ModelProperty, chartModelByCountryBinding);

            Binding chartControllerByCountryBinding = new Binding
            {
                Source = this,
                Path = new PropertyPath("PlotCurrentPrintMonthPagesReadByCountry.ViewController"),
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            BindingOperations.SetBinding(plotViewByCountry, PlotView.ControllerProperty, chartControllerByCountryBinding);

            chartsStackPanel.Children.Add(plotViewByCountry);


            //PlotCurrentMonthPrintPagesReadByLanguage



            PlotView plotViewByLanguage = new PlotView { Height = 400, Width = 600, Padding = new Thickness(10) };
            Binding chartModelByLanguageBinding = new Binding
            {
                Source = this,
                Path = new PropertyPath("PlotCurrentMonthPrintPagesReadByLanguage.Model"),
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            BindingOperations.SetBinding(plotViewByLanguage, PlotView.ModelProperty, chartModelByLanguageBinding);

            Binding chartControllerByLanguageBinding = new Binding
            {
                Source = this,
                Path = new PropertyPath("PlotCurrentPrintMonthPagesReadByLanguage.ViewController"),
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            BindingOperations.SetBinding(plotViewByLanguage, PlotView.ControllerProperty, chartControllerByLanguageBinding);

            chartsStackPanel.Children.Add(plotViewByLanguage);

            chartBlockUiContainerBlockContainer.Child = chartsStackPanel;
            return chartBlockUiContainerBlockContainer;
        }


        /// <summary>
        /// The command action to add a new book from the email to the database.
        /// </summary>
        public void PrintCommandAction(object parameter)
        {
            if (MonthlyReportDocument == null)
                return;

            DoThePrint(MonthlyReportDocument);
#if old

            string path = "test.xps";

            System.IO.Packaging.Package package = System.IO.Packaging.Package.Open(path, System.IO.FileMode.Create);
            System.Windows.Xps.Packaging.XpsDocument document = new System.Windows.Xps.Packaging.XpsDocument(package);
            System.Windows.Xps.XpsDocumentWriter writer = System.Windows.Xps.Packaging.XpsDocument.CreateXpsDocumentWriter(document);
            //FixedDocument doc = new FixedDocument();

            //FixedPage page1 = new FixedPage();
            //PageContent page1Content = new PageContent();
            //((System.Windows.Markup.IAddChild)page1Content).AddChild(page1);

            //// add the content
            ////System.Windows.Controls.Button button = new System.Windows.Controls.Button() { Content = };

            ////if (MonthlyReportDocument != null)
            ////page1.Children.Add(MonthlyReportDocument);

            //doc.Pages.Add(page1Content);
            //writer.Write(doc);
            //document.Close();
            //package.Close();

            System.Windows.Controls.PrintDialog printDialog = new System.Windows.Controls.PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                printDialog.PrintDocument(((IDocumentPaginatorSource)MonthlyReportDocument).DocumentPaginator, "Flow Document Print Job");
            }

#endif
        }

#endregion

    }
}
