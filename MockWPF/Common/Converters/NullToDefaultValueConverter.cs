// -----------------------------------------------------------------------
// <copyright file="NullToDefaultValueConverter.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace MockWPF.Common.Converters
{
    using System;
    using System.Windows;
    using System.Windows.Data;

    /// <summary>
    ///  Converts null or empty string values to a default value.
    /// </summary>
    public class NullToDefaultValueConverter : IValueConverter
    {

        /// <summary>
        /// The default value to be displayed if the value to be converted is null or an empty string.
        /// </summary>
        public string DefaultValue { get; set; }

        /// <summary>
        /// For binding in consumers of this type. Allows binding to the DefaultValue property through this dependency property.
        /// </summary>
        public static readonly DependencyProperty DefaultValueProperty =
            DependencyProperty.RegisterAttached(
                                                    "DefaultValue",
                                                    typeof(string),
                                                    typeof(NullToDefaultValueConverter),
                                                    new PropertyMetadata(String.Empty)
                                               );



        /// <summary>
        /// If the object is null or am empty string, replace it with DefaultValue. Otherwise return it verbatim.
        /// </summary>
        /// <param name="value">The value to be converted.</param>
        /// <param name="targetType">Not used.</param>
        /// <param name="parameter">Not used.</param>
        /// <param name="culture">Not used.</param>
        /// <returns>The value as passed in, or DefaultValue if it was null or an empty string.</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            object result = value;

            if (null == result)
                result = DefaultValue;

            if ((result is string) && (string.IsNullOrEmpty(result as string)))
                result = DefaultValue;

            return result;
        }

        /// <summary>
        /// Not used. This is here to satisfy the IValueConverter interface.
        /// </summary>
        /// <param name="value">Not used.</param>
        /// <param name="targetType">Not used.</param>
        /// <param name="parameter">Not used.</param>
        /// <param name="culture">Not used.</param>
        /// <returns>Not used.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
