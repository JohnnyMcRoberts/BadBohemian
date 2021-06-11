namespace BooksMigration.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Security.Authentication;
    using System.Threading.Tasks;
    using System.Windows;

    using Caliburn.Micro;
    using MongoDB.Driver;

    using BooksDatabase.Implementations;
    using BooksMigration.Models;

    public class MainViewModel : PropertyChangedBase
    {
        #region Private Data

        private BooksReadDatabase _localBooksReadDatabase;
        private NationDatabase _localNationDatabase;
        private UserDatabase _localUserDatabase;

        private BooksReadDatabase _remoteBooksReadDatabase;
        private NationDatabase _remoteNationDatabase;
        private UserDatabase _remoteUserDatabase;

        #endregion

        #region Public Data

        public SettingsViewModel SettingsVm { get; set; }

        public ObservableCollection<DataCollectionSettings> LocalDatabaseCollections { get; set; }

        public ObservableCollection<DataCollectionSettings> RemoteDatabaseCollections { get; set; }

        public string LocalConnectionString
        {
            get { return SettingsVm.LocalConnectionString; }
            set { SettingsVm.LocalConnectionString = value; }
        }

        public string RemoteHost
        {
            get { return SettingsVm.RemoteHost; }
            set { SettingsVm.RemoteHost = value; }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets a database client using the connection string.
        /// </summary>
        /// <param name="databaseName">The name of the database to connect to.</param>
        /// <returns>The database client for the connection string.</returns>
        private MongoClient GetRemoteConnection(string databaseName)
        {
            string host = SettingsVm.RemoteHost;
            string userName = SettingsVm.RemoteUserName;
            string password = SettingsVm.RemotePassword;
            int port = SettingsVm.RemotePort;

            MongoClientSettings settings = new MongoClientSettings();
            settings.Server = new MongoServerAddress(host, port);
            settings.UseSsl = SettingsVm.RemoteUseSsl;
            settings.SslSettings = new SslSettings();
            settings.SslSettings.EnabledSslProtocols = SslProtocols.Tls12;
            settings.RetryWrites = SettingsVm.RemoteRetryWrites;

            MongoIdentity identity = new MongoInternalIdentity(databaseName, userName);
            MongoIdentityEvidence evidence = new PasswordEvidence(password);

            settings.Credential = new MongoCredential("SCRAM-SHA-1", identity, evidence);

            MongoClient client = new MongoClient(settings);
            return client;
        }

        #endregion

        #region Public Methods

        public async void SayHello()
        {
            MessageBox.Show(string.Format("Hello {0}!", SettingsVm.LocalConnectionString)); //Don't do this in real life :)
        }

        public async Task SaveSettings()
        {
            await SettingsVm.SaveSettings();
        }

        public async Task SetupLocalCollections()
        {
            string connection = SettingsVm.LocalConnectionString;

            LocalDatabaseCollections.Clear();
            _localBooksReadDatabase = new BooksReadDatabase(connection);
            _localBooksReadDatabase.ConnectToDatabase();
            LocalDatabaseCollections.Add(new DataCollectionSettings(_localBooksReadDatabase));

            _localNationDatabase = new NationDatabase(LocalConnectionString);
            _localNationDatabase.ConnectToDatabase();
            LocalDatabaseCollections.Add(new DataCollectionSettings(_localNationDatabase));

            _localUserDatabase = new UserDatabase(LocalConnectionString);
            _localUserDatabase.ConnectToDatabase();
            LocalDatabaseCollections.Add(new DataCollectionSettings(_localUserDatabase));

            NotifyOfPropertyChange(() => LocalDatabaseCollections);
        }

        public async Task SetupRemoteCollections()
        {
            string connection = SettingsVm.LocalConnectionString;

            try
            {
                RemoteDatabaseCollections.Clear();

                _remoteBooksReadDatabase = new BooksReadDatabase(connection, true);
                _remoteBooksReadDatabase.MongoClientFunc = GetRemoteConnection;
                //_remoteBooksReadDatabase.ResetLoadedItems();
                _remoteBooksReadDatabase.ConnectToDatabase();
                RemoteDatabaseCollections.Add(new DataCollectionSettings(_remoteBooksReadDatabase));

                _remoteNationDatabase = new NationDatabase(connection, true);
                _remoteNationDatabase.MongoClientFunc = GetRemoteConnection;
                //_remoteNationDatabase.ResetLoadedItems();
                _remoteNationDatabase.ConnectToDatabase();
                RemoteDatabaseCollections.Add(new DataCollectionSettings(_remoteNationDatabase));

                _remoteUserDatabase = new UserDatabase(connection, true);
                _remoteUserDatabase.MongoClientFunc = GetRemoteConnection;
                //_remoteUserDatabase.ResetLoadedItems();
                _remoteUserDatabase.ConnectToDatabase();
                RemoteDatabaseCollections.Add(new DataCollectionSettings(_remoteUserDatabase));

                NotifyOfPropertyChange(() => RemoteDatabaseCollections);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        #endregion

        public MainViewModel()
        {
            SettingsVm = new SettingsViewModel();
            LocalDatabaseCollections = new ObservableCollection<DataCollectionSettings>();
            RemoteDatabaseCollections = new ObservableCollection<DataCollectionSettings>();

            try
            {
                _localBooksReadDatabase = new BooksReadDatabase(SettingsVm.LocalConnectionString, false);
                _localNationDatabase = new NationDatabase(SettingsVm.LocalConnectionString, false);
                _localUserDatabase = new UserDatabase(SettingsVm.LocalConnectionString, false);

                LocalDatabaseCollections.Add(new DataCollectionSettings(_localBooksReadDatabase));
                LocalDatabaseCollections.Add(new DataCollectionSettings(_localNationDatabase));
                LocalDatabaseCollections.Add(new DataCollectionSettings(_localUserDatabase));

                RemoteDatabaseCollections.Add(new DataCollectionSettings());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }
    }
}
