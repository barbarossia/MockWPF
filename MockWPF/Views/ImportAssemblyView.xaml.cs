using AddIn;
using MockWPF.Common.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for ImportAssemblyView.xaml
    /// </summary>
    public partial class ImportAssemblyView : Window, INotifyPropertyChanged
    {
        const string IssuesCountTooltipFormat = "{0} items with issues.";
        public ImportAssemblyView()
        {
            InitializeComponent();
            DataContextChanged += new DependencyPropertyChangedEventHandler(ImportAssemblyView_DataContextChanged);
            Loaded += (s, e) =>
            {
                //verify library metadata
                var errorConverter = (ErrorSectionConverter)Resources["ErrorSectionConverter"];
                CanImport = this.IsValid && !errorConverter.HasErrors && HasNoIssues;
            };
        }

        private bool libraryHasError;
        private void ImportAssemblyView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //Verify activity metadata
            if (null == DataContext) return;
            var context = (ImportAssemblyViewModel)DataContext;

            //verify library metadata
            var errorConverter = (ErrorSectionConverter)Resources["ErrorSectionConverter"];

            if (null != context)
                errorConverter.ViewModelBaseDerivedType = context.SelectedActivityAssemblyItem;
            errorConverter.PropertyChanged += (s, e1) =>
            {
                if (e1.PropertyName == "HasErrors")
                {
                    libraryHasError = errorConverter.HasErrors;
                    CanImport = this.IsValid && !errorConverter.HasErrors && HasNoIssues;
                }
            };

            //verfiy dependencies
            UpdateBoundProperties();
        }

        /// <summary>
        /// Notifies the front end that items it has bound to here have changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        //private bool isValid = false;
        private bool isValid = true;
        private ActivityItem selectedActivityItem;

        /// <summary>
        /// Is the data for this page in a valid state?
        /// </summary>
        public virtual bool IsValid
        {
            get { return isValid; }
            set
            {
                isValid = value;
                PropertyChanged(this, new PropertyChangedEventArgs("IsValid"));
                CanImport = this.IsValid && !libraryHasError && HasNoIssues;
            }
        }

        /// <summary>
        /// This is the currently selected item in the listbox.
        /// </summary>
        public ActivityItem SelectedActivityItem
        {
            get { return selectedActivityItem; }
            set
            {
                var errorConverter = (ErrorSectionConverter)Resources["ErrorSectionConverterActivity"];

                selectedActivityItem = value;

                if (null != selectedActivityItem)
                {
                    selectedActivityItem.IsReviewed = true;
                    errorConverter.ViewModelBaseDerivedType = selectedActivityItem;
                    selectedActivityItem.Validate();
                    libraryHasError = errorConverter.HasErrors;
                }

                PropertyChanged(this, new PropertyChangedEventArgs("SelectedActivityItem"));
                CanImport = this.IsValid && !libraryHasError && HasNoIssues;
            }
        }

        private bool IsCurrentItemValid()
        {
            bool result = true;

            if (null != selectedActivityItem)
            {
                selectedActivityItem.Validate();
                result = selectedActivityItem.IsValid;
            }

            return result;
        }

        private bool canImport;
        public bool CanImport
        {
            get { return this.canImport; }
            set
            {
                this.canImport = value;
                PropertyChanged(this, new PropertyChangedEventArgs("CanImport"));
            }
        }

        #region dependencies
        /// <summary>
        /// Returns a string describing the number of items having issues.
        /// </summary>
        public string ItemsWithIssuesMessage { get { return string.Format(IssuesCountTooltipFormat, ItemsWithIssuesCount); } }

        /// <summary>
        /// Returns the number of items that do not have the location set correctly. Intended to be bound to on the front end. 
        /// </summary>
        public int ItemsWithIssuesCount
        {
            get
            {
                var viewModel = DataContext as ImportAssemblyViewModel;
                var result = 0;

                if ((null != viewModel) && (null != viewModel.AssembliesToImport))
                    result = viewModel.AssembliesToImport
                                .Where(assembly => string.IsNullOrEmpty(assembly.Location))
                                .Count();

                return result;
            }
        }

        /// <summary>
        /// Visible if there are items that do not have locations set properly, Collapsed if all locations are set.
        /// </summary>
        public Visibility ItemsWithIssuesVisibility { get { return HasNoIssues ? Visibility.Collapsed : Visibility.Visible; } }

        #endregion

        private void UpdateBoundProperties()
        {
            PropertyChanged(this, new PropertyChangedEventArgs("ItemsWithIssuesMessage"));
            PropertyChanged(this, new PropertyChangedEventArgs("ItemsWithIssuesCount"));
            PropertyChanged(this, new PropertyChangedEventArgs("ItemsWithIssuesVisibility"));
            PropertyChanged(this, new PropertyChangedEventArgs("HasNoIssues"));
            CanImport = this.IsValid && !libraryHasError && HasNoIssues;
        }

        /// <summary>
        /// True if the data on this page is valid.
        /// </summary>
        public bool HasNoIssues { get { return ItemsWithIssuesCount == 0; } }

        private void PickLocationButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var activityAssemblyItem = button.Tag as ActivityAssemblyItemViewModel;
            var viewModel = DataContext as ImportAssemblyViewModel;
            string assemblyFileName = DialogService.ShowOpenFileDialogAndReturnResult("Assembly files (*.dll)|*.dll", "Open Assembly File");

            if (!string.IsNullOrEmpty(assemblyFileName)) // if user didn't cancel
                viewModel.UpdateReferenceLocation(activityAssemblyItem, assemblyFileName);

            e.Handled = true;

            UpdateBoundProperties();
        }

        private void listLibrary_Selected(object sender, RoutedEventArgs e)
        {
            this.libraryMetaDataPanel.Visibility = System.Windows.Visibility.Visible;
            this.activitiesMetaDataPanel.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void importBtn_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as ImportAssemblyViewModel;
            viewModel.ImportCommand.Execute();
            if (viewModel.ImportResult == true)
            {
                this.DialogResult = true;
                this.Close();
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Verify activity metadata
            if (null == DataContext) return;

            var errorConverter = (ErrorSectionConverter)Resources["ErrorSectionConverterActivity"];
            var context = (ImportAssemblyViewModel)DataContext;
            var activityAssemblyItem = context.SelectedActivityAssemblyItem;

            if ((null != activityAssemblyItem) && activityAssemblyItem.ActivityItems.Count > 0)
                SelectedActivityItem = activityAssemblyItem.ActivityItems[0];

            activityAssemblyItem
                .ActivityItems
                .ToList()
                .ForEach(item =>
                {
                    if (string.IsNullOrEmpty(item.Category))
                        item.Category = context.SelectedCategory;
                    item.IsDirty = false;

                }); //TODO put this in the viewmodel once the refactoring task is complete - v-richt 2/13/2012

            errorConverter.ViewModelBaseDerivedType = context.SelectedActivityAssemblyItem;
            CanImport = this.IsValid && !errorConverter.HasErrors && HasNoIssues;
        }

        private void Border_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (this.activitiesMetaDataPanel.Visibility != System.Windows.Visibility.Visible)
            {
                this.libraryMetaDataPanel.Visibility = System.Windows.Visibility.Collapsed;
                this.activitiesMetaDataPanel.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void listAssemblies_Selected(object sender, RoutedEventArgs e)
        {
            this.libraryMetaDataPanel.Visibility = System.Windows.Visibility.Visible;
            this.activitiesMetaDataPanel.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void browseBtn_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as ImportAssemblyViewModel;
            viewModel.BrowseCommand.Execute();
        }
    }
}
