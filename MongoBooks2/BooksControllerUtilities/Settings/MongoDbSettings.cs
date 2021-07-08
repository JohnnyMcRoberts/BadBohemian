namespace BooksControllerUtilities.Settings
{
    public class MongoDbSettings
    {
        public string ExportDirectory { get; set; }

        public string DatabaseConnectionString { get; set; }

        public bool UseRemoteHost { get; set; }

        public string RemoteHost { get; set; }

        public int RemotePort { get; set; }

        public string RemoteUserName { get; set; }

        public string RemotePassword { get; set; }

        public bool RemoteUseSsl { get; set; }

        public bool RemoteRetryWrites { get; set; }

        public MongoDbSettings()
        {
            ExportDirectory = string.Empty;
            DatabaseConnectionString = string.Empty;
            UseRemoteHost = false;
            RemoteHost = string.Empty;
            RemotePort = 10255;
            RemoteUserName = string.Empty;
            RemotePassword = string.Empty;
            RemoteUseSsl = true;
            RemoteRetryWrites = false;
        }
    }
}
