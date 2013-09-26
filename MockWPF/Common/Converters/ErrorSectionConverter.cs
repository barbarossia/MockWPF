// -----------------------------------------------------------------------
// <copyright file="ErrorSectionConverter.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace MockWPF.Common.Converters
{
    using AddIn;
    using Microsoft.Practices.Prism.ViewModel;
    using System;
    using System.Linq;
    using System.Windows.Data;

    /// <summary>
    /// Monitors a ViewModelBase-derived class for changes in its list of validation errors,
    /// and allows binding to specific errors in that collection.
    /// </summary>
    public class ErrorSectionConverter : NotificationObject, IValueConverter
    {

        ViewModelBase viewModelBaseDerivedType;
        bool hasErrors = true;

        /// <summary>
        /// This is the ViewModelBase-derived type that will have a dictionary of error strings by field.
        /// The client needs to set this property.
        /// </summary>
        public ViewModelBase ViewModelBaseDerivedType
        {
            get { return viewModelBaseDerivedType; }
            set
            {
                viewModelBaseDerivedType = value;
                viewModelBaseDerivedType.PropertyChanged += viewModelBaseDerivedType_PropertyChanged;
            }
        }

        void viewModelBaseDerivedType_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            HasErrors = !viewModelBaseDerivedType.IsValid;
        }

        /// <summary>
        /// True if the ViewModelBase-derived class we are monitoring has errors.
        /// </summary>
        public bool HasErrors
        {
            get { return hasErrors; }
            set
            {
                hasErrors = value;
                RaisePropertyChanged(() => HasErrors);
            }
        }


        /// <summary>
        /// For a ViewModelBase deriverd class, determine if a particular field has an error, and return the string of the error, if so. ViewModelBase 
        /// contains a dictionary that contains errors for any fields that don't pass validation.
        /// </summary>
        /// <param name="value">The ViewModelBase-derived class.</param>
        /// <param name="targetType">Not used.</param>
        /// <param name="parameter">
        /// The name of the field for which we want the error string.
        /// If this parameter contains a semicolon,
        ///  -- the first part (if split on the semicolon) is the field name.
        ///  -- the second part is a type to convert the result to.
        ///  
        /// The second part (case insensitive):
        ///   -- Integer: 0 if no error, 1 if error. This is for setting things like opacity on controls that should only
        ///      be visible in the case of an error, such as a red border.
        ///   
        /// </param>
        /// <param name="culture">Not used.</param>
        /// <returns>The error string produced by the Validate() function in the ViewModelBase-derived class,
        /// for the field passed in the 'parameter' parameter.</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            object result = String.Empty;

            if (null != ViewModelBaseDerivedType)
            {
                var errorDictionary = ViewModelBaseDerivedType.ErrorSectionsDictionary;
                var parameterSections = (parameter ?? String.Empty).ToString().Split(new[] { ';' });

                var key = parameterSections[0];

                if (errorDictionary.ContainsKey(key))
                    result = errorDictionary[key];

                if (parameterSections.Count() > 1)
                    switch (parameterSections[1].ToLower())
                    {
                        case "integer":
                            int trueValue = 1;
                            int falseValue = 0;

                            if (parameterSections.Count() >= 3)
                            {
                                int.TryParse(parameterSections[2], out trueValue);
                                int.TryParse(parameterSections[3], out falseValue);
                            }

                            result = result.ToString().Length > 0 ? trueValue : falseValue;
                            break;
                    }

            }

            return result;
        }


        /// <summary>
        /// This is not used and exists to satisfy the IValueConverter interface.
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
