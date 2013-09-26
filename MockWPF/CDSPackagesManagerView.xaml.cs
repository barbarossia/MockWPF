using MockWPF.CDS;
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

namespace MockWPF
{
    /// <summary>
    /// Interaction logic for CDSPackagesManagerView.xaml
    /// </summary>
    public partial class CDSPackagesManagerView : Window
    {
        public CDSPackagesManagerViewModel vm { get; private set; }
        public CDSPackagesManagerView()
        {
            InitializeComponent();
        }

        private void Install_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var package = button.Tag as CDSPackage;
            var viewModel = DataContext as CDSPackagesManagerViewModel;
            viewModel.Install(package);
            e.Handled = true;

        }
    }
}
