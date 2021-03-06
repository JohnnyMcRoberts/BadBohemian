﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthorsPerCountryMapChartViewModel.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The authors per country geo map chart view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksLiveCharts.ViewModels.GeoMapCharts
{
    using System;
    using System.Collections.Generic;
    using BooksCore.Books;
    using BooksCore.Geography;
    using LiveCharts;

    /// <summary>
    /// The authors per country map chart view model class.
    /// </summary>
    public sealed class AuthorsPerCountryMapChartViewModel : BaseGeoMapChartViewModel
    {
        /// <summary>
        /// Sets up the map chart series values.
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
            double minValue = 1;
            double maxValue = 2;
            foreach (AuthorCountry authorCountry in BooksReadProvider.AuthorCountries)
            {
                Nation nation = authorCountry.Nation;
                int total = authorCountry.AuthorsFromCountry.Count;
                string code = GetNationCode(nation);
                if (!string.IsNullOrEmpty(code) && total > 0)
                {
                    Values.Add(code, Math.Log10(total));
                    minValue = Math.Min(minValue, total);
                    maxValue = Math.Max(maxValue, total);
                }
            }

            // If no countries set up use the defaults.
            if (Values.Count == 0)
            {
                base.SetupSeries();
            }
            else
            {
                MaxValue = maxValue;
                MinValue = minValue;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorsPerCountryMapChartViewModel"/> class.
        /// </summary>
        public AuthorsPerCountryMapChartViewModel()
        {
            Title = "Books Per Country";
            LegendLocation = LegendLocation.None;
            SetupWorldMapFile();
            SetupColorGradient();
            SetupSeries();
        }
    }
}