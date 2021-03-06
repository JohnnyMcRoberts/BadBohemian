﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataGridsViewModel.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The data grids view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MongoDbBooks.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Linq;
    using System.Windows.Input;

    using MongoDbBooks.Models;
    using MongoDbBooks.Models.Geography;
    using MongoDbBooks.Models.Database;
    using MongoDbBooks.ViewModels.Utilities;
    using MongoDbBooks.Views;

    public class DataGridsViewModel : BaseViewModel
    {
        #region Private Data

        private MainWindow _mainWindow;
        private log4net.ILog _log;
        private readonly MainBooksModel _mainModel;
        private MainViewModel _parent;

        private readonly List<object> _rawBooksData;

        private DataTable _languageDeltasTable;
        private DataTable _countryDeltasTable;

        /// <summary>
        /// The select image for nation command.
        /// </summary>
        private ICommand _selectImageForNationCommand;

        #endregion

        #region Constants

        private static readonly BookRead _book = new BookRead()
        {
            Audio = "",
            Author = "",
            Book = "x",
            Comic = "",
            Date = DateTime.Now,
            DateString = "",
            Format = BookFormat.Book,
            Nationality = "",
            Note = "",
            OriginalLanguage = "",
            Pages = 0,
            Title = ""
        };

        private static List<KeyValuePair<string, string>> RawDataTitleToPropertyMapping =
            new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string,string>("DateString", "DateString" ),
                new KeyValuePair<string,string>("Date", "Date" ),
                new KeyValuePair<string,string>("Author", "Author" ),
                new KeyValuePair<string,string>("Title", "Title" ),
                new KeyValuePair<string,string>("Pages", "Pages" ),
                new KeyValuePair<string,string>("Nationality", "Nationality" ),
                new KeyValuePair<string,string>("OriginalLanguage", "OriginalLanguage" ),
                new KeyValuePair<string,string>("Book", "Book" ),
                new KeyValuePair<string,string>("Comic", "Comic" ),
                new KeyValuePair<string,string>("Audio", "Audio" ),
            };

        private static readonly List<Tuple<string, string, Type>> RawDataTitleToPropertyMappings =
            new List<Tuple<string, string, Type>>
            {
                                            //  Header      PropertyName    Type       
                new Tuple<string,string, Type>("Date String", "DateString" , _book.DateString.GetType()),
                new Tuple<string,string, Type>("Date", "Date" , _book.Date.GetType() ),
                new Tuple<string,string, Type>("Author", "Author" , _book.Author.GetType() ),
                new Tuple<string,string, Type>("Title", "Title"  , _book.Title.GetType()),
                new Tuple<string,string, Type>("Pages", "Pages" , _book.Pages.GetType() ),
                new Tuple<string,string, Type>("Nationality", "Nationality"  , _book.Nationality.GetType()),
                new Tuple<string,string, Type>("Original Language", "OriginalLanguage"  , _book.OriginalLanguage.GetType()),
                new Tuple<string,string, Type>("Book", "Book"  , _book.Book.GetType()),
                new Tuple<string,string, Type>("Comic", "Comic"  , _book.Comic.GetType()),
                new Tuple<string,string, Type>("Audio", "Audio"  , _book.Audio.GetType()),
                new Tuple<string,string, Type>("Format", "Format"  , _book.Format.GetType()),
            };

        #endregion

        #region Public Data

        public ObservableCollection<BookRead> BooksRead { get { return _mainModel.BooksRead; } }

        public ObservableCollection<BookAuthor> AuthorsRead { get { return _mainModel.AuthorsRead; } }

        public ObservableCollection<AuthorCountry> AuthorCountries { get { return _mainModel.AuthorCountries; } }

        public ObservableCollection<AuthorLanguage> AuthorLanguages { get { return _mainModel.AuthorLanguages; } }

        public ObservableCollection<TalliedBook> TalliedBooks { get { return _mainModel.TalliedBooks; } }

        public ObservableCollection<BooksDelta> BookDeltas { get { return _mainModel.BookDeltas; } }

        public ObservableCollection<BooksDelta> BookPerYearDeltas { get { return _mainModel.BookPerYearDeltas; } }

        public ObservableCollection<BookTag> BookTags { get { return new ObservableCollection<BookTag>(_mainModel.BookTags.OrderBy(x => x.Tag)); } }

        public DataTable RawDataTable
        {
            get
            {
                return BuildSeriesTable(
                                                "RawBooksData",
                                                RawDataTitleToPropertyMappings,
                                                _rawBooksData
                                                );
            }
        }

        public DataTable LanguageDeltasTable { get { return _languageDeltasTable; } }

        public DataTable CountryDeltasTable { get { return _countryDeltasTable; } }

        public ObservableCollection<WorldCountry> WorldCountries { get { return _mainModel.WorldCountries; } }

        public ObservableCollection<CountryGeography> CountryGeographies
        {
            get
            {
                return _mainModel.CountryGeographies;
            }
        }


        public ObservableCollection<Nation> Nations
        {
            get
            {
                return new ObservableCollection<Nation>(_mainModel.Nations.OrderBy(n => n.Name));
            }
        }


        public ObservableCollection<BookLocationDelta> BookLocationDeltas { get { return _mainModel.BookLocationDeltas; } }

        public ObservableCollection<TalliedMonth> TalliedMonths { get { return _mainModel.TalliedMonths; } }

        #endregion

        #region Constructor

        public DataGridsViewModel(
            MainWindow mainWindow, log4net.ILog log,
            MainBooksModel mainModel, MainViewModel parent)
        {
            _mainWindow = mainWindow;
            _log = log;
            _mainModel = mainModel;
            _parent = parent;

            _rawBooksData = new List<object>();
            _languageDeltasTable = new DataTable("LanguageDeltas");
            _countryDeltasTable = new DataTable("CountryeDeltas");
        }

        #endregion

        #region Public Methods

        public void UpdateData()
        {
            _rawBooksData.Clear();

            foreach (var book in _mainModel.BooksRead)
                _rawBooksData.Add(book);

            BuildLanguageDeltasTable();
            BuildCountryDeltasTable();

            OnPropertyChanged("");

        }

        #endregion

        #region Utility Functions

        private void BuildCountryDeltasTable()
        {
            _countryDeltasTable = new DataTable("CountryDeltasTable");
            if (!BookDeltas.Any()) return;

            // get all the languages from the last delta item in order
            BooksDelta.DeltaTally latestTally = BookDeltas.Last().OverallTally;
            List<string> countries = (from item in latestTally.CountryTotals
                                      orderby item.Item2 descending
                                      select item.Item1).ToList();

            // add the date & total columns
            _countryDeltasTable.Columns.Add(
                new DataColumn("Date", _book.Date.GetType()));
            _countryDeltasTable.Columns.Add(
                new DataColumn("TotalBooks", latestTally.TotalBooks.GetType()));
            _countryDeltasTable.Columns.Add(
                new DataColumn("TotalPages", latestTally.TotalPages.GetType()));

            // add the 4 columns for each of countries: ttl books, % book, ttl pages, % pages
            foreach (var country in countries)
            {
                _countryDeltasTable.Columns.Add(
                    new DataColumn(country + "TotalBooks", typeof(UInt32)));
                _countryDeltasTable.Columns.Add(
                    new DataColumn(country + "PercentageBooks", typeof(double)));
                _countryDeltasTable.Columns.Add(
                    new DataColumn(country + "TotalPages", typeof(UInt32)));
                _countryDeltasTable.Columns.Add(
                    new DataColumn(country + "PercentagePages", typeof(double)));
            }

            // loop through the deltas adding rows foreach
            for (int row = 0; row < BookDeltas.Count; ++row)
            {
                var delta = BookDeltas[row];
                DataRow newRow = _countryDeltasTable.NewRow();
                newRow["Date"] = delta.Date;
                newRow["TotalBooks"] = delta.OverallTally.TotalBooks;
                newRow["TotalPages"] = delta.OverallTally.TotalPages;

                foreach (var country in countries)
                {
                    UInt32 ttlBooks, ttlPages;
                    double pctBooks, pctPages;
                    GetCountryTotalsForDelta(delta, country,
                        out ttlBooks, out ttlPages, out pctBooks, out pctPages);

                    newRow[country + "TotalBooks"] = ttlBooks;
                    newRow[country + "PercentageBooks"] = pctBooks;
                    newRow[country + "TotalPages"] = ttlPages;
                    newRow[country + "PercentagePages"] = pctPages;
                }

                _countryDeltasTable.Rows.Add(newRow);
            }
        }

        private void BuildLanguageDeltasTable()
        {
            _languageDeltasTable = new DataTable("LanguageDeltasTable");
            if (!BookDeltas.Any()) return;

            // get all the languages from the last delta item in order
            BooksDelta.DeltaTally latestTally = BookDeltas.Last().OverallTally;
            List<string> languages = (from item in latestTally.LanguageTotals
                                      orderby item.Item2 descending
                                      select item.Item1).ToList();

            // add the date & total columns
            _languageDeltasTable.Columns.Add(
                new DataColumn("Date", _book.Date.GetType()));
            _languageDeltasTable.Columns.Add(
                new DataColumn("TotalBooks", latestTally.TotalBooks.GetType()));
            _languageDeltasTable.Columns.Add(
                new DataColumn("TotalPages", latestTally.TotalPages.GetType()));

            // add the 4 columns for each of languages: ttl books, % book, ttl pages, % pages
            foreach (var language in languages)
            {
                _languageDeltasTable.Columns.Add(
                    new DataColumn(language + "TotalBooks", typeof(UInt32)));
                _languageDeltasTable.Columns.Add(
                    new DataColumn(language + "PercentageBooks", typeof(double)));
                _languageDeltasTable.Columns.Add(
                    new DataColumn(language + "TotalPages", typeof(UInt32)));
                _languageDeltasTable.Columns.Add(
                    new DataColumn(language + "PercentagePages", typeof(double)));
            }

            // loop through the deltas adding rows foreach
            for (int row = 0; row < BookDeltas.Count; ++row)
            {
                var delta = BookDeltas[row];
                DataRow newRow = _languageDeltasTable.NewRow();
                newRow["Date"] = delta.Date;
                newRow["TotalBooks"] = delta.OverallTally.TotalBooks;
                newRow["TotalPages"] = delta.OverallTally.TotalPages;

                foreach (var language in languages)
                {
                    UInt32 ttlBooks, ttlPages;
                    double pctBooks, pctPages;
                    GetLanguageTotalsForDelta(delta, language,
                        out ttlBooks, out ttlPages, out pctBooks, out pctPages);

                    newRow[language + "TotalBooks"] = ttlBooks;
                    newRow[language + "PercentageBooks"] = pctBooks;
                    newRow[language + "TotalPages"] = ttlPages;
                    newRow[language + "PercentagePages"] = pctPages;
                }

                _languageDeltasTable.Rows.Add(newRow);
            }
        }

        private static void GetCountryTotalsForDelta(BooksDelta delta, string language,
            out UInt32 ttlBooks, out UInt32 ttlPages, out double pctBooks, out double pctPages)
        {
            ttlBooks = 0;
            ttlPages = 0;
            pctBooks = 0;
            pctPages = 0;

            foreach (var total in delta.OverallTally.CountryTotals)
            {
                if (language != total.Item1) continue;
                ttlBooks = total.Item2;
                pctBooks = total.Item3;
                ttlPages = total.Item4;
                pctPages = total.Item5;
                break;
            }
        }

        private static void GetLanguageTotalsForDelta(BooksDelta delta, string language,
            out UInt32 ttlBooks, out UInt32 ttlPages, out double pctBooks, out double pctPages)
        {
            ttlBooks = 0;
            ttlPages = 0;
            pctBooks = 0;
            pctPages = 0;

            foreach (var total in delta.OverallTally.LanguageTotals)
            {
                if (language != total.Item1) continue;
                ttlBooks = total.Item2;
                pctBooks = total.Item3;
                ttlPages = total.Item4;
                pctPages = total.Item5;
                break;
            }
        }

        public static DataTable BuildSeriesTable(string title,
            List<Tuple<string, string, Type>> seriesFieldNames, IList<object> dataItems)
        {
            DataTable seriesTable = new DataTable(title);

            // get the data and the columns
            IList<IList<object>> seriesFieldValues = new List<IList<object>>();
            foreach (var item in seriesFieldNames)
            {
                string colHeader = item.Item1;
                string propertyName = item.Item2;
                var type = item.Item3;

                seriesTable.Columns.Add(new DataColumn(colHeader, type));

                List<object> objectValsForProperty = new List<object>();
                foreach (var dataItem in dataItems)
                {
                    object propertyValue =
                        dataItem.GetType().GetProperty(propertyName).GetValue(dataItem, null);
                    objectValsForProperty.Add(propertyValue);
                }

                seriesFieldValues.Add(objectValsForProperty);
            }

            for (int row = 0; row < seriesFieldValues[0].Count; ++row)
            {
                DataRow newRow = seriesTable.NewRow();

                for (int col = 0; col < seriesFieldValues.Count; ++col)
                {
                    var colVal = seriesFieldValues[col][row];
                    newRow[col] = colVal;
                }
                seriesTable.Rows.Add(newRow);
            }

            return seriesTable;
        }

        public static DataTable BuildSeriesTable(string title,
            List<KeyValuePair<string, string>> seriesFieldNames, IList<object> dataItems)
        {
            DataTable seriesTable = new DataTable(title);

            // get the data and the columns
            IList<IList<object>> seriesFieldValues = new List<IList<object>>();
            foreach (var item in seriesFieldNames)
            {
                string colHeader = item.Value;
                string propertyName = item.Key;

                seriesTable.Columns.Add(new DataColumn(colHeader));

                List<object> objectValsForProperty = new List<object>();
                foreach (var dataItem in dataItems)
                {
                    object propertyValue =
                        dataItem.GetType().GetProperty(propertyName).GetValue(dataItem, null);
                    objectValsForProperty.Add(propertyValue);
                }

                seriesFieldValues.Add(objectValsForProperty);
            }

            for (int row = 0; row < seriesFieldValues[0].Count; ++row)
            {
                DataRow newRow = seriesTable.NewRow();

                for (int col = 0; col < seriesFieldValues.Count; ++col)
                {
                    var colVal = seriesFieldValues[col][row];
                    if (colVal is double)
                    {
                        double asDouble = (double)colVal;
                        newRow[col] = asDouble.ToString("0.##");
                    }
                    else
                        newRow[col] = colVal;
                }
                seriesTable.Rows.Add(newRow);
            }

            return seriesTable;
        }

        private void SelectImageForNation(Nation nation)
        {
            _log.Debug("Getting Nation information for " + nation.Name);

            ImageSelectionViewModel selectionViewModel = new ImageSelectionViewModel(_log, nation.Name);

            ImageSelectionWindow imageSelectDialog = new ImageSelectionWindow { DataContext = selectionViewModel };
            var success = imageSelectDialog.ShowDialog();
            if (success.HasValue && success.Value)
            {
                _log.Debug("Success Getting Nation information for " + nation.Name +
                           "\n   Img = " + selectionViewModel.SelectedImageAddress);

                nation.ImageUri = selectionViewModel.SelectedImageAddress;
                _mainModel.NationDatabase.UpdateDatabaseItem(nation);

                OnPropertyChanged(() => Nations);
            }
            else
            {
                _log.Debug("Failed Getting Nation information for " + nation.Name);
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// Gets the select image for nation command.
        /// </summary>
        public ICommand SelectImageForNationCommand => _selectImageForNationCommand ??
                                            (_selectImageForNationCommand =
                                             new RelayCommandHandler(SelectImageForNationCommandAction) {IsEnabled = true});
        #endregion

        #region Command Handlers

        /// <summary>
        /// The command action to add a new book from the email to the database.
        /// </summary>
        public void SelectImageForNationCommandAction(object parameter)
        {
            Nation nation = parameter as Nation;
            if (nation != null)
            {
                SelectImageForNation(nation);
                return;
            }

            AuthorCountry authorCountry = parameter as AuthorCountry;
            if (authorCountry?.Nation != null)
            {
                SelectImageForNation(authorCountry.Nation);
            }
        }

        #endregion
    }
}
