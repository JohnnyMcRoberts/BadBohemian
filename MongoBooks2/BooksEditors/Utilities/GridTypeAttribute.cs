// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GridTypeAttribute.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The grid type attribute.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksEditors.Utilities
{
    using System;

    [AttributeUsage(AttributeTargets.Field)]
    public class GridTypeAttribute : Attribute
    {
        public string Title { get; set; }

        public Type GeneratorClass { get; set; }

        public GridTypeAttribute()
        {
            Title = string.Empty;
        }
    }
}
