using AddIn;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace MockWPF
{
    public class ActivityItemViewModel : ViewModelBase
    {
        private string selectedStatus;
        private string selectedCategory;

        public string SelectedStatus
        {
            get { return selectedStatus; }
            set
            {
                selectedStatus = value;
                RaisePropertyChanged(() => SelectedStatus);
            }
        }

        public ICollectionView Categories { get { return AssetStoreProxy.ActivityCategories.View; } }

        public string SelectedCategory
        {
            get { return selectedCategory; }
            set
            {
                selectedCategory = value;
                RaisePropertyChanged(() => SelectedCategory);
            }
        }

    }
}
