

namespace MongoDbBooks.Models.Database
{
    using System.Collections.Generic;

    using MongoDB.Driver;
    using MongoDB.Bson;
    using System;
    using System.Collections.ObjectModel;
    using Geography;

    public class NationDatabase : BaseDatabaseConnection<Nation>
    {
        /// <summary>
        /// Gets or sets the connection string for the database.
        /// </summary>
        public override string DatabaseConnectionString { get; protected set; }

        /// <summary>
        /// Gets or sets the name of the database.
        /// </summary>
        public override string DatabaseName { get; protected set; }

        /// <summary>
        /// Gets or sets the name of the collection.
        /// </summary>
        public override string CollectionName { get; protected set; }

        /// <summary>
        /// Gets or sets the database collection filter.
        /// </summary>
        public override FilterDefinition<Nation> Filter { get; protected set; }

        /// <summary>
        /// Gets or sets the set of items from the database.
        /// </summary>
        public override ObservableCollection<Nation> LoadedItems { get; set; }

        /// <summary>
        /// Loads the world countries into the nations data base.
        /// </summary>
        /// <param name="worldCountries">The set of simple countries.</param>
        public void UpdateNationsDatabase(ObservableCollection<WorldCountry> worldCountries)
        {
            // Update the loaded then connect to the database.
            LoadedItems.Clear();
            foreach(var country in worldCountries)
            {
                LoadedItems.Add(new Nation()
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
            foreach(var country in countriesData.Countries)
            {
                foreach(var nation in LoadedItems)
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
        public NationDatabase(string dbConnection)
        {
            DatabaseConnectionString = dbConnection;

            LoadedItems = new ObservableCollection<Nation>();

            // this is a dummy query to get everything to west of the dateline...
            Filter = Builders<Nation>.Filter.Lte(
                new StringFieldDefinition<Nation, BsonDouble>("latitude"), new BsonDouble(360.0));

            DatabaseName = "books_read";
            CollectionName = "nations";
        }
    }
}
