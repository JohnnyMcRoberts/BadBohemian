// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BasePlotGenerator.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The base oxy-plot generator class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksOxyCharts.PlotGenerators
{
    using BooksCore.Interfaces;
    using BooksOxyCharts.Interfaces;
    using OxyPlot;

    /// <summary>
    /// The base plot generator class.
    /// </summary>
    public abstract class BasePlotGenerator : IPlotGenerator
    {
        /// <summary>
        /// Gets the geography data for the plots.
        /// </summary>
        public IGeographyProvider GeographyProvider { get; private set; }

        /// <summary>
        /// Gets the books read data for the plots.
        /// </summary>
        public IBooksReadProvider BooksReadProvider { get; private set; }

        /// <summary>
        /// Sets up the providers then gets plot model to be displayed.
        /// </summary>
        /// <param name="geographyProvider">The geography data source.</param>
        /// <param name="booksReadProvider">The books data source.</param>
        /// <returns>The user view item model.</returns>
        public PlotModel SetupPlot(IGeographyProvider geographyProvider, IBooksReadProvider booksReadProvider)
        {
            GeographyProvider = geographyProvider;
            BooksReadProvider = booksReadProvider;
            return SetupPlot();
        }

        /// <summary>
        /// Sets up the plot model to be displayed.
        /// </summary>
        /// <returns>The plot model.</returns>
        protected abstract PlotModel SetupPlot();
    }
}
