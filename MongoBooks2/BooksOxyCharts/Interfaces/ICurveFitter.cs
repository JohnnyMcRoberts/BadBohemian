// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The main view model for books helix chart test application.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksOxyCharts.Utilities
{
    public interface ICurveFitter
    {
        double EvaluateYValueAtPoint(double xVal);
    }
}
