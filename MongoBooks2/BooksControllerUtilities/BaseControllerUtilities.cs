using System;
using System.Collections.Generic;
using System.Text;

namespace BooksControllerUtilities
{
    using System.Security.Authentication;
    using BooksControllerUtilities.Settings;
    using MongoDB.Driver;

    public abstract class BaseControllerUtilities
    {
        /// <summary>
        /// The database settings.
        /// </summary>
        protected MongoDbSettings MongoDbSettings { get; private set; }

        /// <summary>
        /// Gets a database client using the connection string.
        /// </summary>
        /// <param name="databaseName">The name of the database to connect to.</param>
        /// <returns>The database client for the connection string.</returns>
        protected MongoClient GetRemoteConnection(string databaseName)
        {
            string host = MongoDbSettings.RemoteHost;
            string userName = MongoDbSettings.RemoteUserName;
            string password = MongoDbSettings.RemotePassword;
            int port = MongoDbSettings.RemotePort;

            MongoClientSettings settings = new MongoClientSettings();
            settings.Server = new MongoServerAddress(host, port);
            settings.UseSsl = MongoDbSettings.RemoteUseSsl;
            settings.SslSettings = new SslSettings();
            settings.SslSettings.EnabledSslProtocols = SslProtocols.Tls12;
            settings.RetryWrites = MongoDbSettings.RemoteRetryWrites;

            MongoIdentity identity = new MongoInternalIdentity(databaseName, userName);
            MongoIdentityEvidence evidence = new PasswordEvidence(password);

            settings.Credential = new MongoCredential("SCRAM-SHA-1", identity, evidence);

            MongoClient client = new MongoClient(settings);
            return client;
        }

        public BaseControllerUtilities(MongoDbSettings mongoDbSettings)
        {
            MongoDbSettings = mongoDbSettings;
        }
    }
}
