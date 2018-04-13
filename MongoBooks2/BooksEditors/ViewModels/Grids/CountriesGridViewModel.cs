// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthorsGridViewModel.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The countries grid view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksEditors.ViewModels.Grids
{
    using System.Collections.ObjectModel;
    using BooksCore.Books;

    /// <summary>
    /// The countries grid view model class.
    /// </summary>
    public sealed class CountriesGridViewModel : BaseGridViewModel
    {
        /// <summary>
        /// The author countries for the grid.
        /// </summary>
        private ObservableCollection<AuthorCountry> _authorCountries;

        /// <summary>
        /// Gets the author countries for the grid.
        /// </summary>
        public ObservableCollection<AuthorCountry> AuthorCountries
        {
            get
            {
                return _authorCountries;
            }

            private set
            {
                _authorCountries = value;
                OnPropertyChanged(() => AuthorCountries);
            }

        }

        /// <summary>
        /// Sets up the author languages grid.
        /// </summary>
        protected override void SetupGrid()
        {
            Title = "Countries";

            // If no books return the default.
            if (BooksReadProvider == null)
            {
                _authorCountries = new ObservableCollection<AuthorCountry>();
                return;
            }

            // Otherwise set to the author languages.
            AuthorCountries = BooksReadProvider.AuthorCountries;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CountriesGridViewModel"/> class.
        /// </summary>
        public CountriesGridViewModel()
        {
            Title = "Countries";
            SetupGrid();
        }
    }
}
