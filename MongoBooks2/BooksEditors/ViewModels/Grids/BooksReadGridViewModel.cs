// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthorsGridViewModel.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The books read grid view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksEditors.ViewModels.Grids
{
    using System.Collections.ObjectModel;
    using BooksCore.Books;

    /// <summary>
    /// The books read grid view model class.
    /// </summary>
    public sealed class BooksReadGridViewModel : BaseGridViewModel
    {
        /// <summary>
        /// The author languages for the grid.
        /// </summary>
        private ObservableCollection<BookRead> _booksRead;

        /// <summary>
        /// Gets the books read for the grid.
        /// </summary>
        public ObservableCollection<BookRead> BooksRead
        {
            get
            {
                return _booksRead;
            }

            private set
            {
                _booksRead = value;
                OnPropertyChanged(() => BooksRead);
            }

        }

        /// <summary>
        /// Sets up the books read grid.
        /// </summary>
        protected override void SetupGrid()
        {
            Title = "Languages";

            // If no books return the default.
            if (BooksReadProvider == null)
            {
                _booksRead = new ObservableCollection<BookRead>();
                return;
            }

            // Otherwise set to the books read.
            BooksRead = BooksReadProvider.BooksRead;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BooksReadGridViewModel"/> class.
        /// </summary>
        public BooksReadGridViewModel()
        {
            Title = "Books Read";
            SetupGrid();
        }
    }
}
