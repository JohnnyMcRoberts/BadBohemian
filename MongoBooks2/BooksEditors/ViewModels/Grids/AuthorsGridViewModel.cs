// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseGridViewModel.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The authors grid view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksEditors.ViewModels.Grids
{
    using System.Collections.ObjectModel;
    using BooksCore.Books;

    /// <summary>
    /// The average days per book with time line chart view model class.
    /// </summary>
    public sealed class AuthorsGridViewModel : BaseGridViewModel
    {
        /// <summary>
        /// The authors read list for the grid.
        /// </summary>
        private ObservableCollection<BookAuthor> _authorsRead;

        /// <summary>
        /// Gets the Authors read for the grid.
        /// </summary>
        public ObservableCollection<BookAuthor> AuthorsRead
        {
            get
            {
                return _authorsRead;
            }

            private set
            {
                _authorsRead = value;
                OnPropertyChanged(() => AuthorsRead);
            }

        }

        /// <summary>
        /// Sets up the authors grid.
        /// </summary>
        protected override void SetupGrid()
        {
            Title = "Authors";

            // If no books return the default.
            if (BooksReadProvider == null)
            {
                _authorsRead = new ObservableCollection<BookAuthor>();
                return;
            }

            // Otherwise set to the books authors.
            AuthorsRead = BooksReadProvider.AuthorsRead;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorsGridViewModel"/> class.
        /// </summary>
        public AuthorsGridViewModel()
        {
            Title = "Authors";
            SetupGrid();
        }

    }
}
