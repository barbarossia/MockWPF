using System;
using System.Activities.Presentation.Toolbox;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace AddIn.Converters
{
    [ValueConversion(typeof(ObservableCollection<ActivityAssemblyItem>), typeof(ObservableCollection<ToolboxControl>))]
    public class ActivityAssemblyItemsToToolboxWrappersConverter : IValueConverter
    {
        /// <summary>
        /// Convert an ObservableCollection of ActivityAssemblyItems into an ObservableCollection of ToolboxWrappers
        /// and link them so that inserting/deleting ActivityAssemblyItems regenerates the ToolboxWrapper collection.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //Contract.Requires(values[0] is ObservableCollection<ActivityAssemblyItem>);
            //Contract.Requires(values[1] is bool);

            var items = (ObservableCollection<ActivityAssemblyItem>)value;
            var controls = new ObservableCollection<ToolboxControl>(new[] { ToolboxControlService.CreateToolboxes(items) });
            return controls;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
