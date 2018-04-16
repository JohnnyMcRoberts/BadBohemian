// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChartTypeAttribute.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The import export type attribute.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksImportExport.Utilities
{
    using System;

    [AttributeUsage(AttributeTargets.Field)]
    public class ImportExportTypeAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the generator class.
        /// </summary>
        public Type GeneratorClass { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportExportTypeAttribute"/> class.
        /// </summary>
        public ImportExportTypeAttribute()
        {
            Title = string.Empty;
        }
    }
}
