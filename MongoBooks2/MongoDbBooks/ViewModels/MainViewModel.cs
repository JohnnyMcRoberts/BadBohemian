using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Linq.Expressions;

using MongoDbBooks.Models;
using MongoDbBooks.ViewModels.Utilities;

namespace MongoDbBooks.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
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

            if (_mainModel.ConnectedToDbSuccessfully)
                UpdateData();
        }

        #endregion

        #region Private data

        private MainWindow _mainWindow;
        private log4net.ILog _log;

        private MainBooksModel _mainModel;

        private DataLoaderViewModel _dataLoaderVM;
        private DataUpdaterViewModel _dataUpdaterVM;
        private DataGridsViewModel _dataGridsVM;
        private ChartsViewModel _chartsVM;
        private DiagramsViewModel _diagramsVM;
        private ChartSelectionViewModel _chartSelectionVM;
        private MailboxLoaderViewModel _mailboxLoaderVM;
        private ExportersViewModel _exportersVM;
        private ReportsViewModel _reportsVM;
        private BloggerViewModel _bloggerVM;

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
