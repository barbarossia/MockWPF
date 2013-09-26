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
using System.Windows.Shapes;

namespace MockWPF.Views
{
    /// <summary>
    /// Interaction logic for SelectImportAssemblyView.xaml
    /// </summary>
    public partial class SelectImportAssemblyView : Window
    {
        public SelectImportAssemblyView()
        {
            InitializeComponent();
            Closing += SelectImportAssemblyView_Closing;
        }

        private void SelectImportAssemblyView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var vm = this.DataContext as SelectImportAssemblyViewModel;
            if (vm != null)
            {
                vm.Cleanup();
            }
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as SelectImportAssemblyViewModel;
            if (vm != null && vm.ImportCommand != null)
            {
                vm.ImportCommand.Execute();
                if (vm.ImportResult)
                    this.Close();
            }
        }

        private ActivityItemViewModel ViewModel
        {
            get { return Resources["ViewModel"] as ActivityItemViewModel; }
        }

        private void uxItemCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var vm = this.DataContext as SelectImportAssemblyViewModel;
            if (vm != null)
            {
                vm.ChangeDefaultCategory();
            }
        }

        private void HeaderCheckBox_PreviewMouseDown(object sender, MouseEventArgs e)
        {
            var vm = this.DataContext as SelectImportAssemblyViewModel;
            if (vm != null)
            {
                vm.HeaderCheckboxClicked();
                e.Handled = true;
            }
        }
    }
}
