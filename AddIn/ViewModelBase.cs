using Microsoft.Practices.Prism.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AddIn
{
    [Serializable]
    public class ViewModelBase : NotificationObject
    {
        bool isValid = true;           // Are all the rules for this viewmodel passed?
        bool isDirty = false;          // Has data been changed in this object?
        private string errorMessage;   // Error message exposed to a consumer of this viewmodel.
        private bool isBusy;           // Is this viewmodel busy?
        private string busyCaption;    // Caption to show when IsBusy is set.


        /// <summary>
        /// Is the data in this ViewModel in a valid state?
        /// </summary>
        public virtual bool IsValid
        {
            get { return isValid; }
            set
            {
                isValid = value;
                RaisePropertyChanged(() => IsValid);
            }
        }

        /// <summary>
        /// Has data been changed in this object?
        /// </summary>
        public bool IsDirty
        {
            get { return isDirty; }
            set
            {
                isDirty = value;
                RaisePropertyChanged(() => IsDirty);
            }
        }

        /// <summary>
        /// String intended to be shown to the user in the UX, indicating any human-readable error messages from the last operation.
        /// </summary>
        public string ErrorMessage
        {
            get { return errorMessage; }
            set
            {
                errorMessage = value;
                RaisePropertyChanged(() => ErrorMessage);
                RaisePropertyChanged(() => ErrorSectionsDictionary);
            }
        }

        /// <summary>
        /// List of individual fields and their associated error messages.
        /// </summary>
        public Dictionary<string, string> ErrorSectionsDictionary { get; private set; }

        public ViewModelBase()
        {
            ErrorSectionsDictionary = new Dictionary<string, string>();
        }

        /// <summary>
        /// Gets or sets a value indicating whether view model is busy. If true, a busy indicator should be shown in the UX layer.
        /// </summary>
        public bool IsBusy
        {
            get { return isBusy; }
            set
            {
                isBusy = value;
                RaisePropertyChanged(() => IsBusy);
            }
        }

        /// <summary>
        /// Caption to show in the busy indicator while this viewmodel is processing the current task.
        /// </summary>
        public string BusyCaption
        {
            get { return busyCaption; }

            set
            {
                busyCaption = value;
                RaisePropertyChanged(() => BusyCaption);
            }
        }


        /// <summary>
        /// Validate the data in this ViewModel and set the IsValid and ErrorMessage properties as appropriate.
        /// </summary>
        public virtual void Validate()
        {
            ErrorMessage = String.Empty;

            ErrorSectionsDictionary.Clear();

            GetType()
                .GetProperties()
                .ToList()
                .ForEach(property =>
                            property
                               .GetCustomAttributes(typeof(ValidationAttribute), true)
                               .OfType<ValidationAttribute>()
                               .ToList()
                               .ForEach(validator =>
                               {
                                   if (!validator.IsValid(property.GetValue(this, null)))
                                   {
                                       var newMessage = validator.FormatErrorMessage(GetDisplayName(property));

                                       ErrorMessage += "\r\n" + newMessage;

                                       if (ErrorSectionsDictionary.ContainsKey(property.Name))
                                           ErrorSectionsDictionary[property.Name] += newMessage;
                                       else
                                           ErrorSectionsDictionary.Add(property.Name, newMessage);

                                   }

                                   RaisePropertyChanged(property.Name);
                               }));

            ErrorMessage = ErrorMessage.Trim();

            IsValid = (ErrorMessage.Length == 0);
        }

        /// <summary>
        /// Get the display name for a property, using the DisplayName property if available,
        /// or the name of the property if it is not.
        /// </summary>
        /// <param name="property">The property needing a friendly name.</param>
        /// <returns>A human readable name for the property, if possible.</returns>
        private string GetDisplayName(PropertyInfo property)
        {
            var result = property.Name;
            var attribute = property
                               .GetCustomAttributes(typeof(DisplayNameAttribute), true)
                               .OfType<DisplayNameAttribute>()
                               .FirstOrDefault();

            if (null != attribute)
                result = attribute.DisplayName;

            return result;
        }

    }
}
