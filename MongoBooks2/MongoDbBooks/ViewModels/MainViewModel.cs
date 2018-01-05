namespace MongoDbBooks.ViewModels
{
    using MongoDbBooks.Models;

    public class MainViewModel : BaseViewModel
    {
        #region Constructor

        public MainViewModel(MainWindow mainWindow, log4net.ILog log)
        {
            _mainWindow = mainWindow;
            _log = log;

            _mainModel = new MainBooksModel(log);

            _dataLoaderVM = new DataLoaderViewModel(_mainWindow, log, _mainModel, this);
            _dataUpdaterVM = new DataUpdaterViewModel(_mainWindow, log, _mainModel, this);
            _dataGridsVM = new DataGridsViewModel(_mainWindow, log, _mainModel, this);
            _chartsVM = new ChartsViewModel(_mainWindow, log, _mainModel, this);
            _diagramsVM = new DiagramsViewModel(_mainWindow, log, _mainModel, this);
            _mailboxLoaderVM = new MailboxLoaderViewModel(_mainWindow, log, _mainModel, this);
            _chartSelectionVM = new ChartSelectionViewModel(_mainWindow, log, _mainModel, this);
            _mailboxLoaderVM = new MailboxLoaderViewModel(_mainWindow, log, _mainModel, this);
            _exportersVM = new ExportersViewModel(_mainWindow, log, _mainModel, this);
            _reportsVM = new ReportsViewModel(_mainWindow, log, _mainModel, this);
            _bloggerVM = new BloggerViewModel(_mainWindow, log, _mainModel, this);
            _usersVM = new UsersViewModel(_mainWindow, log, _mainModel, this);

            if (_mainModel.ConnectedToDbSuccessfully)
                UpdateData();
        }

        #endregion

        #region Private data

        private readonly MainWindow _mainWindow;
        private log4net.ILog _log;

        private readonly MainBooksModel _mainModel;

        private readonly DataLoaderViewModel _dataLoaderVM;
        private readonly DataUpdaterViewModel _dataUpdaterVM;
        private readonly DataGridsViewModel _dataGridsVM;
        private readonly ChartsViewModel _chartsVM;
        private readonly DiagramsViewModel _diagramsVM;
        private readonly ChartSelectionViewModel _chartSelectionVM;
        private readonly MailboxLoaderViewModel _mailboxLoaderVM;
        private readonly ExportersViewModel _exportersVM;
        private readonly ReportsViewModel _reportsVM;
        private readonly BloggerViewModel _bloggerVM;
        private readonly UsersViewModel _usersVM;

        #endregion

        #region Public Properties

        public log4net.ILog Log => _log;

        public MainBooksModel MainModel => _mainModel;

        public DataLoaderViewModel DataLoaderVM => _dataLoaderVM; 

        public DataUpdaterViewModel DataUpdaterVM => _dataUpdaterVM;

        public DataGridsViewModel DataGridsVM => _dataGridsVM;

        public ChartsViewModel ChartsVM => _chartsVM;

        public DiagramsViewModel DiagramsVM => _diagramsVM;

        public ChartSelectionViewModel ChartSelectionVM => _chartSelectionVM;

        public MailboxLoaderViewModel MailboxLoaderVM => _mailboxLoaderVM;

        public ExportersViewModel ExportersVM => _exportersVM;

        public ReportsViewModel ReportsVM => _reportsVM;

        public BloggerViewModel BloggerVM => _bloggerVM;

        public UsersViewModel UsersVM => _usersVM;

        #endregion

        #region Public Methods

        public void UpdateData()
        {
            DiagramsVM.UpdateData();
            ChartsVM.UpdateData();
            DataGridsVM.UpdateData();
            DataUpdaterVM.UpdateData();
            ChartSelectionVM.UpdateData();
            ReportsVM.UpdateData();
        }

        #endregion
    }
}
