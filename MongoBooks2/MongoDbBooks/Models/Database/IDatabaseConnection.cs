// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDatabaseConnection.cs" company="N/A">
//   2017-2086
// </copyright>
// <summary>
//   The nation MongoDb entity.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace MongoDbBooks.Models.Database
{
    using MongoDB.Driver;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Interface for a generic database connection.
    /// </summary>
    public interface IDatabaseConnection<T> where T : BaseEntity
    {
        /// <summary>
        /// Gets or sets the connection string for the database.
        /// </summary>
        string DatabaseConnectionString { get; }

        /// <summary>
        /// Gets or sets the name of the database.
        /// </summary>
        string DatabaseName { get; }

        /// <summary>
        /// Gets or sets the name of the collection.
        /// </summary>
        string CollectionName { get; }

        /// <summary>
        /// Gets or sets the database collection filter.
        /// </summary>
        FilterDefinition<T> Filter { get; }

        /// <summary>
        /// Gets or sets the set of items from the database.
        /// </summary>
        ObservableCollection<T> LoadedItems { get; set; }

        /// <summary>
        /// Gets or sets the connection to the database.
        /// </summary>
        IMongoClient Client { get; }

        /// <summary>
        /// Gets or sets the items database.
        /// </summary>
        IMongoDatabase ItemsDatabase { get; }

        /// <summary>
        /// Connects to the database and sets up the loaded items from the collection or vice versa.
        /// </summary>
        void ConnectToDatabase();

        /// <summary>
        /// Adds a new item to the database.
        /// </summary>
        /// <param name="newItem">The item to add.</param>
        void AddNewItemToDatabase(T newItem);

        /// <summary>
        /// Updates an existing item in the database.
        /// </summary>
        /// <param name="existingItem">The item to update.</param>
        void UpdateDatabaseItem(T existingItem);
    }
}
