using AddIn;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MockWPF
{
    public class DataPagingViewModel : ViewModelBase
    {
        private int pageSize;
        private bool resetPageIndex = false;
        private int totalResults;
        private int pageIndex = 1;
        private int totalPages;

        public DelegateCommand FirstPageCommand { get; set; }
        public DelegateCommand PreviousPageCommand { get; set; }
        public DelegateCommand NextPageCommand { get; set; }
        public DelegateCommand LastPageCommand { get; set; }
        public Action SearchExecute { get; set; }

        /// <summary>
        /// Gets o sets PageSize
        /// </summary>
        public int PageSize
        {
            get { return pageSize; }
            set
            {
                pageSize = value;
                RaisePropertyChanged(() => PageSize);
            }
        }

        /// <summary>
        /// Gets or sets ResetPageIndex
        /// </summary>
        public bool ResetPageIndex
        {
            get { return resetPageIndex; }
            set
            {
                resetPageIndex = value;
                RaisePropertyChanged(() => ResetPageIndex);
            }
        }

        /// <summary>
        /// Current page
        /// </summary>
        public int PageIndex
        {
            get
            {
                return pageIndex;
            }
            set
            {
                pageIndex = value;
                RaisePropertyChanged(() => PageIndex);
            }
        }


        /// <summary>
        /// Current page
        /// </summary>
        public int TotalPages
        {
            get
            {
                return totalPages;
            }
            set
            {
                totalPages = value;
                CheckPagingStatus();
                RaisePropertyChanged(() => TotalPages);
            }
        }

        /// <summary>
        /// Results length
        /// </summary>
        public int ResultsLength
        {
            get { return this.totalResults; }
            set
            {

                this.totalResults = value;
                CalculatePages();
                RaisePropertyChanged(() => ResultsLength);
            }
        }

        public DataPagingViewModel()
        {
            FirstPageCommand = new DelegateCommand(delegate
            {
                PageIndex = 1;
                ExecutePaging();
            },
                () => PageIndex > 1);
            PreviousPageCommand = new DelegateCommand(delegate
            {
                PageIndex--;
                ExecutePaging();
            }, () => PageIndex > 1);
            NextPageCommand = new DelegateCommand(delegate
            {
                PageIndex++;
                ExecutePaging();
            }, () => PageIndex < TotalPages);
            LastPageCommand = new DelegateCommand(delegate
            {
                PageIndex = TotalPages;
                ExecutePaging();
            }, () => PageIndex < TotalPages);
        }

        public void ExecutePaging()
        {
            if (pageIndex > 0 && TotalPages != 0 && pageIndex <= TotalPages)
            {
                if (SearchExecute != null)
                    SearchExecute();
                CheckPagingStatus();
            }
        }

        public void CheckPagingStatus()
        {
            FirstPageCommand.RaiseCanExecuteChanged();
            PreviousPageCommand.RaiseCanExecuteChanged();
            NextPageCommand.RaiseCanExecuteChanged();
            LastPageCommand.RaiseCanExecuteChanged();
        }

        public void CalculatePages()
        {
            TotalPages = (totalResults / PageSize) + (totalResults % PageSize > 0 ? 1 : 0);
            if (totalResults == 0)
            {
                this.PageIndex = 0;
            }

            if ((resetPageIndex) && (totalResults > 0))
            {
                this.PageIndex = 1;
                resetPageIndex = false;
            }
        }
    }
}
