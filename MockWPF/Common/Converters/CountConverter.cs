// -----------------------------------------------------------------------
// <copyright file="CountConverter.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace MockWPF.Common.Converters
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Data;

    /// <summary>
    /// For an object that can be counted (an array, an IEnumerable, etc), return the number of objects in that set.
    /// If the object can't produce a count, return the value set in ZeroOrNullDisplayValue.
    /// </summary>
    public class CountConverter : IValueConverter
    {

        private const string DefaultZeroOrNullDisplayValue = "0";
        /// <summary>
        /// Some areas of the code display other values besides a blank or 0 if there is no count or the count is 0.
        /// This sets the value that will be displayed in the case of 0 or null.
        /// </summary>
        public string ZeroOrNullDisplayValue { get; set; }

        /// <summary>
        /// For binding in consumers of this type. Allows binding to the ZeroOrNullDisplayValue property through this dependency property.
        /// </summary>
        public static readonly DependencyProperty ZeroOrNullDisplayValueProperty =
            DependencyProperty.RegisterAttached(
                                                    "ZeroOrNullDisplayValue",
                                                    typeof(string),
                                                    typeof(CountConverter),
                                                    new PropertyMetadata(DefaultZeroOrNullDisplayValue)
                                               );



        /// <summary>
        /// Converts a countable sequence (which means it has a method or property named "Count", or a property named "Length")
        /// into a count of that sequence. If the result is zero or null, return the value of ZeroOrNullDisplayValue, which defaults to 
        /// "0" but can be overriden by the consumer (for instance, to say "None")
        /// </summary>
        /// <param name="value">The value to be converted.</param>
        /// <param name="targetType">Not used.</param>
        /// <param name="parameter">Not used.</param>
        /// <param name="culture">Not used.</param>
        /// <returns>An object containing either the count, or the zero/null value.</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            object result = ZeroOrNullDisplayValue;
            var methods = value.GetType().GetMethods();
            var properties = value.GetType().GetProperties();
            dynamic countable = value;

            if (properties.Any(property => property.Name == "Count"))
                result = countable.Count;
            else if (methods.Any(property => property.Name == "Count"))
                result = countable.Count();
            else if (properties.Any(property => property.Name == "Length"))
                result = countable.Length;

            if (result.ToString() == "0")
                result = ZeroOrNullDisplayValue;

            return result;
        }


        /// <summary>
        /// Converts back from an integer count to the original type. Not used/Not implemented. This is here to satisfy the IValueConverter
        /// Interface.
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
