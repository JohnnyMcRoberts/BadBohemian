// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TotalBooksReadByCountryStackedAreaChartViewModel.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The total books per country stacked area chart view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksLiveCharts.ViewModels.StackedAreaCharts
{
    using LiveCharts;

    /// <summary>
    /// The total books per country stacked area chart view model class.
    /// </summary>
    public sealed class TotalBooksReadByCountryStackedAreaChartViewModel : BaseStackedAreaChartViewModel
    {
        /// <summary>
        /// Sets up the stacked area chart series.
        /// </summary>
        protected override void SetupSeries()
        {
            SetupSeries(true, true);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TotalBooksReadByCountryStackedAreaChartViewModel"/> class.
        /// </summary>
        public TotalBooksReadByCountryStackedAreaChartViewModel()
        {
            Title = "Total Books Read by Country";
            PointLabel = chartPoint => $"{chartPoint.Y:G3}";
            LegendLocation = LegendLocation.Bottom;
            SetupSeries();
        }
    }
}
