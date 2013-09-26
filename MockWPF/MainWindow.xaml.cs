using AddIn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MockWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void CloseTabButton_Click(object sender, RoutedEventArgs e)
        {
            DesignerHostAdapters itemToClose;
            if (sender != null)
            {
                itemToClose = ((Button)sender).Tag as DesignerHostAdapters;

                //if (DataContext != null)
                //{
                //    ((MainWindowViewModel)DataContext).CloseWorkflowCommandExecute(itemToClose);
                //}
            }
        }
    }
}
