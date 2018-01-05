namespace MongoDbBooks.ViewModels.PlotGenerators
{
    using OxyPlot;

    using MongoDbBooks.Models;

    public interface IPlotGenerator
    {
        PlotModel SetupPlot(MainBooksModel mainModel);
    }
}
