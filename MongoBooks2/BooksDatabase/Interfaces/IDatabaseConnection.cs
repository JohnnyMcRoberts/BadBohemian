// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDatabaseCollection.cs" company="N/A">
//   2017-2086
// </copyright>
// <summary>
//   The database interface for a generic database entity.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksDatabase.Interfaces
{
    using System.Collections.ObjectModel;
    using BooksCore.Base;
    using MongoDB.Driver;

    /// <summary>
    /// Interface for a generic database connection.
    /// </summary>
    /// <typeparam name="T">Type of the data entity</typeparam>
    public interface IDatabaseConnection<T> where T : BaseMongoEntity
    {
        /// <summary>
        /// Gets the connection string for the database.
        /// </summary>
        string DatabaseConnectionString { get; }

        /// <summary>
        /// Gets the database collection filter.
        /// </summary>
        /// <typeparam name="T">Type of the data entity</typeparam>
        FilterDefinition<T> Filter { get; }

        /// <summary>
        /// Gets or sets the set of items from the database.
        /// </summary>
        /// <typeparam name="T">Type of the data entity</typeparam>
        ObservableCollection<T> LoadedItems { get; set; }

        /// <summary>
        /// Gets connection to the database.
        /// </summary>
        IMongoClient Client { get; }

        /// <summary>
        /// Gets the items database.
        /// </summary>
        IMongoDatabase ItemsDatabase { get; }

        /// <summary>
        /// Adds a new item to the database.
        /// </summary>
        /// <typeparam name="T">Type of the data entity</typeparam>
        /// <param name="newItem">The item to add.</param>
        void AddNewItemToDatabase(T newItem);

        /// <summary>
        /// Updates an existing item in the database.
        /// </summary>
        /// <typeparam name="T">Type of the data entity</typeparam>
        /// <param name="existingItem">The item to update.</param>
        void UpdateDatabaseItem(T existingItem);
    }
}
