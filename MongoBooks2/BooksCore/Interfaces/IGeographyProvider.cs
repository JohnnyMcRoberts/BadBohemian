// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IGeographyProvider.cs" company="N/A">
//   2017-2086
// </copyright>
// <summary>
//   The geography provider interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksCore.Interfaces
{
    using System.Collections.ObjectModel;

    using BooksCore.Geography;

    public interface IGeographyProvider
    {
        ObservableCollection<WorldCountry> WorldCountries { get; }

        ObservableCollection<Nation> Nations { get; }

        ObservableCollection<CountryGeography> CountryGeographies { get; }
    }
}
