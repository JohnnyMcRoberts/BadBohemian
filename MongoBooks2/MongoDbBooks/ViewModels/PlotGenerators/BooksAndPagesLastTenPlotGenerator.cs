
namespace MongoDbBooks.ViewModels.PlotGenerators
{
    using System.Collections.Generic;
    using System.Linq;

    using OxyPlot;
    using OxyPlot.Series;
    using OxyPlot.Axes;

    using MongoDbBooks.Models;
    using MongoDbBooks.ViewModels.Utilities;

    public class BooksAndPagesLastTenPlotGenerator : IPlotGenerator
    {
        public OxyPlot.PlotModel SetupPlot(Models.MainBooksModel mainModel)
        {
            _mainModel = mainModel;
            return SetupBooksAndPagesLastTenPlot();
        }
        private Models.MainBooksModel _mainModel;

        private PlotModel SetupBooksAndPagesLastTenPlot()
        {
            // Create the plot model
            var newPlot = new PlotModel { Title = "Last 10 Books Time vs Pages Plot" };
            OxyPlotUtilities.SetupPlotLegend(newPlot, "Last 10 Books Time vs Pages Plot");
            SetupPagesPerDayWithTimeVsTimeAxes(newPlot);

            // create series and add them to the plot
            ScatterSeries pointsSeries;

            OxyPlotUtilities.CreateScatterPointSeries(out pointsSeries, 
                ChartAxisKeys.DaysTakenKey, ChartAxisKeys.TotalPagesReadKey, "Time Taken Vs Pages");

            List<BooksDelta> deltasSet = new List<BooksDelta>();

            double minRate = 1e16;
            double maxRate = 0.0;
            
            foreach (var delta in _mainModel.BookDeltas)
            {
                deltasSet.Add(delta);
                if (deltasSet.Count < 10) continue;

                BooksDelta end = deltasSet.Last();

                double daysTaken = end.LastTenTally.DaysInTally;
                double pagesRead = end.LastTenTally.TotalPages;
                if (daysTaken < 1.0)
                    daysTaken = 1.0;
                double rate = pagesRead / daysTaken;
                ScatterPoint point = 
                    new ScatterPoint(daysTaken, pagesRead, 5, rate) { Tag = end.Date.ToString("ddd d MMM yyy") };
                pointsSeries.Points.Add(point);

                if (minRate > rate) minRate = rate;
                if (maxRate < rate) maxRate = rate;

                deltasSet.RemoveAt(0);                
            }
            pointsSeries.TrackerFormatString = "{Tag}\n{1}: {2:0.###}\n{3}: {4:0.###}";
            newPlot.Series.Add(pointsSeries);
            newPlot.Axes.Add(new LinearColorAxis { Position = AxisPosition.Right, Palette = OxyPalettes.Jet(200) , Title = "Page Rate"});

            // finally update the model with the new plot
            return newPlot;
        }
        
        private void SetupPagesPerDayWithTimeVsTimeAxes(PlotModel newPlot)
        {
            var xAxis = new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Title = "Days Taken",
                Key = ChartAxisKeys.DaysTakenKey,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.None
            };
            newPlot.Axes.Add(xAxis);

            var lhsAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Pages Read",
                Key = ChartAxisKeys.TotalPagesReadKey,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.None
            };
            newPlot.Axes.Add(lhsAxis);
        }

    }
}
