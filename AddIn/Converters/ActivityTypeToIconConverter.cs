using System;
using System.Activities.Statements;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace AddIn.Converters
{
    public class ActivityTypeToIconConverter : IValueConverter
    {
        readonly ResourceDictionary dict = new ResourceDictionary
        {
            Source =
                new Uri("pack://application:,,,/System.Activities.Presentation;V4.0.0.0;31bf3856ad364e35;component/themes/icons.xaml")
        };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //Contract.Requires(value is Type); // Should be passing in toolboxWrapper.Type
            var activityType = (Type)value;
            return LookupBuiltinIcon(activityType);
        }

        /// <summary>
        /// Icons for built-in activities like Sequence and Send/Receive
        /// </summary>
        /// <param name="activityType"></param>
        /// <returns></returns>
        Drawing LookupBuiltinIcon(Type activityType)
        {
            string resourceName = GetIconName(activityType);


            // Lookup by icon name
            DrawingBrush icon = dict[resourceName] as DrawingBrush;

            if (icon != null && icon.Drawing != null)
                return icon.Drawing;
            else
                return GetDefaultIcon();
        }

        Drawing GetDefaultIcon()
        {
            DrawingBrush defaultIcon = dict["GenericLeafActivityIcon"] as DrawingBrush;

            return defaultIcon.IfNotNull(i => i.Drawing);
        }

        string GetIconName(Type activityType)
        {
            // For the most part, icons are the name of the activity + "Icon"
            if (activityType == typeof(Flowchart)) // special-case Flowchart because for some reason its icon has weird casing
            {
                return "FlowChartIcon";
            }
            else
            {
                if (!activityType.IsGenericType)
                {
                    return string.Concat(activityType.Name, "Icon");
                }
                else
                {
                    // strip off the generic part
                    string name = activityType.Name;
                    int genericBackquoteIndex = name.IndexOf('`');
                    return string.Concat(genericBackquoteIndex > 0 ? name.Substring(0, genericBackquoteIndex) : name, "Icon");
                    // >0 because we don't want Substring to throw if the typename somehow starts with a `, 
                    // although I think this is impossible in .NET
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
