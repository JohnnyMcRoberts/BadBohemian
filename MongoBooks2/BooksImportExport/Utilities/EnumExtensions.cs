// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChartTypeAttribute.cs" company="N/A">
//   2016
// </copyright>
// <summary>
//   The extension methods for the import export types.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace BooksImportExport.Utilities
{
    using System;
    using System.Reflection;

    public static class EnumExtensions
    {
        /// <summary>
        /// Gets the field info for an enum element with the <c>ImportExportTypeAttribute</c> set.
        /// </summary>
        /// <param name="value">The enum value.</param>
        /// <param name="field">The field info on exit.</param>
        /// <returns>True if missing the attribute, false otherwise.</returns>
        private static bool GetFieldInfo(Enum value, out FieldInfo field)
        {
            field = null;
            Type type = value.GetType();

            string name = Enum.GetName(type, value);
            if (name == null)
            {
                return true;
            }

            field = type.GetField(name);
            return field == null;
        }

        /// <summary>
        /// Gets the title string from an enum element with the <c>ImportExportTypeAttribute</c> set.
        /// </summary>
        /// <param name="value">The enum value.</param>
        /// <returns>The title string.</returns>
        public static string GetTitle(this Enum value)
        {
            FieldInfo field;
            if (GetFieldInfo(value, out field))
                return null;

            ImportExportTypeAttribute attr =
                Attribute.GetCustomAttribute(field, typeof(ImportExportTypeAttribute)) as ImportExportTypeAttribute;

            return attr?.Title;
        }

        /// <summary>
        /// Gets the type generator class from an enum element with the <c>ImportExportTypeAttribute</c> set.
        /// </summary>
        /// <param name="value">The enum value.</param>
        /// <returns>The title string.</returns>
        public static Type GetGeneratorClass(this Enum value)
        {
            FieldInfo field;
            if (GetFieldInfo(value, out field))
                return null;

            ImportExportTypeAttribute attr =
                Attribute.GetCustomAttribute(field, typeof(ImportExportTypeAttribute)) as ImportExportTypeAttribute;

            return attr?.GeneratorClass;
        }
    }
}
