// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BooleanToVisibilityConverter.cs" company="N/A">
//   2016-2020
// </copyright>
// <summary>
//   Defines the BooleanToVisibilityConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MongoDbBooks.Converters
{
    using System;
    using System.Windows;
    using System.Windows.Data;

    /// <summary>
    /// The boolean to visibility converter.
    /// </summary>
    public class BooleanToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Visibility visibility = Visibility.Collapsed;
            bool show;
            if (value== null || !bool.TryParse(value.ToString(), out show))
            {
                return visibility;
            }

            bool isInverted;
            if (parameter != null && bool.TryParse(parameter.ToString(), out isInverted))
            {
                show = isInverted ? !show : show;
            }

            visibility = show ? Visibility.Visible : Visibility.Collapsed;

            return visibility;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
