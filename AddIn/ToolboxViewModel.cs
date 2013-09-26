using Microsoft.Practices.Prism.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace AddIn
{
    public class ToolboxViewModel : NotificationObject
    {
        private ObservableCollection<ActivityAssemblyItem> items;
        public ObservableCollection<ActivityAssemblyItem> ActivityAssemblyItems
        {
            get { return this.items; }
            set
            {
                this.items = value;
                RaisePropertyChanged(() => ActivityAssemblyItems);
            }
        }

        public ToolboxViewModel()
        {
            ActivityAssemblyItems = new ObservableCollection<ActivityAssemblyItem>(Caching.ActivityAssemblyItems);
        }
    }
}
