// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LineChartType.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The grid view types enum.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksEditors.ViewModels.Grids
{
    using BooksEditors.Utilities;

    /// <summary>
    /// Type of grid view.
    /// </summary>
    public enum GridType
    {
        [GridType(Title = "Authors",
            GeneratorClass = typeof(AuthorsGridViewModel))]
        Authors,

        [GridType(Title = "Languages",
            GeneratorClass = typeof(LanguagesGridViewModel))]
        Languages,
    }
}
