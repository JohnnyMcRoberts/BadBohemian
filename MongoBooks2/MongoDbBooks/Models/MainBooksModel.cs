namespace MongoDbBooks.Models
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.IO;
    using System.Globalization;

    using CsvHelper;

    using MongoDB.Bson;
    using MongoDB.Driver;

    using MongoDbBooks.Models.Geography;
    using MongoDbBooks.Models.Mailbox;

    public class MainBooksModel
    {
        #region Private Data

        private readonly log4net.ILog _log;

        private static IMongoClient _client;
        private static IMongoDatabase _booksDatabase;
        private static IMongoDatabase _countriesDatabase;

        private CountriesData _countriesData;
        private Dictionary<string, WorldCountry> _worldCountryLookup;

        private static IMailReader _mailReader;
        private string _defaultUserName;

        #endregion

        #region Constructor

        public MainBooksModel(log4net.ILog log)
        {
            _log = log;

            BooksRead = new ObservableCollection<BookRead>();
            AuthorsRead = new ObservableCollection<BookAuthor>();
            AuthorCountries = new ObservableCollection<AuthorCountry>();
            AuthorLanguages = new ObservableCollection<AuthorLanguage>();
            TalliedBooks = new ObservableCollection<TalliedBook>();
            BookDeltas = new ObservableCollection<BooksDelta>();
            BookPerYearDeltas = new ObservableCollection<BooksDelta>();
            WorldCountries = new ObservableCollection<WorldCountry>();
            BookLocationDeltas = new ObservableCollection<BookLocationDelta>();

            InputFilePath = Properties.Settings.Default.InputFile;
            OutputFilePath = Properties.Settings.Default.OutputFile;
            InputCountriesFilePath = Properties.Settings.Default.InputCountriesFile;
            InputWorldMapFilePath = Properties.Settings.Default.InputWorldMapFile;

            InputFilePath = Properties.Settings.Default.InputFile;
            OutputFilePath = Properties.Settings.Default.OutputFile;
            InputCountriesFilePath = Properties.Settings.Default.InputCountriesFile;
            InputWorldMapFilePath = Properties.Settings.Default.InputWorldMapFile;
            _defaultUserName = Properties.Settings.Default.UserName;

            string errorMsg;
            ConnectedToDbSuccessfully = ConnectToDatabase(out errorMsg);
            if (!ConnectedToDbSuccessfully)
                _log.Debug("error connecting to db : " + errorMsg);

            _mailReader = new GmailReader();
        }

        #endregion

        #region Connection strings

        public const string DatabaseConnectionString = "mongodb://localhost:27017";

        #endregion

        #region Public Data

        public ObservableCollection<BookRead> BooksRead { get; private set; }

        public ObservableCollection<BookAuthor> AuthorsRead { get; private set; }

        public ObservableCollection<AuthorCountry> AuthorCountries { get; private set; }

        public ObservableCollection<AuthorLanguage> AuthorLanguages { get; private set; }

        public ObservableCollection<TalliedBook> TalliedBooks { get; private set; }

        public ObservableCollection<BooksDelta> BookDeltas { get; private set; }

        public ObservableCollection<BooksDelta> BookPerYearDeltas { get; private set; }

        public ObservableCollection<BookLocationDelta> BookLocationDeltas { get; private set; }

        public string InputFilePath { get; set; }
        public string OutputFilePath { get; set; }
        public string InputCountriesFilePath { get; set; }
        public string InputWorldMapFilePath { get; set; }

        public string DefaultUserName
        {
            get
            {
                return _defaultUserName;
            }
            set
            {
                _defaultUserName = value;
                Properties.Settings.Default.Save();
            }
        }

        public bool DataFromFile { get; set; }
        public bool DataFromDb { get; set; }

        public bool ConnectedToDbSuccessfully { get; private set; }

        public ObservableCollection<WorldCountry> WorldCountries { get; set; }

        public ObservableCollection<CountryGeography> CountryGeographies
        {
            get
            {
                if (CountriesData != null && CountriesData.Countries != null)
                    return new ObservableCollection<CountryGeography>(CountriesData.Countries);
                return null;
            }
        }

        public CountriesData CountriesData => _countriesData;

        public IMailReader MailReader => _mailReader;

        #endregion

        #region Public Methods

        public void ReadBooksFromFile(string filename)
        {

            using (var sr = new StreamReader(filename, Encoding.Default))
            {
                var csv = new CsvReader(sr);

                BooksRead.Clear();

                // Date,DD/MM/YYYY,Author,Title,Pages,Note,Nationality,Original Language,Book,Comic,Audio
                while (csv.Read())
                {
                    var stringFieldDate = csv.GetField<string>(0);
                    var stringFieldDDMMYYYY = csv.GetField<string>(1);
                    var stringFieldAuthor = csv.GetField<string>(2);
                    var stringFieldTitle = csv.GetField<string>(3);
                    var stringFieldPages = csv.GetField<string>(4);
                    var stringFieldNote = csv.GetField<string>(5);
                    var stringFieldNationality = csv.GetField<string>(6);
                    var stringFieldOriginalLanguage = csv.GetField<string>(7);
                    var stringFieldBook = csv.GetField<string>(8);
                    var stringFieldComic = csv.GetField<string>(9);
                    var stringFieldAudio = csv.GetField<string>(10);

                    DateTime dateForBook;
                    if (DateTime.TryParseExact(stringFieldDDMMYYYY, "d/M/yyyy",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out dateForBook))
                    {
                        UInt16 pages;
                        UInt16.TryParse(stringFieldPages, out pages);
                        BookRead book = new BookRead()
                        {
                            DateString = stringFieldDate,
                            Date = dateForBook,
                            Author = stringFieldAuthor,
                            Title = stringFieldTitle,
                            Pages = pages,
                            Note = stringFieldNote,
                            Nationality = stringFieldNationality,
                            OriginalLanguage = stringFieldOriginalLanguage,
                            Audio = stringFieldAudio,
                            Book = stringFieldBook,
                            Comic = stringFieldComic,
                        };

                        BooksRead.Add(book);
                    }
                }
            }
            UpdateCollections();
            Properties.Settings.Default.InputFile = filename;
            Properties.Settings.Default.Save();
            DataFromFile = true;
        }

        public void ReadWorldMapFromFile(string filename)
        {
            using (var sr = new StreamReader(filename, Encoding.Default))
            {
                string asStringXml = sr.ReadToEnd();
                _countriesData = new CountriesData(asStringXml);
            }
            UpdateCollections();
            Properties.Settings.Default.InputWorldMapFile = filename;
            Properties.Settings.Default.Save();
        }

        public void WriteBooksToFile(string filename)
        {
            StreamWriter sw = new StreamWriter(filename, false, Encoding.Default); //overwrite original file

            // write the header
            sw.WriteLine(
                "Date,DD/MM/YYYY,Author,Title,Pages,Note,Nationality,Original Language,Book,Comic,Audio"
                );

            // write the records
            var csv = new CsvWriter(sw);
            foreach (var book in BooksRead)
            {
                csv.WriteField(book.DateString);
                csv.WriteField(book.Date.ToString("d/M/yyyy"));
                csv.WriteField(book.Author);
                csv.WriteField(book.Title);
                csv.WriteField(book.Pages > 0 ? book.Pages.ToString() : "");
                csv.WriteField(book.Note);
                csv.WriteField(book.Nationality);
                csv.WriteField(book.OriginalLanguage);
                csv.WriteField(book.Format == BookFormat.Book ? "x" : "");
                csv.WriteField(book.Format == BookFormat.Comic ? "x" : "");
                csv.WriteField(book.Format == BookFormat.Audio ? "x" : "");
                csv.NextRecord();
            }

            // tidy up
            sw.Close();

            //update the settings
            Properties.Settings.Default.OutputFile = filename;
            Properties.Settings.Default.Save();
        }

        public bool ConnectToDatabase(out string errorMsg)
        {
            errorMsg = "";
            try
            {
                _client = new MongoClient(DatabaseConnectionString);
                ConnectToBooksDatabase();
                ConnectToCountriesDatabase();
            }
            catch (Exception e)
            {
                errorMsg = e.ToString();
                return false;
            }

            return true;
        }

        public bool AddNewBook(BookRead newBook, out string errorMsg)
        {
            errorMsg = "";

            // for the moment insist only that the date is after the last of the existing items
            if (BooksRead.Last().Date > newBook.Date)
            {
                errorMsg = "Date must be after last date : " + BooksRead.Last().DateString;
                return false;
            }

            BooksRead.Add(newBook);

            if (DataFromDb)
                AddNewBookToDatabase(newBook);

            UpdateCollections();

            return true;
        }

        public bool UpdateBook(BookRead editBook, out string errorMsg)
        {
            errorMsg = "";

            if (DataFromDb)
                UpdateBookInDatabase(editBook);

            UpdateCollections();

            return true;
        }

        public void ReadCountriesFromFile(string filename)
        {

            using (var sr = new StreamReader(filename, Encoding.Default))
            {
                var csv = new CsvReader(sr);

                WorldCountries.Clear();

                // Country,Capital,Latitude,Longitude

                while (csv.Read())
                {
                    
                    var stringFieldCountry = csv.GetField<string>(0);                    
                    var stringFieldCapital = csv.GetField<string>(1);

                    var stringFieldLatitude = csv.GetField<string>(2);
                    var stringFieldLongitude = csv.GetField<string>(3);
                    
                    double latitude = GetLatLongFromString(stringFieldLatitude, true);
                    double longitude = GetLatLongFromString(stringFieldLongitude, false);

                    if (latitude > -100 & longitude > -190)
                    {
                        WorldCountry country = new WorldCountry()
                        {
                            Country = stringFieldCountry,
                            Capital = stringFieldCapital,
                            Latitude = latitude,
                            Longitude = longitude
                        };

                        WorldCountries.Add(country);
                    }
                }
            }

            UpdateWorldCountryLookup();
            UpdateCollections();
            Properties.Settings.Default.InputCountriesFile = filename;
            Properties.Settings.Default.Save();
        }

        #endregion

        #region Private Methods

        private double GetLatLongFromString(string stringField, bool isLat)
        {
            if (stringField == null || stringField.Length < 7) return -360.0;

            char lastChar = stringField.ToUpper()[stringField.Length - 1];
            bool isPositive;
            switch(lastChar)
            {
                case 'E':
                    if (isLat) return -360.0;
                    isPositive = true;
                    break;
                case 'W':
                    if (isLat) return -360.0;
                    isPositive = false;
                    break;
                case 'N':
                    if (!isLat) return -360.0;
                    isPositive = true;
                    break;
                case 'S':
                    if (!isLat) return -360.0;
                    isPositive = false;
                    break;
                default:
                    return -360.0;
            }

            string degreesStr = stringField.Substring(0, stringField.Length - 5);
            ushort degrees;
            if(!UInt16.TryParse(degreesStr, out degrees)) return -360;


            string minsStr = stringField.Substring(degreesStr.Length + 1, 2);
            ushort mins;
            if (!UInt16.TryParse(minsStr, out mins)) return -360;

            double angle = degrees + (mins / 60.0);
            if (!isPositive)
                angle *= -1.0;
            return angle;
        }

        private void UpdateCollections()
        {
            UpdateAuthors();
            int booksReadWorldwide;
            UInt32 pagesReadWorldwide;
            UpdateCountries(out booksReadWorldwide, out pagesReadWorldwide);
            UpdateLanguages(booksReadWorldwide, pagesReadWorldwide);
            UpdateTalliedBooks();
            UpdateBookDeltas();
            UpdateBookPerYearDeltas();
            UpdateWorldCountryLookup();
            UpdateBookLocationDeltas();
        }

        private void UpdateBookLocationDeltas()
        {
            // clear the list and the counts
            BookLocationDeltas.Clear();
            if (BooksRead.Count < 1) return;
            DateTime startDate = BooksRead[0].Date;

            // get all the dates a book has been read (after the first quarter)
            Dictionary<DateTime, DateTime> bookReadDates = GetBookReadDates(startDate);

            // then add the delta made up of the books up to that date
            foreach (var date in bookReadDates.Keys.ToList())
            {
                BookLocationDelta delta = new BookLocationDelta(date, startDate);
                foreach (var book in BooksRead)
                {
                    if (book.Date <= date)
                    {
                        WorldCountry country = GetCountryForBook(book);
                        if (country != null)
                        {
                            BookLocation location = 
                                new BookLocation() { Book = book, Latitude = country.Latitude, Longitude = country.Longitude };

                            delta.BooksLocationsToDate.Add(location);
                        }
                    }
                    else
                        break;
                }
                //delta.UpdateTallies();
                BookLocationDeltas.Add(delta);
            }
        }

        private WorldCountry GetCountryForBook(BookRead book)
        {
            if (_worldCountryLookup == null || _worldCountryLookup.Count == 0 || 
                !_worldCountryLookup.ContainsKey(book.Nationality)) return null;
            return _worldCountryLookup[book.Nationality];
        }

        private void UpdateWorldCountryLookup()
        {
            _worldCountryLookup = new Dictionary<string, WorldCountry>();
            foreach (var country in WorldCountries)
                if (!_worldCountryLookup.ContainsKey(country.Country))
                    _worldCountryLookup.Add(country.Country, country);
        }

        private void UpdateBookPerYearDeltas()
        {
            // clear the list and the counts
            BookPerYearDeltas.Clear();
            if (BooksRead.Count < 1) return;
            DateTime startDate = BooksRead[0].Date;
            startDate = startDate.AddYears(1);

            // get all the dates a book has been read that are at least a year since the start
            Dictionary<DateTime, DateTime> bookReadDates = new Dictionary<DateTime, DateTime>();
            foreach (var book in BooksRead)
            {
                if (startDate > book.Date) continue;
                if (!bookReadDates.ContainsKey(book.Date))
                {
                    bookReadDates.Add(book.Date, book.Date);
                }
            }

            // then add the delta made up of the books up to that date
            foreach (var date in bookReadDates.Keys.ToList())
            {
                DateTime startYearDate = date;
                startYearDate = startYearDate.AddYears(-1);
                BooksDelta delta = new BooksDelta(date, startYearDate);
                foreach (var book in BooksRead)
                {
                    if (book.Date < startYearDate)
                        continue;

                    if (book.Date <= date)
                        delta.BooksReadToDate.Add(book);
                    else
                        break;
                }
                delta.UpdateTallies();
                BookPerYearDeltas.Add(delta);
            }
        }

        private void UpdateBookDeltas()
        {
            // clear the list and the counts
            BookDeltas.Clear();
            if (BooksRead.Count < 1) return;
            DateTime startDate = BooksRead[0].Date;

            // get all the dates a book has been read (after the first quarter)
            Dictionary<DateTime, DateTime> bookReadDates = GetBookReadDates(startDate);

            // then add the delta made up of the books up to that date
            foreach (var date in bookReadDates.Keys.ToList())
            {
                BooksDelta delta = new BooksDelta(date, startDate);
                foreach (var book in BooksRead)
                {
                    if (book.Date <= date)
                        delta.BooksReadToDate.Add(book);
                    else
                        break;
                }
                delta.UpdateTallies();
                BookDeltas.Add(delta);
            }
        }

        private Dictionary<DateTime, DateTime> GetBookReadDates(DateTime startDate)
        {
            Dictionary<DateTime, DateTime> bookReadDates = new Dictionary<DateTime, DateTime>();
            foreach (var book in BooksRead)
            {
                if (!bookReadDates.ContainsKey(book.Date))
                {
                    TimeSpan ts = book.Date - startDate;
                    if (ts.Days >= 90)
                        bookReadDates.Add(book.Date, book.Date);
                }
            }
            return bookReadDates;
        }

        private void UpdateTalliedBooks()
        {
            // clear the list and the counts
            TalliedBooks.Clear();
            UInt32 totalBooks = 0;
            UInt32 totalPagesRead = 0;
            UInt32 totalBookFormat = 0;
            UInt32 totalComicFormat = 0;
            UInt32 totalAudioFormat = 0;

            // The assumption is the books arrive in order (as they do)
            List<TalliedBook> booksTally = new List<TalliedBook>();
            foreach (var book in BooksRead)
            {
                totalBooks++;
                totalPagesRead += book.Pages;
                if (book.Format == BookFormat.Book) totalBookFormat++;
                if (book.Format == BookFormat.Comic) totalComicFormat++;
                if (book.Format == BookFormat.Audio) totalAudioFormat++;

                TalliedBook tally = new TalliedBook(book)
                {
                    TotalBooks = totalBooks,
                    TotalAudioFormat = totalAudioFormat,
                    TotalBookFormat = totalBookFormat,
                    TotalComicFormat = totalComicFormat,
                    TotalPagesRead = totalPagesRead
                };

                booksTally.Add(tally);
            }

            // finally need to sort them into datae descending
            var sortedTallies =
                (from item in booksTally orderby item.TotalBooks descending select item);
            foreach (var tallied in sortedTallies)
                TalliedBooks.Add(tallied);
        }

        private void UpdateLanguages(int booksReadWorldwide, uint pagesReadWorldwide)
        {
            // clear the list
            AuthorLanguages.Clear();

            // get the uniquely named languages
            Dictionary<string, AuthorLanguage> languageSet = new Dictionary<string, AuthorLanguage>();
            foreach (var author in AuthorsRead)
            {
                if (languageSet.ContainsKey(author.Language))
                    languageSet[author.Language].AuthorsInLanguage.Add(author);
                else
                {
                    AuthorLanguage language =
                        new AuthorLanguage() { Language = author.Language };
                    language.AuthorsInLanguage.Add(author);
                    languageSet.Add(language.Language, language);
                }
            }

            // Update the language totals + add to the list
            foreach (var language in languageSet.Values.ToList())
            {
                language.TotalBooksWorldWide = booksReadWorldwide;
                language.TotalPagesWorldWide = pagesReadWorldwide;
                AuthorLanguages.Add(language);
            }
        }

        private void UpdateCountries(out int booksReadWorldwide, out UInt32 pagesReadWorldwide)
        {
            // clear the list & counts
            AuthorCountries.Clear();
            booksReadWorldwide = 0;
            pagesReadWorldwide = 0;

            // get the uniquely named countries + the counts
            Dictionary<string, AuthorCountry> countrySet = new Dictionary<string, AuthorCountry>();
            foreach (var author in AuthorsRead)
            {
                booksReadWorldwide += author.TotalBooksReadBy;
                pagesReadWorldwide += author.TotalPages;

                if (countrySet.ContainsKey(author.Nationality))
                    countrySet[author.Nationality].AuthorsFromCountry.Add(author);
                else
                {
                    AuthorCountry country =
                        new AuthorCountry() { Country = author.Nationality };
                    country.AuthorsFromCountry.Add(author);
                    countrySet.Add(country.Country, country);
                }
            }

            // Update the country totals + add to the list
            foreach (var country in countrySet.Values.ToList())
            {
                country.TotalBooksWorldWide = booksReadWorldwide;
                country.TotalPagesWorldWide = pagesReadWorldwide;
                AuthorCountries.Add(country);
            }
        }

        private void UpdateAuthors()
        {
            AuthorsRead.Clear();

            Dictionary<string, BookAuthor> authorsSet = new Dictionary<string, BookAuthor>();
            foreach (var book in BooksRead)
            {
                if (authorsSet.ContainsKey(book.Author))
                    authorsSet[book.Author].BooksReadBy.Add(book);
                else
                {
                    BookAuthor author =
                        new BookAuthor() { Author = book.Author, Language = book.OriginalLanguage, Nationality = book.Nationality };
                    author.BooksReadBy.Add(book);
                    authorsSet.Add(book.Author, author);
                }
            }
            foreach (var author in authorsSet.Values.ToList())
                AuthorsRead.Add(author);
        }

        private void AddNewBookToDatabase(BookRead newBook)
        {
            _client = new MongoClient(DatabaseConnectionString);
            _booksDatabase = _client.GetDatabase("books_read");

            IMongoCollection<BookRead> booksRead = _booksDatabase.GetCollection<BookRead>("books");

            booksRead.InsertOne(newBook);
        }

        private void UpdateBookInDatabase(BookRead editBook)
        {
            _client = new MongoClient(DatabaseConnectionString);
            _booksDatabase = _client.GetDatabase("books_read");

            IMongoCollection<BookRead> booksRead = _booksDatabase.GetCollection<BookRead>("books");

            var filterOnId = Builders<BookRead>.Filter.Eq(s => s.Id, editBook.Id);

            long totalCount = booksRead.Count(filterOnId);

            var result = booksRead.ReplaceOne(filterOnId, editBook);

        }

        private void ConnectToCountriesDatabase()
        {
            _countriesDatabase = _client.GetDatabase("world_countries");

            IMongoCollection<WorldCountry> worldCountries =
                _countriesDatabase.GetCollection<WorldCountry>("countries");

            // this is a dummy query to get everything to west of the dateline...
            var filter = Builders<WorldCountry>.Filter.Lte(
                new StringFieldDefinition<WorldCountry, BsonDouble>("latitude"), new BsonDouble(360.0));

            long totalCount = worldCountries.Count(filter);

            if (totalCount == 0 && WorldCountries.Count != 0)
            {
                AddLoadedCountriesToBlankDatabase(worldCountries, filter);
            }
            else if (WorldCountries.Count != 0)
            {
                AddNewCountriesToExistingDatabase(worldCountries, filter);
            }
            else if (totalCount != 0 && WorldCountries.Count == 0)
            {
                LoadAllCountriesFromDatabase(worldCountries, filter);
            }
            else if (totalCount != 0 && totalCount < WorldCountries.Count)
            {
                UpdateDatabaseCountries(worldCountries, filter);
            }

            if (WorldCountries.Count > 0)
            {
                List<WorldCountry> orderedCountries = WorldCountries.OrderBy(c => c.Country).ToList();
                WorldCountries.Clear();
                foreach (var orderedCountry in orderedCountries)
                    WorldCountries.Add(orderedCountry);
            }

            UpdateWorldCountryLookup();

        }

        private void UpdateDatabaseCountries(IMongoCollection<WorldCountry> worldCountries, FilterDefinition<WorldCountry> filter)
        {
            List<WorldCountry> missingItems = new List<WorldCountry>();
            List<WorldCountry> existingItems;

            using (var cursor = worldCountries.FindSync(filter))
            {
                existingItems = cursor.ToList();
            }

            // get the missing items
            foreach (var country in WorldCountries)
            {
                bool alreadyThere = false;
                foreach (var existing in existingItems)
                {
                    if (country.Country == existing.Country)
                    {
                        alreadyThere = true;
                        break;
                    }
                }
                if (!alreadyThere)
                    missingItems.Add(country);
            }

            // then insert them to the list

            worldCountries.InsertMany(missingItems);
            worldCountries.Count(filter);
        }

        private void LoadAllCountriesFromDatabase(IMongoCollection<WorldCountry> worldCountries, 
            FilterDefinition<WorldCountry> filter)
        {
            WorldCountries.Clear();

            using (var cursor = worldCountries.FindSync(filter))
            {
                var countryList = cursor.ToList();
                foreach (var country in countryList)
                {
                    WorldCountries.Add(country);
                }
            }
            UpdateCollections();
            DataFromFile = false;
            DataFromDb = true;
        }

        private void AddNewCountriesToExistingDatabase(IMongoCollection<WorldCountry> worldCountries, FilterDefinition<WorldCountry> filter)
        {
            ObservableCollection<WorldCountry> dbCountries = new ObservableCollection<WorldCountry>();
            ObservableCollection<WorldCountry> missingCountries = new ObservableCollection<WorldCountry>();

            using (IAsyncCursor<WorldCountry> cursor = worldCountries.FindSync(filter))
            {
                List<WorldCountry> countryList = cursor.ToList();
                foreach (var country in countryList)
                {
                    dbCountries.Add(country);
                }
            }

            foreach (var currentCountry in WorldCountries)
            {
                bool countryInDb = false;
                foreach (var dbCountry in dbCountries)
                {
                    if (dbCountry.Country == currentCountry.Country)
                        countryInDb = true;
                    if (countryInDb)
                        break;
                }

                if (!countryInDb)
                    missingCountries.Add(currentCountry);
            }

            worldCountries.InsertMany(missingCountries);
            worldCountries.Count(filter);
        }

        private long AddLoadedCountriesToBlankDatabase(IMongoCollection<WorldCountry> worldCountries, 
            FilterDefinition<WorldCountry> filter)
        {
            worldCountries.InsertMany(WorldCountries);
            return worldCountries.Count(filter);
        }

        private void ConnectToBooksDatabase()
        {
            _booksDatabase = _client.GetDatabase("books_read");

            IMongoCollection<BookRead> booksRead = _booksDatabase.GetCollection<BookRead>("books");

            // this is a dummy query to get everything to date...

            var filter = Builders<BookRead>.Filter.Lte(
                new StringFieldDefinition<BookRead, BsonDateTime>("date"), new BsonDateTime(DateTime.Now.AddDays(1000)));

            long totalCount = booksRead.Count(filter);

            if (totalCount == 0 && BooksRead.Count != 0)
            {
                booksRead.InsertMany(BooksRead);
                totalCount = booksRead.Count(filter);

            }
            else if (totalCount != 0 && BooksRead.Count == 0)
            {
                RemoveDuplicateBooksRead(booksRead, filter);

                BooksRead.Clear();

                using (var cursor = booksRead.FindSync(filter))
                {
                    var booksList = cursor.ToList();

                    var sortedBooks =
                        (from item in booksList orderby item.Date select item);

                    foreach (var book in sortedBooks)
                    {
                        BooksRead.Add(book);
                    }
                }

                UpdateCollections();
                DataFromFile = false;
                DataFromDb = true;
            }
            else if (totalCount != 0 && totalCount != BooksRead.Count)
            {
                List<BookRead> missingItems = new List<BookRead>();
                List<BookRead> existingItems = new List<BookRead>();

                using (var cursor = booksRead.FindSync(filter))
                {
                    existingItems = cursor.ToList();
                }

                // get the missing items
                foreach (var book in BooksRead)
                {
                    bool alreadyThere = false;
                    foreach (var existing in existingItems)
                    {
                        if (book.Title == existing.Title &&
                            book.Author == existing.Author &&
                            Math.Abs((book.Date - existing.Date).Hours) < 48)
                        {
                            alreadyThere = true;
                            break;
                        }
                    }
                    if (!alreadyThere)
                        missingItems.Add(book);
                }

                // then insert them to the list

                booksRead.InsertMany(missingItems);
                totalCount = booksRead.Count(filter);

                UpdateCollections();
                DataFromFile = false;
                DataFromDb = true;
            }

        }

        private void RemoveDuplicateBooksRead(IMongoCollection<BookRead> booksRead, FilterDefinition<BookRead> filter)
        {
            List<BookRead> existingItems = new List<BookRead>();

            using (var cursor = booksRead.FindSync(filter))
            {
                existingItems = cursor.ToList();
            }

            List<BookRead> duplicateBooks = new List<BookRead>();

            for(int i = 0; i < existingItems.Count; i++)
            {
                var extBook = existingItems[i];

                // if in the duplicate list skip on
                bool inDuplicatedList = false;
                foreach(var dupBook in duplicateBooks)
                {
                    if (dupBook.Author == extBook.Author &&
                        dupBook.Title == extBook.Title &&
                        dupBook.Format == extBook.Format &&
                        dupBook.Pages == extBook.Pages)
                    {
                        inDuplicatedList = true;
                        break;
                    }
                }
                if (inDuplicatedList)
                    continue;

                // see if duplicates with different dates
                for (int j = 0; j < existingItems.Count; j++)
                {
                    // ignore self
                    if (j == i) 
                        continue;

                    var dupBook = existingItems[j];

                    if (dupBook.Author == extBook.Author &&
                        dupBook.Title == extBook.Title &&
                        dupBook.Format == extBook.Format &&
                        dupBook.Pages == extBook.Pages)
                    {
                        var timeDiff = dupBook.Date - extBook.Date;
                        int hours = (timeDiff.Days * 24) +  timeDiff.Hours;

                        if (Math.Abs(hours) < 48)
                            duplicateBooks.Add(dupBook);
                    }
                }
            }

            foreach (var dupBook in duplicateBooks)
            {
                var remFilter = Builders<BookRead>.Filter.Eq("Id", dupBook.Id);
                var result = booksRead.Find(remFilter);
                var asList = result.ToList();
                booksRead.DeleteMany(remFilter);
            }
        }

        #endregion
    }
}
