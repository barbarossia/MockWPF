using AddIn;
using MockWPF.CDS;
using MockWPF.Common;
using NuGet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MockWPF
{
    public class CDSPackagesManagerViewModel : ViewModelBase
    {
        private string searchText;
        private List<CDSPackage> packages;
        private CDSPackage selectedPackage;
        private int pageSize = 15;      // The page size for the result set.
        private DataPagingViewModel dpViewModel;

        public string SearchText
        {
            get
            {
                return searchText;
            }
            set
            {
                searchText = value;
                RaisePropertyChanged(() => SearchText);
            }
        }

        public List<CDSPackage> Packages
        {
            get
            {
                return packages;
            }
            set
            {
                packages = value;
                RaisePropertyChanged(() => Packages);
            }
        }

        public CDSPackage SelectedPackage
        {
            get
            {
                return selectedPackage;
            }
            set
            {
                selectedPackage = value;
                RaisePropertyChanged(() => SelectedPackage);
            }
        }

        public DataPagingViewModel DataPagingVM
        {
            get { return this.dpViewModel; }
            set
            {
                this.dpViewModel = value;
                RaisePropertyChanged(() => DataPagingVM);
            }
        }

        public CDSPackagesManagerViewModel()
        {
            this.DataPagingVM = new DataPagingViewModel();
            this.DataPagingVM.SearchExecute = LoadData;
            this.DataPagingVM.PageSize = pageSize;
            LoadData();
        }

        private void LoadData()
        {
            LoadLiveData();
        }

        private void LoadLiveData()
        {
            int startIndex = this.DataPagingVM.ResetPageIndex ? 1 : this.DataPagingVM.PageIndex;
            //var result = CDSService.SearchUpdate(0, pageSize, CDSSortByType.NameAscending, SearchText, true);
            var result = CDSService.SearchOnline(startIndex - 1, pageSize, "Package Source", CDSSortByType.NameAscending, SearchText, false);
            this.DataPagingVM.ResultsLength = result.TotalCount;
            Packages = result.Select(p => new CDSPackage(p)).ToList();
            this.SelectedPackage = Packages.First();
        }

        public bool Install(CDSPackage package)
        {
            return CDSService.Install("Package Source", package.Id, package.Version);
            //return CDSService.Update(package.Id, package.Version);
        }

    }
}
