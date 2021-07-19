// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NationDatabase.cs" company="N/A">
//   2017-2086
// </copyright>
// <summary>
//   The nation MongoDb database.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace BooksDatabase.Implementations
{
    using System.Collections.ObjectModel;
    using BooksCore.Geography;
    using MongoDB.Bson;
    using MongoDB.Driver;

    /// <summary>
    /// The nations read database.
    /// </summary>
    public class NationDatabase : BaseDatabaseConnection<Nation>
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
        public sealed override FilterDefinition<Nation> Filter { get; protected set; }

        /// <summary>
        /// Gets or sets the set of items from the database.
        /// </summary>
        public sealed override ObservableCollection<Nation> LoadedItems { get; set; }

        /// <summary>
        /// Loads the world countries into the nations data base.
        /// </summary>
        /// <param name="worldCountries">The set of simple countries.</param>
        public void UpdateNationsDatabase(ObservableCollection<WorldCountry> worldCountries)
        {
            // Update the loaded then connect to the database.
            LoadedItems.Clear();
            foreach (WorldCountry country in worldCountries)
            {
                LoadedItems.Add(new Nation
                {
                    Name = country.Country,
                    Capital = country.Capital,
                    Latitude = country.Latitude,
                    Longitude = country.Longitude
                });
            }

            ConnectToDatabase();
        }

        /// <summary>
        /// Updates the nations database with the geographical info.
        /// </summary>
        /// <param name="countriesData">The set of geographical data for the countries.</param>
        public void UpdateNationsDatabase(CountriesData countriesData)
        {
            foreach (CountryGeography country in countriesData.Countries)
            {
                foreach (Nation nation in LoadedItems)
                {
                    if (country.Name == nation.Name)
                    {
                        if (country.XmlElement != null)
                        {
                            if (nation.GeographyXml == null ||
                                (nation.GeographyXml != null && 
                                country.XmlElement.OuterXml.Length != nation.GeographyXml.Length))
                            {
                                nation.GeographyXml = country.XmlElement.OuterXml;
                                UpdateDatabaseItem(nation);
                                break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NationDatabase"/> class.
        /// </summary>
        /// <param name="dbConnection">The connection string for the database.</param>
        /// <param name="connectAtStartup">Whether or not to connect to the database at startup.</param>
        public NationDatabase(string dbConnection, bool connectAtStartup = true)
        {
            DatabaseConnectionString = dbConnection;

            LoadedItems = new ObservableCollection<Nation>();

            // this is a dummy query to get everything to west of the dateline...
            Filter = Builders<Nation>.Filter.Lte(
                new StringFieldDefinition<Nation, BsonDouble>("latitude"), new BsonDouble(360.0));

            DatabaseName = "books_read";
            CollectionName = "nations";

            if (connectAtStartup)
            {
                ConnectToDatabase();
            }
        }
    }
}
