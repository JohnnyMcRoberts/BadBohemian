namespace MongoDbBooks.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Threading;
    using System.Xml;

    using MongoDbBooks.Models;
    using MongoDbBooks.Models.Exporters;
    using MongoDbBooks.ViewModels.Utilities;
    using MongoDbBooks.ViewModels.PlotGenerators;

    using BlogReadWrite;

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

        /// <summary>
        /// The print command.
        /// </summary>
        private ICommand _printCommand;

        private ICommand _saveAsHtmlCommand;

        private ICommand _postToBlogCommand;

        #endregion

        #region Constants


        private static string ForceRenderFlowDocumentXaml =
            @"<Window xmlns=""http://schemas.microsoft.com/netfx/2007/xaml/presentation""
                    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
                <FlowDocumentScrollViewer Name=""viewer""/>
            </Window>";

        private const string DataTemplateForBookWithNotes =
          @"<DataTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">
                <Grid Margin=""10,10"" Width=""460"">
                    <Grid.RowDefinitions>
                        <RowDefinition Height=""150""/>
                        <RowDefinition/>
                    </Grid.RowDefinitions>
                    <Grid.ColumnDefinitions>
                        <ColumnDefinition/>
                        <ColumnDefinition MinWidth=""330""/>
                    </Grid.ColumnDefinitions>

                    <Image Grid.Row=""0"" Grid.Column=""0""  
                        HorizontalAlignment=""Left"" VerticalAlignment=""Center"" Source=""{Binding DisplayImage}"" />

                    <Grid Grid.Row=""0"" Grid.Column=""1"">
                        <Grid.RowDefinitions>
                            <RowDefinition/>
                            <RowDefinition/>
                            <RowDefinition/>
                            <RowDefinition/>
                            <RowDefinition/>
                            <RowDefinition/>
                            <RowDefinition/>
                        </Grid.RowDefinitions>
                        <TextBlock Grid.Row=""0"" TextWrapping=""Wrap"">
                            <Run Text=""Title: "" FontWeight=""Bold"" />
                            <Run Text=""{Binding Title}"" />
                        </TextBlock>
                        <TextBlock Grid.Row=""1"" TextWrapping=""Wrap"">
                            <Run Text=""Author: "" FontWeight=""Bold"" />
                            <Run Text=""{Binding Author}"" />
                        </TextBlock>
                        <TextBlock Grid.Row=""2"" TextWrapping=""Wrap"">
                            <Run Text=""Pages: "" FontWeight=""Bold"" />
                            <Run Text=""{Binding Pages}"" />
                        </TextBlock>
                        <TextBlock Grid.Row=""3"" TextWrapping=""Wrap"">
                            <Run Text=""Nationality: "" FontWeight=""Bold"" />
                            <Run Text=""{Binding Nationality}"" />
                        </TextBlock>
                        <TextBlock Grid.Row=""4"" TextWrapping=""Wrap"">
                            <Run Text=""Original Language: "" FontWeight=""Bold"" />
                            <Run Text=""{Binding OriginalLanguage}"" />
                        </TextBlock>
                        <TextBlock Grid.Row=""5"" TextWrapping=""Wrap"">
                            <Run Text=""Date: "" FontWeight=""Bold"" />
                            <Run Text=""{Binding DateString}"" />
                        </TextBlock>
                        <TextBlock Grid.Row=""6"" TextWrapping=""Wrap"">
                            <Run Text=""Format: "" FontWeight=""Bold"" />
                            <Run Text=""{Binding Format}"" />
                        </TextBlock>
                    </Grid>

                    <TextBlock Grid.Row=""1"" Grid.Column=""0"" Grid.ColumnSpan=""2""
                                TextWrapping=""Wrap"" MaxWidth=""450"" HorizontalAlignment=""Left"">
                        <Run Text=""Notes: "" FontWeight=""Bold"" />
                        <Run Text=""{Binding Note}"" />
                    </TextBlock>
                </Grid>
            </DataTemplate>";


        private const string DataTemplateForBookWithotNotesXaml =
            @"<DataTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">
                <Grid Margin=""10, 10"" Width=""460"">
                    <Grid.RowDefinitions>
                        <RowDefinition Height = ""150"" />
                        <RowDefinition />
                    </Grid.RowDefinitions>
 
                    <Grid.ColumnDefinitions>
                        <ColumnDefinition />
                        <ColumnDefinition MinWidth = ""330"" />
                    </Grid.ColumnDefinitions>

                    <Image Grid.Row = ""0"" Grid.Column = ""0"" HorizontalAlignment = ""Left"" 
                        VerticalAlignment = ""Center"" Source = ""{Binding DisplayImage}"" />
								
                    <Grid Grid.Row = ""0"" Grid.Column = ""1"" >
                        <Grid.RowDefinitions>
                            <RowDefinition />
                            <RowDefinition />       
                            <RowDefinition />
                            <RowDefinition />
                            <RowDefinition />
                            <RowDefinition />
                            <RowDefinition />
                        </Grid.RowDefinitions>
                        <TextBlock Grid.Row = ""0"" TextWrapping = ""Wrap"" >
                            <Run Text = ""Title: "" FontWeight = ""Bold"" />
                            <Run Text = ""{ Binding Title}"" />
                        </TextBlock>
                        <TextBlock Grid.Row = ""1"" TextWrapping = ""Wrap"" >
                            <Run Text = ""Author: "" FontWeight = ""Bold"" />
                        </TextBlock>
                        <TextBlock Grid.Row=""2"" TextWrapping=""Wrap"">
                    <Run Text=""Pages: "" FontWeight=""Bold"" />
                    <Run Text=""{Binding Pages}"" />
                        </TextBlock>
                        <TextBlock Grid.Row=""3"" TextWrapping=""Wrap"">
                    <Run Text=""Nationality: "" FontWeight=""Bold"" />
                    <Run Text=""{Binding Nationality}"" />
                        </TextBlock>
                        <TextBlock Grid.Row=""4"" TextWrapping=""Wrap"">
                    <Run Text=""Original Language: "" FontWeight=""Bold"" />
                    <Run Text=""{Binding OriginalLanguage}"" />
                        </TextBlock>
                        <TextBlock Grid.Row=""5"" TextWrapping=""Wrap"">
                    <Run Text=""Date: "" FontWeight=""Bold"" />
                    <Run Text=""{Binding DateString}"" />
                        </TextBlock>
                        <TextBlock Grid.Row=""6"" TextWrapping=""Wrap"">
                    <Run Text=""Format: "" FontWeight=""Bold"" />
                    <Run Text=""{Binding Format}"" />
                        </TextBlock>
                    </Grid>  

                </Grid>
            </DataTemplate>";

        #endregion

        #region Public Data

        public string Title => "Reports";

        public ObservableCollection<TalliedMonth> TalliedMonths { get; set; }

        public ObservableCollection<MonthlyReportsTally> ReportsTallies { get; private set; }

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
                    UpdateReportsTallies();
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

        public UIElement CurrentMonthPagesReadByCountryChart { get; set; }

        public UIElement CurrentMonthPagesReadByLanguageChart { get; set; }

        public string ReportTitle => "Report for " + SelectedMonth.ToString("MMMM yyyy");

        public Table TotalsTable
        {
            get
            {
                Table totalsTable = new Table();
                FillTotalsTable(totalsTable);
                return totalsTable;
            }
        }

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

            ReportsTallies = 
                new ObservableCollection<MonthlyReportsTally>
                {
                    new MonthlyReportsTally(_mainModel.SelectedMonthTally),
                    new MonthlyReportsTally(_mainModel.BookDeltas.Last().OverallTally)
                };
        }
        
        #endregion

        #region Public Methods

        public void UpdateData()
        {
            // if no data don't waste time trying to create plots
            if (_mainModel?.BooksRead == null || _mainModel.BooksRead.Count == 0)
                return;

            PlotCurrentMonthPagesReadByLanguage.UpdateData(_mainModel);
            PlotCurrentMonthPagesReadByCountry.UpdateData(_mainModel);

            PlotCurrentMonthDocumentPagesReadByLanguage.UpdateData(_mainModel);
            PlotCurrentMonthDocumentPagesReadByCountry.UpdateData(_mainModel);

            PlotCurrentMonthPrintPagesReadByLanguage.UpdateData(_mainModel);
            PlotCurrentMonthPrintPagesReadByCountry.UpdateData(_mainModel);

            UpdateReportsTallies();

            OnPropertyChanged("");
        }

        #endregion

        #region Utility Functions

        private void UpdateReportsTallies()
        {
            ReportsTallies.Clear();
            ReportsTallies.Add(new MonthlyReportsTally(_mainModel.SelectedMonthTally));
            ReportsTallies.Add(new MonthlyReportsTally(_mainModel.BookDeltas.Last().OverallTally));
            OnPropertyChanged(() => ReportsTallies);
        }

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

        private void ForceRenderFlowDocument(FlowDocument document)
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

            for (int i = 0; i < SelectedMonthBooksRead.Count; i++)
            {
                BlockUIContainer bookBlockContainer = GetBookBlockContainer(i, SelectedMonthBooksRead[i]);
                if (bookBlockContainer != null)
                {
                    doc.Blocks.Add(bookBlockContainer);
                }
            }
            
            doc.Blocks.Add(TotalsTable);

            BlockUIContainer chartBlockUiContainerBlockContainer = GetChartBlockUiContainerBlockContainer();
            doc.Blocks.Add(chartBlockUiContainerBlockContainer);

            return doc;
        }

        private void FillTotalsTable(Table table)
        {
            SetTableGlobalFormatting(table);

            CreateTableColumns(table);

            CreateTableOverallHeader(table);

            CreateTableSubHeaders(table);
            
            AddTotalsTableDataRows(table, GetTotalsTableRowValues());
        }

        private static void SetTableGlobalFormatting(Table table)
        {
            // Set some global formatting properties for the table.
            table.CellSpacing = 10;
            table.Background = System.Windows.Media.Brushes.White;
        }

        private static void CreateTableColumns(Table table)
        {
            // Create the columns and add them to the table's Columns collection.
            for (int i = 0; i < 3; i++)
            {
                table.Columns.Add(new TableColumn());

                // Set alternating background colors for the middle colums.
                table.Columns[i].Background =
                    i != 0 ? System.Windows.Media.Brushes.Beige : System.Windows.Media.Brushes.LightSteelBlue;
            }
        }

        private static void CreateTableOverallHeader(Table table)
        {
            // Create and add an empty TableRowGroup to hold the table's Rows.
            table.RowGroups.Add(new TableRowGroup());

            // Add the first (title) row.
            table.RowGroups[0].Rows.Add(new TableRow());

            // Alias the current working row for easy reference.
            TableRow currentRow = table.RowGroups[0].Rows[0];

            // Global formatting for the title row.
            currentRow.Background = System.Windows.Media.Brushes.Silver;
            currentRow.FontSize = 40;
            currentRow.FontWeight = FontWeights.Bold;

            // Add the header row with content, 
            currentRow.Cells.Add(
                new TableCell(new Paragraph(new Run("Monthly Totals")) { TextAlignment = TextAlignment.Center }));

            // and set the row to span all the columns.
            currentRow.Cells[0].ColumnSpan = 3;
        }

        private void CreateTableSubHeaders(Table table)
        {
            // Add the second (header) row.
            table.RowGroups[0].Rows.Add(new TableRow());
            TableRow currentRow = table.RowGroups[0].Rows[1];

            // Global formatting for the header row.
            currentRow.FontSize = 18;
            currentRow.FontWeight = FontWeights.Bold;

            // Add cells with content to the second row.
            currentRow.Cells.Add(new TableCell(new Paragraph(new Run(""))));
            currentRow.Cells.Add(
                new TableCell(new Paragraph(new Run(ReportsTallies[0].DisplayTitle)) { TextAlignment = TextAlignment.Center }));
            currentRow.Cells.Add(
                new TableCell(new Paragraph(new Run(ReportsTallies[1].DisplayTitle)) { TextAlignment = TextAlignment.Center }));
        }

        private List<Tuple<string, string, string>> GetTotalsTableRowValues()
        {
            List<Tuple<string, string, string>> rowValues = new List<Tuple<string, string, string>>
            {
                new Tuple<string, string, string>(
                    "Days", ReportsTallies[0].TotalDays.ToString(), ReportsTallies[1].TotalDays.ToString()),
                new Tuple<string, string, string>(
                    "Total Books", ReportsTallies[0].TotalBooks.ToString(), ReportsTallies[1].TotalBooks.ToString()),
                new Tuple<string, string, string>(
                    "Book", ReportsTallies[0].TotalBookFormat.ToString(), ReportsTallies[1].TotalBookFormat.ToString()),
                new Tuple<string, string, string>(
                    "Comic", ReportsTallies[0].TotalComicFormat.ToString(), ReportsTallies[1].TotalComicFormat.ToString()),
                new Tuple<string, string, string>(
                    "Audio", ReportsTallies[0].TotalAudioFormat.ToString(), ReportsTallies[1].TotalAudioFormat.ToString()),
                new Tuple<string, string, string>(
                    "% in English", ReportsTallies[0].PercentageInEnglish.ToString("N2"), ReportsTallies[1].PercentageInEnglish.ToString("N2")),
                new Tuple<string, string, string>(
                    "% in Translation", ReportsTallies[0].PercentageInTranslation.ToString("N2"), ReportsTallies[1].PercentageInTranslation.ToString("N2")),
                new Tuple<string, string, string>(
                    "Page Rate", ReportsTallies[0].PageRate.ToString("N2"), ReportsTallies[1].PageRate.ToString("N2")),
                new Tuple<string, string, string>(
                    "Days per Book", ReportsTallies[0].DaysPerBook.ToString("N2"), ReportsTallies[1].DaysPerBook.ToString("N2")),
                new Tuple<string, string, string>(
                    "Pages per Book", ReportsTallies[0].PagesPerBook.ToString("N2"), ReportsTallies[1].PagesPerBook.ToString("N2")),
                new Tuple<string, string, string>(
                    "Books per Year", ReportsTallies[0].BooksPerYear.ToString("N2"), ReportsTallies[1].BooksPerYear.ToString("N2"))
            };
            return rowValues;
        }

        private static void AddTotalsTableDataRows(Table table, List<Tuple<string, string, string>> rowValues)
        {
            // Add the data rows.
            for (int i = 0; i < rowValues.Count; i++)
            {
                // Add the data row.
                table.RowGroups[0].Rows.Add(new TableRow());
                TableRow currentRow = table.RowGroups[0].Rows[2 + i];

                // Global formatting for the row.
                currentRow.FontSize = 14;
                currentRow.FontWeight = FontWeights.Normal;

                // Add cells with content to the third row.
                currentRow.Cells.Add(
                    new TableCell(new Paragraph(new Run(rowValues[i].Item1)) { TextAlignment = TextAlignment.Left }));
                currentRow.Cells.Add(
                    new TableCell(new Paragraph(new Run(rowValues[i].Item2)) { TextAlignment = TextAlignment.Right }));
                currentRow.Cells.Add(
                    new TableCell(new Paragraph(new Run(rowValues[i].Item3)) { TextAlignment = TextAlignment.Right }));

                // Bold the first cell.
                currentRow.Cells[0].FontWeight = FontWeights.Bold;
            }
        }

        private BlockUIContainer GetBookBlockContainer(int itemIndex, BookRead bookRead)
        {
            BlockUIContainer bookBlockContainer = new BlockUIContainer();

            ContentControl contentControl = new ContentControl { Margin = new Thickness(10), MinHeight = 100 };

            string itemPath = "SelectedMonthBooksRead[" + itemIndex + "]";

            Binding itemSourceBinding = new Binding
            {
                Source = this,
                Path = new PropertyPath(itemPath),
                Mode = BindingMode.OneWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            BindingOperations.SetBinding(contentControl, ContentControl.ContentProperty, itemSourceBinding);

            contentControl.ContentTemplate = CreateDataTemplate(!string.IsNullOrEmpty(bookRead.Note));

            bookBlockContainer.Child = contentControl;
            return bookBlockContainer;
        }

        public DataTemplate CreateDataTemplate(bool hasNotes = false)
        {
            StringReader stringReader =
                new StringReader(hasNotes ? DataTemplateForBookWithNotes : DataTemplateForBookWithotNotesXaml);
            XmlReader xmlReader = XmlReader.Create(stringReader);
            return XamlReader.Load(xmlReader) as DataTemplate;
        }

        private BlockUIContainer GetTitleBlockContainer()
        {
            BlockUIContainer titleBlockContainer = new BlockUIContainer();

            StackPanel stackPanel = new StackPanel { Orientation = Orientation.Vertical, Width = 1000 };

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

            StackPanel chartsStackPanel = new StackPanel { Orientation = Orientation.Vertical, Width = 800 };

            PlotView plotViewByCountry = new PlotView { Height = 400, Width = 600, Padding = new Thickness(10) };
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

        private void GetChartFilesForPlots(IList<string> chartFiles)
        {
            chartFiles.Clear();
            chartFiles.Add(CreateLocalImageFileForPlot(CurrentMonthPagesReadByCountryChart));
            chartFiles.Add(CreateLocalImageFileForPlot(CurrentMonthPagesReadByLanguageChart));
        }

        private string CreateLocalImageFileForPlot(UIElement plotElement)
        {
            // Generate a new temporary file.
            var ticks = DateTime.Now.Ticks;
            string outputDir = _mainModel.DefaultExportDirectory;
            string temporaryFileName = outputDir + @"\chart_" + ticks + ".png";

            // Get a control to render the image into.
            UIElement target = plotElement;
            double dpiX = 96.0;
            double dpiY = 96.0;
            if (target == null)
            {
                return null;
            }

            // Render it into a target bitmap.
            Rect bounds = VisualTreeHelper.GetDescendantBounds(target);
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)(bounds.Width * dpiX / 96.0),
                (int)(bounds.Height * dpiY / 96.0),
                dpiX,
                dpiY,
                PixelFormats.Pbgra32);

            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext ctx = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(target);
                ctx.DrawRectangle(vb, null, new Rect(new Point(), bounds.Size));
            }

            rtb.Render(dv);

            // Save the encoded bitmap to a file.
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(rtb));
            using (Stream stm = File.Create(temporaryFileName))
                encoder.Save(stm);

            return temporaryFileName;
        }

        #endregion

        #region Commands

        /// <summary>
        /// Gets the select image for nation command.
        /// </summary>
        public ICommand PrintCommand => _printCommand ??
                                            (_printCommand =
                                             new RelayCommandHandler(PrintCommandAction) { IsEnabled = true });

        public ICommand SaveAsHtmlCommand => _saveAsHtmlCommand ??
                                             (_saveAsHtmlCommand =
                                                 new RelayCommandHandler(SaveAsHtmlCommandAction) { IsEnabled = true });


        public ICommand PostToBlogCommand => _postToBlogCommand ??
                                             (_postToBlogCommand =
                                                 new RelayCommandHandler(PostToBlogCommandAction) { IsEnabled = true });

        #endregion

        #region Command Handlers

        /// <summary>
        /// The command action to generate a document and send it to the printer.
        /// </summary>
        public void PrintCommandAction(object parameter)
        {
            if (MonthlyReportDocument == null)
                return;

            DoThePrint(MonthlyReportDocument);
        }

        /// <summary>
        /// The command action to generate an html document for the selected month.
        /// </summary>
        public void SaveAsHtmlCommandAction(object parameter)
        {
            if (MonthlyReportDocument == null)
                return;

            // Create image files for the charts.
            IList<string> chartFiles = new List<string>();
            GetChartFilesForPlots(chartFiles);

            ExportMonthlyReportToToHtml exporter = 
                new ExportMonthlyReportToToHtml(SelectedMonthTally, ReportsTallies, chartFiles);
            string outfile;
            if (exporter.WriteToFile(out outfile))
            {
                
            }
        }


        /// <summary>
        /// The command action to generate an html document for the selected month.
        /// </summary>
        public void PostToBlogCommandAction(object parameter)
        {
            if (MonthlyReportDocument == null)
                return;
            
            ExportMonthlyReportToToHtml exporter =
                new ExportMonthlyReportToToHtml(SelectedMonthTally, ReportsTallies, new List<string>(), true);
            string title;
            string content;
            if (exporter.GetAsBlogPost(out title, out content))
            {
                BlogReadWrite.ViewModels.AddBlogPostViewModel addPostVM = 
                    new BlogReadWrite.ViewModels.AddBlogPostViewModel()
                {
                    BlogPostTitle = title,
                    BlogPostContent = content,
                    ApplictionName = "Books DB",
                    WindowTitle = "Books Blogger"
                };

                BlogReadWrite.Views.AddBlogPostView bloggerDialog = new BlogReadWrite.Views.AddBlogPostView
                { DataContext = addPostVM };
                bloggerDialog.ShowDialog();
            }
        }

        #endregion

        #region Nested classes

        public class MonthlyReportsTally
        {
            public MonthlyReportsTally(BooksDelta.DeltaTally overallTally)
            {
                DisplayTitle = "Overall";
                //TotalDays = talliedBook.;
                TotalDays = overallTally.DaysInTally;
                TotalBooks = overallTally.TotalBooks;
                TotalPagesRead = overallTally.TotalPages;
                TotalBookFormat = overallTally.TotalBookFormat;
                TotalComicFormat = overallTally.TotalComicFormat;
                TotalAudioFormat = overallTally.TotalAudioFormat;
                PercentageInEnglish = overallTally.PercentageInEnglish;
            }

            public MonthlyReportsTally(TalliedMonth selectedMonthTally)
            {
                DisplayTitle = selectedMonthTally.DisplayString;
                TotalDays = selectedMonthTally.DaysInTheMonth;
                TotalBooks = selectedMonthTally.TotalBooks;
                TotalPagesRead = selectedMonthTally.TotalPagesRead;
                TotalBookFormat = selectedMonthTally.TotalBookFormat;
                TotalComicFormat = selectedMonthTally.TotalComicFormat;
                TotalAudioFormat = selectedMonthTally.TotalAudioFormat;
                PercentageInEnglish = selectedMonthTally.PercentageInEnglish;
            }

            /// <summary>
            /// Gets the title to display.
            /// </summary>
            public string DisplayTitle { get; set; }

            /// <summary>
            /// Gets or sets the number of days in the tallied period.
            /// </summary>
            public int TotalDays { get; set; }

            /// <summary>
            /// Gets the total books read in the month.
            /// </summary>
            public UInt32 TotalBooks { get; set; }

            /// <summary>
            /// Gets the total pages read in the month.
            /// </summary>
            public UInt32 TotalPagesRead { get; set; }

            /// <summary>
            /// Gets the total number of physical books for the month.
            /// </summary>
            public UInt32 TotalBookFormat { get; set; }

            /// <summary>
            /// Gets the total number of comics for the month.
            /// </summary>
            public UInt32 TotalComicFormat { get; set; }

            /// <summary>
            /// Gets the total number of audiobooks for the month.
            /// </summary>
            public UInt32 TotalAudioFormat { get; set; }

            /// <summary>
            /// Gets the percentage of books read in English for the month.
            /// </summary>
            public double PercentageInEnglish { get; set; }

            /// <summary>
            /// Gets the percentage of books read in translation for the month.
            /// </summary>
            public double PercentageInTranslation => 100.0 - PercentageInEnglish;

            /// <summary>
            /// Gets the page rate for the month.
            /// </summary>
            public double PageRate => TotalPagesRead / (double)TotalDays;

            /// <summary>
            /// Gets the average days per book for the month.
            /// </summary>
            public double DaysPerBook => TotalDays / (double)TotalBooks;

            /// <summary>
            /// Gets the average pages per book for the month.
            /// </summary>
            public double PagesPerBook => TotalPagesRead / (double)TotalBooks;

            /// <summary>
            /// Gets the expected books per year for the month.
            /// </summary>
            public double BooksPerYear => 365.25 / DaysPerBook;
        }

        #endregion
    }
}
