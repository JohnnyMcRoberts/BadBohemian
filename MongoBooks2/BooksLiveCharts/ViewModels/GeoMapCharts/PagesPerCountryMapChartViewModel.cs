// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PagesPerDayWithTimeLineChartViewModel.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The pages per day with time line chart view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksLiveCharts.ViewModels.GeoMapCharts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Media;
    using BooksCore.Books;
    using BooksUtilities.Colors;
    using LiveCharts;
    using LiveCharts.Definitions.Series;

    /// <summary>
    /// The pages per country map chart view model class.
    /// </summary>
    public sealed class PagesPerCountryMapChartViewModel : BaseGeoMapChartViewModel
    {
        /// <summary>
        /// Sets up the map chart series.
        /// </summary>
        protected override void SetupSeries()
        {
            // If no books return the default.
            if (BooksReadProvider == null)
            {
                base.SetupSeries();
                return;
            }

            Values = new Dictionary<string, double>();

            foreach (AuthorCountry authorCountry in BooksReadProvider.AuthorCountries)
            {
                var nation = authorCountry.Nation;
                var count = authorCountry.TotalPagesReadFromCountry;
                if (!string.IsNullOrEmpty(nation.Geography.ISO_A2) && count > 0)
                {
                    Values.Add(nation.Geography.ISO_A2, Math.Log10(count));
                }
            }

            if (Values.Count == 0)
            {
                Values["MX"] = 50;
                Values["US"] = 100;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PagesPerCountryMapChartViewModel"/> class.
        /// </summary>
        public PagesPerCountryMapChartViewModel()
        {
            Title = "pages Per Country";
            LegendLocation = LegendLocation.None;
            SetupWorldMapFile();
            //SetupColorGradient();

            SetupSeries();
        }
    }
}