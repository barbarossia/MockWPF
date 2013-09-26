using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MockWPF.UIControls
{
    /// <summary>
    /// Interaction logic for WorkflowDataPaging.xaml
    /// </summary>
    public partial class WorkflowDataPaging : UserControl
    {
        private const int DEFAULTPAGESIZE = 15;//default pagesize

        public WorkflowDataPaging()
        {
            InitializeComponent();
        }
        private string trace;
        private void PageIndex_KeyDown(object sender, KeyEventArgs e)
        {
            DataPagingViewModel vm = this.DataContext as DataPagingViewModel;
            if (vm != null && e.Key == Key.Enter && (!string.IsNullOrEmpty((sender as TextBox).Text)))
            {
                int pageIndex = System.Convert.ToInt32((sender as TextBox).Text);
                if (pageIndex > 0 && vm.TotalPages != 0 && pageIndex <= vm.TotalPages)
                {
                    trace += "invoke ExecutePaging\r\n";
                    vm.ExecutePaging();
                    e.Handled = true;
                }
            }
        }
    }
}
