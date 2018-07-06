// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EditBookViewModel.cs" company="N/A">
//   2018
// </copyright>
// <summary>
//   The book editor view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksEditors.ViewModels.Editors
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Input;
    using BooksCore.Books;
    using BooksEditors.Views.Editors;
    using BooksUtilities.ViewModels;

    /// <summary>
    /// The book editor view model class.
    /// </summary>
    public sealed class BookEditorViewModel : BaseEditorViewModel
    {
        #region Private Data

        /// <summary>
        /// The selected book.
        /// </summary>
        private BookRead _selectedBook;

        private readonly Dictionary<BookFormat, string> _bookFormats = new Dictionary<BookFormat, string>()
        {
            {BookFormat.Book, "Book"},
            {BookFormat.Comic, "Comic"},
            {BookFormat.Audio, "Audio"}
        };

        /// <summary>
        /// The text to show in the update book command button.
        /// </summary>
        private string _updateBookCommandText;

        /// <summary>
        /// The select image for book command.
        /// </summary>
        private ICommand _selectImageForBookCommand;

        /// <summary>
        /// The add book tag command.
        /// </summary>
        private ICommand _addBookTagCommand;

        /// <summary>
        /// The clear the book tags command.
        /// </summary>
        private ICommand _clearBookTagsCommand;

        /// <summary>
        /// The update selected book command.
        /// </summary>
        private ICommand _updateBookCommand;

        #endregion

        #region Public Data

        /// <summary>
        /// Gets the current set of books.
        /// </summary>
        public ObservableCollection<BookRead> BooksRead =>
            BooksReadProvider != null
                ? BooksReadProvider.BooksRead
                : new ObservableCollection<BookRead>();

        /// <summary>
        /// Gets the current set of book tags.
        /// </summary>
        public List<string> BookTags =>
            BooksReadProvider?.BookTags.Select(tag => tag.Tag.Trim()).OrderBy(x => x).ToList() ?? new List<string>();

        /// <summary>
        /// Gets the current set of author names.
        /// </summary>
        public List<string> AuthorNames => (from book in BooksRead orderby book.Author select book.Author).Distinct().ToList();

        /// <summary>
        /// Gets the current set of author nationalities.
        /// </summary>
        public List<string> AuthorNationalities => 
            (GeographyProvider != null) 
                ? (from country in GeographyProvider.WorldCountries orderby country.Country select country.Country).Distinct().ToList()
                : new List<string>();

        /// <summary>
        /// Gets the current set of original languages.
        /// </summary>
        public List<string> OriginalLanguages => (from book in BooksRead orderby book.OriginalLanguage select book.OriginalLanguage).Distinct().ToList();

        /// <summary>
        /// Gets the available formats for the books.
        /// </summary>
        public Dictionary<BookFormat, string> BookFormats => _bookFormats;

        /// <summary>
        /// Gets or sets the date the book was read.
        /// </summary>
        public DateTime BookDate
        {
            get { if (_selectedBook != null) return _selectedBook.Date; return DateTime.Now; }
            set { if (_selectedBook != null) _selectedBook.Date = value; OnPropertyChanged(() => BookDateText); }
        }

        /// <summary>
        /// Gets or sets the author of the book.
        /// </summary>
        public string BookAuthor
        {
            get
            {
                return _selectedBook?.Author;
            }

            set
            {
                if (value != null)
                    _selectedBook.Author = value;
            }
        }

        /// <summary>
        /// Gets or sets the title of the book.
        /// </summary>
        public string BookTitle
        {
            get
            {
                return _selectedBook?.Title;
            }
            set
            {
                if (_selectedBook != null)
                    _selectedBook.Title = value;
            }
        }

        /// <summary>
        /// Gets or sets the number of pages in the book.
        /// </summary>
        public UInt16 BookPages
        {
            get
            {
                if (_selectedBook != null)
                    return _selectedBook.Pages; return 0;
            }

            set
            {
                if (_selectedBook != null)
                    _selectedBook.Pages = value;
            }
        }

        /// <summary>
        /// Gets or sets the note.
        /// </summary>
        public string BookNote
        {
            get
            {
                return _selectedBook?.Note;
            }
            set
            {
                if (_selectedBook != null)
                    _selectedBook.Note = value;
            }
        }

        /// <summary>
        /// Gets or sets the nationality.
        /// </summary>
        public string BookNationality
        {
            get
            {
                return _selectedBook?.Nationality;
            }
            set
            {
                _selectedBook.Nationality = value;
            }
        }

        /// <summary>
        /// Gets or sets the original language.
        /// </summary>
        public string BookOriginalLanguage
        {
            get
            {
                return _selectedBook?.OriginalLanguage;
            }

            set
            {
                if (_selectedBook != null)
                    _selectedBook.OriginalLanguage = value;
            }
        }

        /// <summary>
        /// Gets or sets the format of the book.
        /// </summary>
        public BookFormat BookFormat
        {
            get
            {
                if (_selectedBook != null)
                    return _selectedBook.Format;

                return BookFormat.Book;
            }
            set
            {
                if (_selectedBook != null)
                    _selectedBook.Format = value;
                OnPropertyChanged(() => BookFormat);
            }
        }

        /// <summary>
        /// Gets the text for the date.
        /// </summary>
        public string BookDateText
        {
            get
            {
                if (_selectedBook == null)
                    return "";

                _selectedBook.DateString = GetBookDateText();
                return _selectedBook.DateString;
            }
        }

        /// <summary>
        /// Gets or sets the text for the author.
        /// </summary>
        public string BookAuthorText { get; set; }

        /// <summary>
        /// Gets or sets the text for the nationality.
        /// </summary>
        public string BookNationalityText { get; set; }

        /// <summary>
        /// Gets or sets the text for the original language.
        /// </summary>
        public string BookOriginalLanguageText { get; set; }

        /// <summary>
        /// Gets the image URI.
        /// </summary>
        public Uri BookImageSource => _selectedBook?.DisplayImage;

        /// <summary>
        /// Gets or sets the new tag.
        /// </summary>
        public string BookNewTag { get; set; }

        /// <summary>
        /// Gets or sets the text for the new tag.
        /// </summary>
        public string BookNewTagText { get; set; }

        public List<string> SelectedBookTags
        {
            get { if (_selectedBook != null) return _selectedBook.Tags; return null; }
            set { if (value != null) _selectedBook.Tags = value; }
        }

        /// <summary>
        /// Gets the tags list ready to be displayed.
        /// </summary>
        public string BookDisplayTags
        {
            get
            {
                if (SelectedBookTags == null || SelectedBookTags.Count == 0)
                {
                    return string.Empty;
                }

                if (SelectedBookTags.Count == 0)
                {
                    return SelectedBookTags[0];
                }

                string listOfTags = SelectedBookTags[0];
                for (int i = 1; i < SelectedBookTags.Count; i++)
                {
                    listOfTags += ", " + SelectedBookTags[i];
                }

                return listOfTags;
            }
        }

        /// <summary>
        /// Gets or sets the selected book.
        /// </summary>
        public BookRead SelectedBook
        {
            get
            {
                return _selectedBook;
            }

            set
            {
                _selectedBook = value;
                OnPropertyChanged("");
            }
        }

        /// <summary>
        /// Gets or sets the text for the update book button.
        /// </summary>
        public string UpdateBookCommandText
        {
            get
            {
                return _updateBookCommandText;
            }

            set
            {
                _updateBookCommandText = value; OnPropertyChanged(() => UpdateBookCommandText);
            }
        }

        /// <summary>
        /// Gets or sets the action for the update book.
        /// </summary>
        public Action<BookRead> UpdateBookAction { get; set; }

        #endregion

        #region Commands

        /// <summary>
        /// Gets the select image for a book command.
        /// </summary>
        public ICommand SelectImageForBookCommand => _selectImageForBookCommand ??
                                                     (_selectImageForBookCommand =
                                                         new RelayCommandHandler(SelectImageForBookCommandAction) { IsEnabled = true });

        /// <summary>
        /// Gets the add book tag command.
        /// </summary>
        public ICommand AddBookTagCommand => _addBookTagCommand ??
                                                     (_addBookTagCommand =
                                                         new RelayCommandHandler(AddBookTagCommandAction) { IsEnabled = true });

        /// <summary>
        /// Gets the clear the book tags command.
        /// </summary>
        public ICommand ClearBookTagsCommand => _clearBookTagsCommand ??
                                             (_clearBookTagsCommand =
                                                 new RelayCommandHandler(ClearBookTagsCommandAction) { IsEnabled = true });

        /// <summary>
        /// Gets the update selected book command.
        /// </summary>
        public ICommand UpdateBookCommand => _updateBookCommand ??
                                                (_updateBookCommand =
                                                    new RelayCommandHandler(UpdateBookCommandAction) { IsEnabled = true });
        #endregion

        #region Command handlers

        /// <summary>
        /// The command action to update the selected book.
        /// </summary>
        public void UpdateBookCommandAction(object parameter)
        {
            UpdateBookAction?.Invoke(_selectedBook);
        }

        /// <summary>
        /// The command action to clear the book tags.
        /// </summary>
        public void ClearBookTagsCommandAction(object parameter)
        {
            SelectedBookTags.Clear();
            OnPropertyChanged(() => BookDisplayTags);
            BookNewTagText = string.Empty;
        }

        /// <summary>
        /// The command action to add a book tag.
        /// </summary>
        public void AddBookTagCommandAction(object parameter)
        {
            if (!string.IsNullOrEmpty(BookNewTagText))
            {
                if (!SelectedBookTags.Contains(BookNewTagText))
                {
                    SelectedBookTags.Add(BookNewTagText);
                    OnPropertyChanged(() => BookDisplayTags);
                    BookNewTagText = string.Empty;
                }
            }
        }

        /// <summary>
        /// The command action to select an image for a book.
        /// </summary>
        public void SelectImageForBookCommandAction(object parameter)
        {
            if (_selectedBook != null)
            {
                SelectImageForBook(_selectedBook);
            }
        }

        #endregion

        #region Utility functions

        private string GetBookDateText()
        {
            DateTime date = BookDate;

            string datetext = date.ToString("y");
            datetext = datetext.Replace(", ", " ");
            int day = date.Day;
            string dayString = day.ToString() + "th ";
            switch (day)
            {
                case 1: dayString = "1st "; break;
                case 21: dayString = "21st "; break;
                case 31: dayString = "31st "; break;
                case 2: dayString = "2nd "; break;
                case 22: dayString = "22nd "; break;
                case 3: dayString = "3rd "; break;
                case 23: dayString = "23rd "; break;
            }

            datetext = dayString + datetext;

            return datetext;

        }
        
        private void SelectImageForBook(BookRead book)
        {
            ImageSelectionWindowViewModel selectionViewModel =
                new ImageSelectionWindowViewModel { SearchTerm = $"{book.Author} {book.Title} amazon"};

            ImageSelectionWindowView imageSelectDialog = new ImageSelectionWindowView(selectionViewModel);
            imageSelectDialog.ShowDialog();
            if (selectionViewModel.DialogResult.HasValue && selectionViewModel.DialogResult.Value)
            {
                book.ImageUrl = selectionViewModel.SelectedImageAddress;
                OnPropertyChanged(() => BookImageSource);
            }
        }

        #endregion

        /// <summary>
        /// Sets up the editor.
        /// </summary>
        protected override void SetupEditor()
        {

        }

        public BookEditorViewModel()
        {
            _updateBookCommandText = "Update Book";
        }
    }
}
