// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TotalBooksReadByCountryStackedAreaChartViewModel.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The total pages per country stacked area chart view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksLiveCharts.ViewModels.StackedAreaCharts
{
    using LiveCharts;

    /// <summary>
    /// The total pages per country stacked area chart view model class.
    /// </summary>
    public sealed class TotalPagesReadByCountryStackedAreaChartViewModel : BaseStackedAreaChartViewModel
    {
        /// <summary>
        /// Sets up the stacked area chart series.
        /// </summary>
        protected override void SetupSeries()
        {
            SetupSeries(false, true);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TotalPagesReadByCountryStackedAreaChartViewModel"/> class.
        /// </summary>
        public TotalPagesReadByCountryStackedAreaChartViewModel()
        {
            Title = "Total Pages Read by Country";
            PointLabel = chartPoint => $"{chartPoint.Y:G5}";
            LegendLocation = LegendLocation.Bottom;
            SetupSeries();
        }
    }
}
