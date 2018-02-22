namespace BooksCore.Interfaces
{
    using System.Collections.ObjectModel;
    using BooksCore.Books;

    public interface IBooksReadProvider
    {
        ObservableCollection<AuthorCountry> AuthorCountries { get; }
    }
}
