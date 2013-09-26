// -----------------------------------------------------------------------
// <copyright file="LocationTypeToVisibilityConverter.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation 2012.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MockWPF.Common.Converters
{
    using System;
    using System.Windows;
    using System.Windows.Data;

    /// <summary>
    /// Converter to convert LocationType to Visibility. None == Visible, all else are Collapsed.
    /// </summary>
    public class LocationTypeToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// If true, inverts whether the result should be Collapsed or Visible.
        /// </summary>
        public bool Invert { get; set; }

        /// <summary>
        /// Converts a LocationType to visibility. If the location type is none, then we want to do things such as display buttons,
        /// allow certain functionality, etc
        /// </summary>
        /// <param name="value">The value to be converted.</param>
        /// <param name="targetType">Ignored.</param>
        /// <param name="parameter">Ignored.</param>
        /// <param name="culture">Ignored.</param>
        /// <returns>A Visibility object, depending on the vlaue of the LocationType passed in.</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var result = Visibility.Collapsed;

            if (value is LocationType)
                result = (LocationType)value == LocationType.None
                            ? Visibility.Visible
                            : Visibility.Collapsed;

            if (value is String)
                result = string.IsNullOrEmpty(value.ToString())
                            ? Visibility.Visible
                            : Visibility.Collapsed;

            if (Invert)
                if (result == Visibility.Visible)
                    result = Visibility.Collapsed;
                else
                    result = Visibility.Visible;


            return result;
        }


        /// <summary>
        /// ConvertBack is not implemented.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
