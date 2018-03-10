using System;
using System.Collections.Generic;
using System.Linq;
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChartTypeAttribute.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The base pie-chart view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksLiveCharts.Utilities
{
    using System;

    [AttributeUsage(AttributeTargets.Field)]
    public class ChartTypeAttribute : Attribute
    {
        public string Title { get; set; }

        public Type GeneratorClass { get; set; }

        public ChartTypeAttribute()
        {
            Title = string.Empty;
        }
    }
}
