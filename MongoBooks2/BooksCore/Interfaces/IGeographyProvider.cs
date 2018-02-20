namespace BooksCore.Interfaces
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using BooksCore.Geography;

    public interface IGeographyProvider
    {
        ObservableCollection<WorldCountry> WorldCountries { get; }

        ObservableCollection<Nation> Nations { get; }
    }
}
