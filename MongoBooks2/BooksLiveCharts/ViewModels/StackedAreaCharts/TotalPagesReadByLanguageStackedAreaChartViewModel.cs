// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TotalPagesReadByLanguageStackedAreaChartViewModel.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The total pages per language stacked area chart view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksLiveCharts.ViewModels.StackedAreaCharts
{
    using LiveCharts;

    /// <summary>
    /// The total pages per language stacked area chart view model class.
    /// </summary>
    public sealed class TotalPagesReadByLanguageStackedAreaChartViewModel : BaseStackedAreaChartViewModel
    {
        /// <summary>
        /// Sets up the stacked area chart series.
        /// </summary>
        protected override void SetupSeries()
        {
            SetupSeries(false, false);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TotalPagesReadByCountryStackedAreaChartViewModel"/> class.
        /// </summary>
        public TotalPagesReadByLanguageStackedAreaChartViewModel()
        {
            Title = "Total Pages Read by Language";
            PointLabel = chartPoint => $"{chartPoint.Y:G6}";
            LegendLocation = LegendLocation.Bottom;
            SetupSeries();
        }
    }
}
