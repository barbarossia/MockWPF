// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BooleanToVisibilityConverter.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation 2011.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace MockWPF.Common.Converters
{
    using System;
    using System.Globalization;
    using System.Threading;
    using System.Windows;
    using System.Windows.Data;
    using System.Security;

    /// <summary>
    /// False = Visible, True = Hidden or Collapsed (depends on CollapseWhenInvisible)
    /// </summary>
    public class BooleanToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// The convert. Input a boolean value indicates if data source is Read Only. If true, output value is  Visibility.Collapsed.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="targetType">
        /// The target type.
        /// </param>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <param name="culture">
        /// The culture.
        /// </param>
        /// <returns>
        /// The converted value.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var v = (bool)(value ?? false);

            if (targetType == typeof(Visibility))
            {
                Visibility result;
                if (v == VisibleWhen)
                {
                    result = Visibility.Visible;
                }
                else
                {
                    result = CollapseWhenInvisible ? Visibility.Collapsed : Visibility.Hidden;
                }
                return result;
            }
            else if (targetType == typeof(double))
            {
                if (v == VisibleWhen)
                {
                    return 1.00D;
                }
                else
                {
                    return PartialHideWhenInvisible ? 0.40D : 0.00D;
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public bool CollapseWhenInvisible { get; set; }
        public bool PartialHideWhenInvisible { get; set; }
        public bool VisibleWhen { get; set; }

        /// <summary>
        /// The convert back. Never be called.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="targetType">
        /// The target type.
        /// </param>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <param name="culture">
        /// The culture.
        /// </param>
        /// <returns>
        /// The convert back value.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}