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
    using Database;

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
        private string _defaultRecipientName;
        private string _defaultExportDirectory;

        private NationDatabase _nationsDatabase;

        private UserDatabase _usersDatabase;

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
            TalliedMonths = new ObservableCollection<TalliedMonth>();
            BookTags = new ObservableCollection<BookTag>();

            InputFilePath = Properties.Settings.Default.InputFile;
            OutputFilePath = Properties.Settings.Default.OutputFile;
            InputCountriesFilePath = Properties.Settings.Default.InputCountriesFile;
            InputWorldMapFilePath = Properties.Settings.Default.InputWorldMapFile;

            InputFilePath = Properties.Settings.Default.InputFile;
            OutputFilePath = Properties.Settings.Default.OutputFile;
            InputCountriesFilePath = Properties.Settings.Default.InputCountriesFile;
            InputWorldMapFilePath = Properties.Settings.Default.InputWorldMapFile;
            _defaultUserName = Properties.Settings.Default.UserName;
            _defaultRecipientName = Properties.Settings.Default.RecipientName;
            _defaultExportDirectory = Properties.Settings.Default.ExportDirectory;

            string errorMsg;
            ConnectedToDbSuccessfully = ConnectToDatabase(out errorMsg);
            if (!ConnectedToDbSuccessfully)
                _log.Debug("error connecting to db : " + errorMsg);

            _mailReader = new GmailReader();

            _usersDatabase = new UserDatabase(DatabaseConnectionString);

            _nationsDatabase = new NationDatabase(DatabaseConnectionString);

            if (ConnectedToDbSuccessfully)
                _nationsDatabase.UpdateNationsDatabase(WorldCountries);
        }

        #endregion

        #region Connection strings

        public const string DatabaseConnectionString = "mongodb://localhost:27017";

        #endregion

        #region Public Data

        public ObservableCollection<BookRead> BooksRead { get; }

        public ObservableCollection<BookAuthor> AuthorsRead { get; }

        public ObservableCollection<AuthorCountry> AuthorCountries { get; }

        public ObservableCollection<AuthorLanguage> AuthorLanguages { get; }

        public ObservableCollection<TalliedBook> TalliedBooks { get; }

        public ObservableCollection<BooksDelta> BookDeltas { get; }

        public ObservableCollection<BooksDelta> BookPerYearDeltas { get; }

        public ObservableCollection<BookLocationDelta> BookLocationDeltas { get; }

        public ObservableCollection<TalliedMonth> TalliedMonths { get; set; }

        public ObservableCollection<BookTag> BookTags { get; }

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
                if (_defaultUserName != value)
                {
                    _defaultUserName = value;
                    Properties.Settings.Default.UserName = _defaultUserName;
                    Properties.Settings.Default.Save();
                }
            }
        }

        public string DefaultRecipientName
        {
            get
            {
                return _defaultRecipientName;
            }
            set
            {
                if (_defaultRecipientName != value)
                {
                    _defaultRecipientName = value;
                    Properties.Settings.Default.RecipientName = _defaultRecipientName;
                    Properties.Settings.Default.Save();
                }
            }
        }

        public string DefaultExportDirectory
        {
            get
            {
                return _defaultExportDirectory;
            }
            set
            {
                if (_defaultExportDirectory != value)
                {
                    _defaultExportDirectory = value;
                    Properties.Settings.Default.ExportDirectory = _defaultExportDirectory;
                    Properties.Settings.Default.Save();
                }
            }
        }

        public TalliedMonth SelectedMonthTally { get; set; }

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

        public ObservableCollection<Nation> Nations
        {
            get
            {
                if (_nationsDatabase?.LoadedItems != null)
                    return new ObservableCollection<Nation>(_nationsDatabase.LoadedItems);
                return null;
            }
        }

        public NationDatabase NationDatabase => _nationsDatabase;

        public ObservableCollection<User> Users
        {
            get
            {
                if (_usersDatabase?.LoadedItems != null)
                    return new ObservableCollection<User>(_usersDatabase.LoadedItems);
                return null;
            }
        }

        public UserDatabase UserDatabase => _usersDatabase; 

        #endregion

        #region Public Methods

        public void ReadBooksFromFile(string filename)
        {
            ObservableCollection<BookRead> readItems = GetBooksFromFile(filename);

            BooksRead.Clear();
            foreach (var book in readItems)
                BooksRead.Add(book);

            UpdateCollections();
            Properties.Settings.Default.InputFile = filename;
            Properties.Settings.Default.Save();
            DataFromFile = true;
        }

        public void UpdateBooksFromFile(string fileName)
        {
            ObservableCollection<BookRead> readItems = GetBooksFromFile(fileName);

            foreach (var readItem in readItems.OrderBy(x => x.Date))
            {
                BookRead existingReadBook = FindExistingBookRead(readItem);
                string error;
                if (existingReadBook == null)
                {
                    if (!AddNewBook(readItem, out error))
                    {
                        _log.Debug("Add new book failed:" + error + "\n Title: " + readItem.Title +
                            "\n Author " + readItem.Audio +
                            "\n Date: " + readItem.DateString);
                    }
                }
                else
                {
                    MergeBookData(existingReadBook, readItem);
                    if (!UpdateBook(existingReadBook, out error, false))
                    {
                        _log.Debug("Update existing book failed:" + error);
                    }
                }
            }
        }

        public void ReadWorldMapFromFile(string filename)
        {
            using (var sr = new StreamReader(filename, Encoding.Default))
            {
                string asStringXml = sr.ReadToEnd();
                _countriesData = new CountriesData(asStringXml);
            }
            _nationsDatabase.UpdateNationsDatabase(_countriesData);
            UpdateCollections();
            Properties.Settings.Default.InputWorldMapFile = filename;
            Properties.Settings.Default.Save();
        }

        public void WriteBooksToFile(string filename)
        {
            StreamWriter sw = new StreamWriter(filename, false, Encoding.Default); //overwrite original file

            // write the header
            sw.WriteLine(
                "Date,DD/MM/YYYY,Author,Title,Pages,Note,Nationality,Original Language,Book,Comic,Audio,Image"
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
                csv.WriteField(book.ImageUrl);
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

        public bool UpdateBook(BookRead editBook, out string errorMsg, bool updateCollections = true)
        {
            errorMsg = "";

            if (DataFromDb)
                UpdateBookInDatabase(editBook);

            if (updateCollections)
                UpdateCollections();

            return true;
        }

        public void ReadCountriesFromFile(string filename)
        {
            using (StreamReader sr = new StreamReader(filename, Encoding.Default))
            {
                CsvReader csv = new CsvReader(sr);

                WorldCountries.Clear();

                // Country,Capital,Latitude,Longitude,Flag

                while (csv.Read())
                {

                    var stringFieldCountry = csv.GetField<string>(0);
                    var stringFieldCapital = csv.GetField<string>(1);

                    var stringFieldLatitude = csv.GetField<string>(2);
                    var stringFieldLongitude = csv.GetField<string>(3);
                    var stringFieldFlagUrl = csv.GetField<string>(4);

                    double latitude = GetLatLongFromString(stringFieldLatitude, true);
                    double longitude = GetLatLongFromString(stringFieldLongitude, false);

                    if (latitude > -100 & longitude > -190)
                    {
                        WorldCountry country = new WorldCountry()
                        {
                            Country = stringFieldCountry,
                            Capital = stringFieldCapital,
                            Latitude = latitude,
                            Longitude = longitude,
                            FlagUrl = stringFieldFlagUrl != null ? stringFieldFlagUrl : string.Empty
                        };

                        WorldCountries.Add(country);
                    }
                }
            }

            UpdateWorldCountryLookup();
            UpdateCollections();
            UpdateNationalFlags();
            Properties.Settings.Default.InputCountriesFile = filename;
            Properties.Settings.Default.Save();
        }

        public bool ExportFiles(string outputDirectory, bool sendBooksReadFile, bool sendLocationsFile,
            List<string> outputFileNames, out string mailboxErrorMessage)
        {
            mailboxErrorMessage = string.Empty;
            if (sendBooksReadFile)
            {
                try
                {
                    string fileName = GetExportFileName(outputDirectory, "Books");
                    WriteBooksToFile(fileName);

                    outputFileNames.Add(fileName);
                }
                catch (Exception e)
                {
                    mailboxErrorMessage = e.ToString();
                    return false;
                }
            }

            if (sendLocationsFile)
            {
                try
                {
                    //string fileName = GetExportFileName("Locations");

                    // TODO: Integrate the locations writer....

                    //WriteBooksToFile(fileName);

                    //outputFileNames.Add(fileName);
                }
                catch (Exception e)
                {
                    mailboxErrorMessage = e.ToString();
                    return false;
                }
            }

            DefaultExportDirectory = outputDirectory;
            return true;
        }


        public void UpdateCollections()
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
            UpdateBooksPerMonth();
            UpdateBookTags();
        }


        #endregion

        #region Private Methods

        private double GetLatLongFromString(string stringField, bool isLat)
        {
            if (stringField == null || stringField.Length < 7) return -360.0;

            char lastChar = stringField.ToUpper()[stringField.Length - 1];
            bool isPositive;
            switch (lastChar)
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
            if (!UInt16.TryParse(degreesStr, out degrees)) return -360;


            string minsStr = stringField.Substring(degreesStr.Length + 1, 2);
            ushort mins;
            if (!UInt16.TryParse(minsStr, out mins)) return -360;

            double angle = degrees + (mins / 60.0);
            if (!isPositive)
                angle *= -1.0;
            return angle;
        }

        private void UpdateBooksPerMonth()
        {
            // clear the list and the counts
            Dictionary<DateTime, List<BookRead>> bookMonths = new Dictionary<DateTime, List<BookRead>>();
            if (BooksRead.Count < 1) return;
            DateTime startDate = BooksRead[0].Date;
            DateTime endDate = BooksRead.Last().Date;
            endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59);

            DateTime monthStart = new DateTime(startDate.Year, startDate.Month, 1);
            DateTime monthEnd = monthStart.AddMonths(1).AddSeconds(-1);

            // get all the months a book has been read
            while (monthStart <= endDate)
            {
                List<BookRead> monthList = new List<BookRead>();

                foreach (BookRead book in BooksRead)
                {
                    if (book.Date >= monthStart && book.Date <= monthEnd)
                    {
                        monthList.Add(book);
                    }
                }

                if (monthList.Count > 0)
                {
                    bookMonths.Add(monthStart, monthList);
                }

                monthStart = monthStart.AddMonths(1);
                monthEnd = monthStart.AddMonths(1).AddSeconds(-1);
            }

            TalliedMonths.Clear();
            foreach (DateTime date in bookMonths.Keys.OrderBy(x => x))
            {
                TalliedMonths.Add(new TalliedMonth(date, bookMonths[date]));
            }
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
                        new AuthorCountry(this) { Country = author.Nationality };
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

        private void UpdateBookTags()
        {
            BookTags.Clear();

            Dictionary<string, BookTag> bookTagSet = new Dictionary<string, BookTag>();
            foreach (BookRead book in BooksRead)
            {
                if (book.Tags == null || book.Tags.Count == 0)
                    continue;

                foreach (string tag in book.Tags)
                {
                    if (bookTagSet.ContainsKey(tag))
                    {
                        bookTagSet[tag].BooksWithTag.Add(book);
                    }
                    else
                    {
                        BookTag bookTag = new BookTag { Tag = tag };
                        bookTag.BooksWithTag.Add(book);
                        bookTagSet.Add(tag, bookTag);
                    }
                }
            }

            foreach (BookTag bookTag in bookTagSet.Values.ToList())
            {
                BookTags.Add(bookTag);
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
            ObservableCollection<WorldCountry> updateCountries = new ObservableCollection<WorldCountry>();

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
                ObjectId foundId = ObjectId.Empty;
                foreach (var dbCountry in dbCountries)
                {
                    if (dbCountry.Country == currentCountry.Country)
                    {
                        countryInDb = true;
                        foundId = dbCountry.Id;
                    }
                    if (countryInDb)
                        break;
                }

                if (!countryInDb)
                    missingCountries.Add(currentCountry);
                else
                {
                    if (foundId != ObjectId.Empty)
                    {
                        currentCountry.Id = foundId;
                        updateCountries.Add(currentCountry);
                    }
                }
            }

            foreach (var updateCountry in updateCountries)
            {
                var filterOnId = Builders<WorldCountry>.Filter.Eq(s => s.Id, updateCountry.Id);

                long totalCount = worldCountries.Count(filterOnId);

                var result = worldCountries.ReplaceOne(filterOnId, updateCountry);
            }
            //worldCountries.u
            //worldCountries.UpdateMany(updateCountries);

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

            // This is a query to get everything to date. 
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

                using (IAsyncCursor<BookRead> cursor = booksRead.FindSync(filter))
                {
                    existingItems = cursor.ToList();
                }

                // get the missing items
                foreach (BookRead book in BooksRead)
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

            for (int i = 0; i < existingItems.Count; i++)
            {
                var extBook = existingItems[i];

                // if in the duplicate list skip on
                bool inDuplicatedList = false;
                foreach (var dupBook in duplicateBooks)
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
                        int hours = (timeDiff.Days * 24) + timeDiff.Hours;

                        if (Math.Abs(hours) < 48)
                            duplicateBooks.Add(dupBook);
                    }
                }
            }

            foreach (BookRead dupBook in duplicateBooks)
            {
                var remFilter = Builders<BookRead>.Filter.Eq("Id", dupBook.Id);
                var result = booksRead.Find(remFilter);
                var asList = result.ToList();
                booksRead.DeleteMany(remFilter);
            }
        }

        private void UpdateNationalFlags()
        {
            foreach (Nation nation in Nations)
            {
                if (nation.ImageUri != null)
                {
                    continue;
                }

                if (_worldCountryLookup == null || _worldCountryLookup.Count == 0 ||
                    !_worldCountryLookup.ContainsKey(nation.Name))
                {
                    continue;
                }

                WorldCountry country = _worldCountryLookup[nation.Name];
                if (country.FlagUrl == null)
                {
                    continue;
                }

                nation.ImageUri = country.FlagUrl;
                _nationsDatabase.UpdateDatabaseItem(nation);
            }
        }

        private string GetExportFileName(string exportDirectory, string fileName)
        {
            DateTime time = DateTime.Now;
            fileName += " ";
            fileName += time.ToString("yyyy MMMM dd H-mm-ss");
            fileName += ".csv";
            return Path.Combine(exportDirectory, fileName);
        }
        
        private void MergeBookData(BookRead existingReadBook, BookRead readItem)
        {
            if (existingReadBook.Author != readItem.Author)
                existingReadBook.Author = readItem.Author;

            if (existingReadBook.Date != readItem.Date)
                existingReadBook.Date = readItem.Date;

            if (existingReadBook.ImageUrl != readItem.ImageUrl)
                existingReadBook.ImageUrl = readItem.ImageUrl;

            if (existingReadBook.Nationality != readItem.Nationality)
                existingReadBook.Nationality = readItem.Nationality;

            if (existingReadBook.OriginalLanguage != readItem.OriginalLanguage)
                existingReadBook.OriginalLanguage = readItem.OriginalLanguage;

            if (existingReadBook.Format != readItem.Format)
                existingReadBook.Format = readItem.Format;

            if (existingReadBook.Pages != readItem.Pages)
                existingReadBook.Pages = readItem.Pages;

            if (existingReadBook.Title != readItem.Title)
                existingReadBook.Title = readItem.Title;

            if (existingReadBook.Note != readItem.Note &&
                !string.IsNullOrEmpty(readItem.Note))
            {
                if (!string.IsNullOrEmpty(existingReadBook.Note) && 
                    existingReadBook.Note.Length < readItem.Note.Length)
                    existingReadBook.Note = readItem.Note;
                else if (string.IsNullOrEmpty(existingReadBook.Note))
                    existingReadBook.Note = readItem.Note;
            }
        }

        private static ObservableCollection<BookRead> GetBooksFromFile(string filename)
        {
            ObservableCollection<BookRead> readItems = new ObservableCollection<BookRead>();
            using (var sr = new StreamReader(filename, Encoding.Default))
            {
                var csv = new CsvReader(sr);

                readItems.Clear();

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
                    var stringFieldImage = csv.GetField<string>(11);

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
                            ImageUrl = stringFieldImage
                        };

                        readItems.Add(book);
                    }
                }
            }

            return readItems;
        }

        private BookRead FindExistingBookRead(BookRead readItem)
        {
            foreach(var book in BooksRead)
            {
                int matches = 0;

                if (readItem.Title == book.Title) matches++;
                if (readItem.Author == book.Author) matches++;
                if (readItem.Pages == book.Pages) matches++;
                if ((readItem.Date - book.Date).Days < 1) matches++;

                if (matches > 2)
                {
                    return book;
                }
            }

            return null;
        }

        #endregion

        }
    }
