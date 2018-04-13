// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthorsGridViewModel.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The languages grid view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksEditors.ViewModels.Grids
{
    using System.Collections.ObjectModel;
    using BooksCore.Books;

    /// <summary>
    /// The languages grid view model class.
    /// </summary>
    public sealed class LanguagesGridViewModel : BaseGridViewModel
    {
        /// <summary>
        /// The author languages for the grid.
        /// </summary>
        private ObservableCollection<AuthorLanguage> _authorLanguages;

        /// <summary>
        /// Gets the author languages for the grid.
        /// </summary>
        public ObservableCollection<AuthorLanguage> AuthorLanguages
        {
            get
            {
                return _authorLanguages;
            }

            private set
            {
                _authorLanguages = value;
                OnPropertyChanged(() => AuthorLanguages);
            }

        }

        /// <summary>
        /// Sets up the author languages grid.
        /// </summary>
        protected override void SetupGrid()
        {
            Title = "Languages";

            // If no books return the default.
            if (BooksReadProvider == null)
            {
                _authorLanguages = new ObservableCollection<AuthorLanguage>();
                return;
            }

            // Otherwise set to the author languages.
            AuthorLanguages = BooksReadProvider.AuthorLanguages;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LanguagesGridViewModel"/> class.
        /// </summary>
        public LanguagesGridViewModel()
        {
            Title = "Languages";
            SetupGrid();
        }
    }
}
