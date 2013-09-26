using Microsoft.Practices.Prism.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MockWPF.CDS
{
    public class CDSRepository : NotificationObject
    {
        private bool isEnabled;
        private string name;
        private string source;

        public bool IsEnabled
        {
            get { return isEnabled; }
            set
            {
                isEnabled = value;
                RaisePropertyChanged(() => IsEnabled);
            }
        }
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                RaisePropertyChanged(() => Name);
            }
        }
        public string Source
        {
            get { return source; }
            set
            {
                source = value;
                RaisePropertyChanged(() => Source);
            }
        }

        public CDSRepository Clone()
        {
            return (CDSRepository)MemberwiseClone();
        }
    }
}
