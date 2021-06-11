// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDatabaseCollection.cs" company="N/A">
//   2017-2086
// </copyright>
// <summary>
//   The base database interface for a generic database entity.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksDatabase.Interfaces
{
    /// <summary>
    /// Base interface for a Mongo database connection.
    /// </summary>
    public interface IDatabaseCollection
    {
        /// <summary>
        /// Gets the name of the database.
        /// </summary>
        string DatabaseName { get; }

        /// <summary>
        /// Gets the name of the collection.
        /// </summary>
        string CollectionName { get; }

        /// <summary>
        /// Gets the number of loaded items read from the collection.
        /// </summary>
        int LoadedItemsCount { get; }

        /// <summary>
        /// Gets if have read items database.
        /// </summary>
        bool ReadFromDatabase { get; }

        /// <summary>
        /// Resets the list of items loaded from the collection data.
        /// </summary>
        void ResetLoadedItems();

        /// <summary>
        /// Connects to the database and sets up the loaded items from the collection or vice versa.
        /// </summary>
        void ConnectToDatabase();
    }
}
