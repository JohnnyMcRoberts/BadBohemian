// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EditBookViewModel.cs" company="N/A">
//   2018
// </copyright>
// <summary>
//   The base editor view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksEditors.ViewModels.Editors
{
    using BooksCore.Interfaces;
    using BooksUtilities.ViewModels;

    /// <summary>
    /// The base editor view model class.
    /// </summary>
    public abstract class BaseEditorViewModel : BaseViewModel
    {
        /// <summary>
        /// Gets the geography data for the editor.
        /// </summary>
        public IGeographyProvider GeographyProvider { get; private set; }

        /// <summary>
        /// Gets the books read data for the editor.
        /// </summary>
        public IBooksReadProvider BooksReadProvider { get; private set; }

        /// <summary>
        /// Sets up the editor values.
        /// </summary>
        protected abstract void SetupEditor();

        /// <summary>
        /// Sets up the providers then gets editor to be displayed.
        /// </summary>
        /// <param name="geographyProvider">The geography data source.</param>
        /// <param name="booksReadProvider">The books data source.</param>
        public void SetupEditor(IGeographyProvider geographyProvider, IBooksReadProvider booksReadProvider)
        {
            GeographyProvider = geographyProvider;
            BooksReadProvider = booksReadProvider;
            SetupEditor();
        }
    }
}
