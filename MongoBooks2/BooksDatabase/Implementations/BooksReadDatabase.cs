// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BooksReadDatabase.cs" company="N/A">
//   2017-2086
// </copyright>
// <summary>
//   The user MongoDb database.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BooksDatabase.Implementations
{
    using System;
    using System.Collections.ObjectModel;
    using BooksCore.Books;
    using MongoDB.Bson;
    using MongoDB.Driver;

    /// <summary>
    /// The books read database.
    /// </summary>
    public class BooksReadDatabase : BaseDatabaseConnection<BookRead>
    {
        /// <summary>
        /// Gets or sets the connection string for the database.
        /// </summary>
        public sealed override string DatabaseConnectionString { get; protected set; }

        /// <summary>
        /// Gets or sets the name of the database.
        /// </summary>
        public sealed override string DatabaseName { get; protected set; }

        /// <summary>
        /// Gets or sets the name of the collection.
        /// </summary>
        public sealed override string CollectionName { get; protected set; }

        /// <summary>
        /// Gets or sets the database collection filter.
        /// </summary>
        public sealed override FilterDefinition<BookRead> Filter { get; protected set; }

        /// <summary>
        /// Gets or sets the set of items from the database.
        /// </summary>
        public sealed override ObservableCollection<BookRead> LoadedItems { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BooksReadDatabase"/> class.
        /// </summary>
        /// <param name="dbConnection">The connection string for the database.</param>
        /// <param name="connectAtStartup">Whether or not to connect to the database at startup.</param>
        public BooksReadDatabase(string dbConnection, bool connectAtStartup = true) 
        {
            DatabaseConnectionString = dbConnection;

            LoadedItems = new ObservableCollection<BookRead>();

            // This is a query to get everything to date. 
            Filter = Builders<BookRead>.Filter.Lte(
                new StringFieldDefinition<BookRead, BsonDateTime>("date"), new BsonDateTime(DateTime.Now.AddDays(1000)));

            DatabaseName = "books_read";
            CollectionName = "books";

            if (connectAtStartup)
            {
                ConnectToDatabase();
            }
        }
    }
}
