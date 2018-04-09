// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TotalPagesReadByLanguageStackedAreaChartViewModel.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The total books per language stacked area chart view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksLiveCharts.ViewModels.StackedAreaCharts
{
    using LiveCharts;

    /// <summary>
    /// The total books per language stacked area chart view model class.
    /// </summary>
    public sealed class TotalBooksReadByLanguageStackedAreaChartViewModel : BaseStackedAreaChartViewModel
    {
        /// <summary>
        /// Sets up the stacked area chart series.
        /// </summary>
        protected override void SetupSeries()
        {
            SetupSeries(true, false);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TotalBooksReadByCountryStackedAreaChartViewModel"/> class.
        /// </summary>
        public TotalBooksReadByLanguageStackedAreaChartViewModel()
        {
            Title = "Total Pages Read by Language";
            PointLabel = chartPoint => $"{chartPoint.Y:G6}";
            LegendLocation = LegendLocation.Bottom;
            SetupSeries();
        }
    }
}
