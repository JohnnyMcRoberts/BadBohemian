// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseGridViewModel.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The base grid view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksEditors.ViewModels.Grids
{
    using BooksCore.Interfaces;
    using BooksUtilities.ViewModels;

    /// <summary>
    /// The base chart view model class.
    /// </summary>
    public abstract class BaseGridViewModel : BaseViewModel
    {
        /// <summary>
        /// Gets the geography data for the plots.
        /// </summary>
        public IGeographyProvider GeographyProvider { get; private set; }

        /// <summary>
        /// Gets the books read data for the plots.
        /// </summary>
        public IBooksReadProvider BooksReadProvider { get; private set; }

        /// <summary>
        /// Gets the grid title.
        /// </summary>
        public string Title { get; protected set; }

        /// <summary>
        /// Sets up the grid.
        /// </summary>
        protected abstract void SetupGrid();

        /// <summary>
        /// Sets up the providers then gets grid ready to be displayed.
        /// </summary>
        /// <param name="geographyProvider">The geography data source.</param>
        /// <param name="booksReadProvider">The books data source.</param>
        public void SetupGrid(IGeographyProvider geographyProvider, IBooksReadProvider booksReadProvider)
        {
            GeographyProvider = geographyProvider;
            BooksReadProvider = booksReadProvider;
            SetupGrid();
        }
    }
}
