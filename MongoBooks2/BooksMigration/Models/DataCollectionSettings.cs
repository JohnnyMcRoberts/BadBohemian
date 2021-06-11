namespace BooksMigration.Models
{
    using BooksDatabase.Interfaces;

    public class DataCollectionSettings
    {
        /// <summary>
        /// Gets the name of the database.
        /// </summary>
        public string DatabaseName { get; set; }

        /// <summary>
        /// Gets the name of the collection.
        /// </summary>
        public string CollectionName { get; set; }

        /// <summary>
        /// Gets if have read items database.
        /// </summary>
        public bool ReadFromDatabase { get; set; }

        /// <summary>
        /// Gets the number of loaded items read from the collection.
        /// </summary>
        public int LoadedItemsCount { get; set; }

        public DataCollectionSettings()
        {
            DatabaseName = "Unknown";
            CollectionName = "Unknown";
            LoadedItemsCount = 0;
            ReadFromDatabase = false;
        }

        public DataCollectionSettings(IDatabaseCollection collection)
        {
            DatabaseName = collection.DatabaseName;
            CollectionName = collection.CollectionName;
            LoadedItemsCount = collection.LoadedItemsCount;
            ReadFromDatabase = collection.ReadFromDatabase;
        }
    }
}
