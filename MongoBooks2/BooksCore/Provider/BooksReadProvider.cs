
namespace BooksCore.Provider
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Collections.ObjectModel;

    using BooksCore.Books;
    using BooksCore.Geography;
    using BooksCore.Interfaces;

    public class BooksReadProvider : IBooksReadProvider
    {
        /// <summary>
        /// The world countries lookup.
        /// </summary>
        private Dictionary<string, WorldCountry> _worldCountryLookup;

        /// <summary>
        /// The geography provider.
        /// </summary>
        private IGeographyProvider _geographyProvider;

        /// <summary>
        /// The selected month.
        /// </summary>
        private DateTime _selectedMonth;

        /// <summary>
        /// Gets the books.
        /// </summary>
        public ObservableCollection<BookRead> BooksRead { get; private set; }

        /// <summary>
        /// Gets the changes between books.
        /// </summary>
        public ObservableCollection<BooksDelta> BookDeltas { get; }

        /// <summary>
        /// Gets the changes per year between books.
        /// </summary>
        public ObservableCollection<BooksDelta> BookPerYearDeltas { get; }

        /// <summary>
        /// Gets the authors.
        /// </summary>
        public ObservableCollection<BookAuthor> AuthorsRead { get; }

        /// <summary>
        /// Gets the book location deltas.
        /// </summary>
        public ObservableCollection<BookLocationDelta> BookLocationDeltas { get; private set; }

        /// <summary>
        /// Gets the author countries.
        /// </summary>
        public ObservableCollection<AuthorCountry> AuthorCountries { get; }

        /// <summary>
        /// Gets the author languages.
        /// </summary>
        public ObservableCollection<AuthorLanguage> AuthorLanguages { get; }

        /// <summary>
        /// Gets the book tallies.
        /// </summary>
        public ObservableCollection<TalliedBook> TalliedBooks { get; }

        /// <summary>
        /// Gets the tags added to the books.
        /// </summary>
        public ObservableCollection<BookTag> BookTags { get; }

        /// <summary>
        /// Gets the selected month tally.
        /// </summary>
        public ObservableCollection<TalliedMonth> TalliedMonths { get; private set; }

        /// <summary>
        /// Gets the current month and overall tallies for the reports.
        /// </summary>
        public ObservableCollection<MonthlyReportsTally> ReportsTallies { get; private set; }

        /// <summary>
        /// Gets or sets the selected month tally.
        /// </summary>
        public DateTime SelectedMonth
        {
            get
            {
                return _selectedMonth;
            }

            set
            {
                if (value.Year != _selectedMonth.Year || value.Month != _selectedMonth.Month)
                {
                    _selectedMonth = value;
                    foreach (TalliedMonth talliedMonth in TalliedMonths)
                    {
                        if (talliedMonth.MonthDate.Year == _selectedMonth.Year &&
                            talliedMonth.MonthDate.Month == _selectedMonth.Month)
                        {
                            // Set the selected month and report tallies.
                            SelectedMonthTally = talliedMonth;
                            ReportsTallies =
                                new ObservableCollection<MonthlyReportsTally>
                                {
                                    new MonthlyReportsTally(SelectedMonthTally),
                                    new MonthlyReportsTally(BookDeltas.Last().OverallTally)
                                };

                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the selected month tally.
        /// </summary>
        public TalliedMonth SelectedMonthTally { get; private set; }

        private void UpdateCountries(out int booksReadWorldwide, out uint pagesReadWorldwide)
        {
            // clear the list & counts
            AuthorCountries.Clear();
            booksReadWorldwide = 0;
            pagesReadWorldwide = 0;

            // get the uniquely named countries + the counts
            Dictionary<string, AuthorCountry> countrySet = new Dictionary<string, AuthorCountry>();
            foreach (BookAuthor author in AuthorsRead)
            {
                booksReadWorldwide += author.TotalBooksReadBy;
                pagesReadWorldwide += author.TotalPages;

                if (countrySet.ContainsKey(author.Nationality))
                    countrySet[author.Nationality].AuthorsFromCountry.Add(author);
                else
                {
                    AuthorCountry country = new AuthorCountry(_geographyProvider) { Country = author.Nationality };
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

        private void UpdateBookLocationDeltas()
        {
            // clear the list and the counts
            BookLocationDeltas.Clear();
            if (BooksRead.Count < 1) return;
            DateTime startDate = BooksRead[0].Date;

            // get all the dates a book has been read (after the first quarter)
            Dictionary<DateTime, DateTime> bookReadDates = GetBookReadDates(startDate);

            // then add the delta made up of the books up to that date
            foreach (DateTime date in bookReadDates.Keys.ToList())
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

        private void UpdateAuthors()
        {
            AuthorsRead.Clear();

            Dictionary<string, BookAuthor> authorsSet = new Dictionary<string, BookAuthor>();
            foreach (BookRead book in BooksRead)
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

        private void UpdateWorldCountryLookup()
        {
            _worldCountryLookup = new Dictionary<string, WorldCountry>();
            foreach (var country in _geographyProvider.WorldCountries)
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

        private void UpdateLanguages(int booksReadWorldwide, uint pagesReadWorldwide)
        {
            // clear the list
            AuthorLanguages.Clear();

            // get the uniquely named languages
            Dictionary<string, AuthorLanguage> languageSet = new Dictionary<string, AuthorLanguage>();
            foreach (BookAuthor author in AuthorsRead)
            {
                if (languageSet.ContainsKey(author.Language))
                    languageSet[author.Language].AuthorsInLanguage.Add(author);
                else
                {
                    AuthorLanguage language = new AuthorLanguage { Language = author.Language };
                    language.AuthorsInLanguage.Add(author);
                    languageSet.Add(language.Language, language);
                }
            }

            // Update the language totals + add to the list
            foreach (AuthorLanguage language in languageSet.Values.ToList())
            {
                language.TotalBooksWorldWide = booksReadWorldwide;
                language.TotalPagesWorldWide = pagesReadWorldwide;
                AuthorLanguages.Add(language);
            }
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
                    string trimmedTag = tag.Trim();
                    if (bookTagSet.ContainsKey(trimmedTag))
                    {
                        bookTagSet[trimmedTag].BooksWithTag.Add(book);
                    }
                    else
                    {
                        BookTag bookTag = new BookTag { Tag = trimmedTag };
                        bookTag.BooksWithTag.Add(book);
                        bookTagSet.Add(trimmedTag, bookTag);
                    }
                }
            }

            foreach (BookTag bookTag in bookTagSet.Values.ToList())
            {
                BookTags.Add(bookTag);
            }
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

            // finally need to sort them into date descending
            var sortedTallies =
                (from item in booksTally orderby item.TotalBooks descending select item);
            foreach (var tallied in sortedTallies)
                TalliedBooks.Add(tallied);
        }

        public void Setup(IList<BookRead> books, IGeographyProvider geographyProvider)
        {
            _geographyProvider = geographyProvider;

            BooksRead.Clear();
            foreach (BookRead book in books.OrderBy(x => x.Date))
                BooksRead.Add(book);

            UpdateBookDeltas();
            UpdateBookPerYearDeltas();
            UpdateAuthors();
            UpdateWorldCountryLookup();
            int booksReadWorldwide;
            uint pagesReadWorldwide;
            UpdateCountries(out booksReadWorldwide, out pagesReadWorldwide);
            UpdateLanguages(booksReadWorldwide, pagesReadWorldwide);
            BookLocationDeltas = new ObservableCollection<BookLocationDelta>();
            UpdateBookLocationDeltas();
            UpdateBooksPerMonth();
            UpdateBookTags();
            UpdateTalliedBooks();
            SelectedMonthTally = TalliedMonths.FirstOrDefault();
            _selectedMonth = DateTime.Now;
            if (SelectedMonthTally != null)
                _selectedMonth = SelectedMonthTally.MonthDate;
        }

        public BooksReadProvider()
        {
            BooksRead = new ObservableCollection<BookRead>();
            BookDeltas = new ObservableCollection<BooksDelta>();
            BookPerYearDeltas = new ObservableCollection<BooksDelta>();
            AuthorsRead = new ObservableCollection<BookAuthor>();
            AuthorLanguages = new ObservableCollection<AuthorLanguage>();
            BookLocationDeltas = new ObservableCollection<BookLocationDelta>();
            AuthorCountries = new ObservableCollection<AuthorCountry>();
            BookTags = new ObservableCollection<BookTag>();
            TalliedBooks = new ObservableCollection<TalliedBook>();
            TalliedMonths = new ObservableCollection<TalliedMonth>();
            ReportsTallies = new ObservableCollection<MonthlyReportsTally>();
        }
    }
}
