using AddIn;
using AddIn.Common.ExceptionHandling;
using AddIn.Common.Message;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace MockWPF
{
    public class ImportAssemblyViewModel : ViewModelBase
    {
        private const int MaxCategoryNameLength = 50;     // Maximum length of the category names.
        private const string CategoryNamePattern = "^[a-zA-Z0-9 ]+$";

        private HashSet<string> resolvedAssemblies = new HashSet<string>();  // The list of locations the user has resolved.
        private string selectedCategory = String.Empty;                      // Selected category (default) for the assemblies to import
        private bool isAddingCategory;                                       // Flag to indicate if we are in a state of adding a new category.
        private string newCategoryName;                                      // New category name that will be inserted if the AddNewCategoryCommand executes.
        private ActivityAssemblyItemViewModel selectedActivityAssemblyItem;  // Selected assembly item in the collection.
        private ObservableCollection<ActivityAssemblyItemViewModel> assembliesToImport;  // Collection of assemblies that will be imported.
        private ObservableCollection<ActivityAssemblyItemViewModel> selectedActivityAssemblyItems;  // Collection of assemblies that will be imported.

        /// <summary>
        /// Gets or sets the command that changes the state of the viewmodel to indicate that the user wants to add a category.
        /// </summary>
        public DelegateCommand StartAddingCategoryCommand { get; private set; }

        /// <summary>
        /// Gets or sets the command that changes the state of the viewmodel to the default value.
        /// </summary>
        public DelegateCommand StopAddingCategoryCommand { get; private set; }

        /// <summary>
        /// Gets or sets the command that starts the logic for importing the selected assembly.
        /// </summary>
        public DelegateCommand ImportCommand { get; private set; }

        public DelegateCommand BrowseCommand { get; private set; }

        /// <summary>
        /// Gets or sets the command that adds a new category of assemblies to the asset store.
        /// </summary>
        public DelegateCommand AddNewCategoryCommand { get; private set; }

        /// <summary>
        /// Gets or sets the name of the new category that the user is creating.
        /// </summary>
        public string NewCategoryName
        {
            get { return newCategoryName; }
            set
            {
                newCategoryName = (value ?? String.Empty);
                RaisePropertyChanged(() => NewCategoryName);
                AddNewCategoryCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Gets or Sets a value that indicates if the user is adding a category.
        /// </summary>
        public bool IsAddingCategory
        {
            get { return isAddingCategory; }
            set
            {
                isAddingCategory = value;
                RaisePropertyChanged(() => IsAddingCategory);
            }
        }

        /// <summary>
        /// Gets the list of activity categories.
        /// </summary>
        public ICollectionView ActivityCategories
        {
            get
            {
                AssetStoreProxy proxy = new AssetStoreProxy();
                return proxy.NewActivityCategories.View;
            }
        }

        /// <summary>
        /// Gets or Sets the default category for the assemblies and activities in the import process.
        /// </summary>
        public string SelectedCategory
        {
            get { return selectedCategory; }
            set
            {
                selectedCategory = value;
                RaisePropertyChanged(() => SelectedCategory);
                ChangeDefaultCategory(value);
            }
        }

        /// <summary>
        /// Collection of assembly items that will be imported.
        /// </summary>
        public ObservableCollection<ActivityAssemblyItemViewModel> AssembliesToImport
        {
            get
            {
                assembliesToImport = assembliesToImport ?? new ObservableCollection<ActivityAssemblyItemViewModel>();
                return assembliesToImport;
            }
        }

        /// <summary>
        /// Selected assembly items.
        /// </summary>
        public ObservableCollection<ActivityAssemblyItemViewModel> SelectedActivityAssemblyItems
        {
            get
            {
                if (selectedActivityAssemblyItems == null)
                {
                    selectedActivityAssemblyItems = new ObservableCollection<ActivityAssemblyItemViewModel>();
                }

                return selectedActivityAssemblyItems;
            }
            set
            {
                selectedActivityAssemblyItems = value;
                RaisePropertyChanged(() => SelectedActivityAssemblyItems);
            }
        }

        /// <summary>
        /// Currently selected assembly item.
        /// </summary>
        public ActivityAssemblyItemViewModel SelectedActivityAssemblyItem
        {
            get { return selectedActivityAssemblyItem; }
            set
            {
                selectedActivityAssemblyItem = value;
                RaisePropertyChanged(() => SelectedActivityAssemblyItem);
            }
        }

        /// <summary>
        /// Indicates the result of the import operation 
        /// </summary>
        public bool ImportResult { get; private set; }

        /// <summary>
        /// Stores the exception produced during the import operation if this operation was not successful.
        /// </summary>
        public Exception ImportResultException { get; private set; }

        public WorkflowItem FocusedWorkflowItem { get; set; }

        /// <summary>
        /// Default constructor for the class
        /// </summary>
        public ImportAssemblyViewModel(string assemblyFilePath)
            : this(new string[] { assemblyFilePath })
        {
        }

        public ImportAssemblyViewModel(string[] assemblyFilePaths)
            : this()
        {
            if (assemblyFilePaths == null)
            {
                throw new ArgumentNullException("assemblyFilePath");
            }
            //Utility.DoTaskWithBusyCaption("Loading assemblies...", () =>
            //{
            //    assemblyFilePaths.ToList().ForEach(path =>
            //        AssemblyInspectionService.CheckAssemblyPath(path));

            //    InspectAssemblies(assemblyFilePaths);
            //});

            assemblyFilePaths.ToList().ForEach(path =>
                AssemblyInspectionService.CheckAssemblyPath(path));

            InspectAssemblies(assemblyFilePaths);
        }

        public ImportAssemblyViewModel()
        {
            StartAddingCategoryCommand = new DelegateCommand(() => { IsAddingCategory = true; });
            StopAddingCategoryCommand = new DelegateCommand(() => { IsAddingCategory = false; });
            ImportCommand = new DelegateCommand(ImportCommandExecute, CanImportCommandExecute);
            BrowseCommand = new DelegateCommand(BrowseCommandExecute);
            AddNewCategoryCommand = new DelegateCommand(AddNewCategoryCommandExecute, CanAddNewCategoryCommandExecute);

            Initialize();
        }

        /// <summary>
        /// Overload for testability purposes
        /// </summary>
        private void Initialize()
        {
            SelectedActivityAssemblyItems.CollectionChanged += SelectedActivityAssemblyItemsCollectionChanged;
        }

        void SelectedActivityAssemblyItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (SelectedActivityAssemblyItem == null && SelectedActivityAssemblyItems.Count > 0)
                SelectedActivityAssemblyItem = SelectedActivityAssemblyItems[0];
        }

        /// <summary>
        /// Check if the import command can execute. 
        /// </summary>
        public bool CanImportCommandExecute()
        {
            // Import command can only execute if all activities have locations (i.e. versions/name match, and the tree is 
            // complete) and have been reviewed (i.e. user has entered category information, etc., for all 
            // activities that he cares about)
            return AssembliesToImport.Any() &&
                AssembliesToImport.All(assemblyItem => !string.IsNullOrEmpty(assemblyItem.Location) && assemblyItem.IsReviewed);
        }

        /// <summary>
        /// Check if the add new category command can execute.
        /// </summary>
        /// <returns></returns>
        public bool CanAddNewCategoryCommandExecute()
        {
            try
            {
                ValidateCategoryName();
                return true;
            }
            catch
            {
                //In case of errors, return false;
                return false;
            }
        }

        /// <summary>
        /// Execute the add new category logic.
        /// </summary>
        public void AddNewCategoryCommandExecute()
        {

            ValidateCategoryName();
            newCategoryName = newCategoryName.Trim();
            if (AssetStoreProxy.ActivityCategoryCreateOrUpdate(newCategoryName))
            {
                SelectedCategory = newCategoryName;
                NewCategoryName = String.Empty;
                IsAddingCategory = false;
            }
        }

        /// <summary>
        /// Execute the import command logic (Imports the assemblies).
        /// </summary>
        public void ImportCommandExecute()
        {

            if (!AssembliesToImport.Any())
            {
                throw new ArgumentException(ImportMessages.AssembliesToImportNull);
            }

            if (AssembliesToImport.Any(item => item.Location == null))
            {
                throw new ArgumentException(ImportMessages.AssemblyLocationNull);
            }

            if (AssembliesToImport.Any(item => !File.Exists(Path.GetFullPath(item.Location))))
            {
                throw new ArgumentException(ImportMessages.AssemblyLocationNull);
            }

            var assemblies = AssembliesToImport.Select(item => item.Source).ToList();

            try
            {
                //Caching.CacheAssembly(assemblies);
                //Caching.Refresh();
                ImportResult = true;
                ImportResultException = null;
            }
            catch (Exception ex)
            {
                ImportResult = false;
                ImportResultException = ex;
                throw new UserFacingException(ex.Message);
            }
        }

        /// <summary>
        /// Execute the import command logic (Imports the assemblies).
        /// </summary>
        public void BrowseCommandExecute()
        {
            string[] assemblyFileNames = DialogService.ShowOpenFileDialogAndReturnMultiResult("Assembly files (*.dll)|*.dll", "Open Assembly File");
            if (assemblyFileNames != null)
            {
                InspectAssemblies(assemblyFileNames);
            }
        }

        /// <summary>
        /// Check if the name of the new category is valid
        /// </summary>
        private void ValidateCategoryName()
        {
            if (string.IsNullOrEmpty(newCategoryName))
            {
                throw new ArgumentNullException(ImportMessages.CategoryNameNull);
            }

            if (newCategoryName.Length > MaxCategoryNameLength)
            {
                throw new ArgumentOutOfRangeException(ImportMessages.CategoryNameOutOfRange);
            }

            if (newCategoryName.Trim().Length == 0)
            {
                throw new ArgumentOutOfRangeException(ImportMessages.CategoryNameOutOfRange);
            }

            if (!Regex.IsMatch(newCategoryName, CategoryNamePattern))
            {
                throw new UserFacingException(ImportMessages.CategoryWithInvalidName);
            }
        }

        public void InspectAssembly(string assemblyPath)
        {
            var inspection = Utility.GetAssemblyInspectionService();

            if (!inspection.Inspect(assemblyPath))
            {
                throw new UserFacingException(inspection.OperationException.Message, inspection.OperationException);
            }

            if (AssembliesToImport.Any())
            {
                //Remove the assembly without location that we just analyzed
                AssembliesToImport.Remove(item => item.Name == inspection.SourceAssembly.Name &&
                                                  item.Version == inspection.SourceAssembly.Version.ToString() &&
                                                  string.IsNullOrEmpty(item.Location));
            }

            var newAssembly = new ActivityAssemblyItemViewModel(inspection.SourceAssembly);
            AssembliesToImport.Add(newAssembly);
            SelectedActivityAssemblyItems.Add(newAssembly);

            if (inspection.ReferencedAssemblies.Any())
            {
                //Check if we have already located the missing assemblies in the last reference inspection, and remove them.
                AssembliesToImport.Remove(item => string.IsNullOrEmpty(item.Location) &&
                                                  inspection.ReferencedAssemblies.Any(
                                                      element => element.Name == item.Name &&
                                                                 element.Version.ToString() == item.Version &&
                                                                 !string.IsNullOrEmpty(element.Location)));

                foreach (var item in inspection.ReferencedAssemblies)
                {
                    //Check to verify assembly is not already in the list
                    if (!AssembliesToImport.Any(element => element.Name == item.Name && element.Version == item.Version.ToString()) &&
                        !Caching.ActivityAssemblyItems.Any(caching => caching.Name == item.Name && caching.Version == item.Version))
                    {
                        var viewModel = new ActivityAssemblyItemViewModel(item);
                        AssembliesToImport.Add(viewModel);
                    }
                }
            }

            MarkResolvedAssembly(newAssembly);
        }

        private void InspectAssembliesWithBusy(string[] assemblyPaths)
        {
            //Utility.DoTaskWithBusyCaption("Loading...", () =>
            //{
            //    InspectAssemblies(assemblyPaths);
            //}, false);
            InspectAssemblies(assemblyPaths);
        }

        /// <summary>
        /// Starts the inspection of one assembly to get all its references and contained activities
        /// </summary>
        /// <param name="assemblyPath">Path of the assembly to import</param>
        public void InspectAssemblies(string[] assemblyPaths)
        {
            if (assemblyPaths == null || !assemblyPaths.Any())
            {
                throw new ArgumentNullException("assemblyPath");
            }

            foreach (var path in assemblyPaths)
            {
                AssemblyInspectionService.CheckAssemblyPath(path);
                var assemblyName = AssemblyName.GetAssemblyName(path);
                if (AssembliesToImport.Any(item => item.Name == assemblyName.Name
                                                   && item.Version == assemblyName.Version.ToString()
                                                   && !string.IsNullOrEmpty(item.Location)))
                {

                    continue;
                }

                InspectAssembly(path);
            }

            SortAssembliesToImport();
            if (!string.IsNullOrEmpty(this.SelectedCategory))
                ChangeDefaultCategory(this.SelectedCategory);
        }

        /// <summary>
        /// Updates the location of one of the Assemblies to be imported. 
        /// </summary>
        /// <param name="activityAssemblyItem"></param>
        /// <param name="assemblyPath"></param>
        public void UpdateReferenceLocation(ActivityAssemblyItemViewModel activityAssemblyItem, string assemblyPath)
        {
            if (string.IsNullOrEmpty(assemblyPath))
            {
                throw new ArgumentNullException("assemblyPath");
            }

            AssemblyInspectionService.CheckAssemblyPath(assemblyPath);

            // Validate assembly, inspect if its valid
            if (ValidateLocation(activityAssemblyItem, assemblyPath))
            {
                activityAssemblyItem.IsReviewed = false; //user will have to review it again
                InspectAssembly(assemblyPath);
                SortAssembliesToImport();
            }
        }

        /// <summary>
        /// Validates one assembly to check if a file path corresponds to one of our assembly items (name,version)
        /// </summary>
        /// <param name="activityAssemblyItem"></param>
        /// <param name="location"></param>
        /// <param name="throwOnFailure">true for user-facing operations which must give feedback about reasons for failure, false for programmatic operations that must be fast</param>
        /// <returns>True on success</returns>
        public static bool ValidateLocation(ActivityAssemblyItemViewModel activityAssemblyItem, string location, bool throwOnFailure = true)
        {
            AssemblyName name;

            if (null == activityAssemblyItem)
            {
                if (!throwOnFailure)
                    return false;
                throw new ArgumentNullException("activityAssemblyItem");
            }
            try
            {
                name = AssemblyName.GetAssemblyName(location);
            }
            catch
            {
                if (!throwOnFailure)
                    return false;
                throw new UserFacingException(string.Format(ImportMessages.NotADotNetAssembly, location));
            }

            AssemblyName assemblyName = ActivityAssemblyItem.TreatUnsignedAsUnversioned(name);

            if (assemblyName.Name != activityAssemblyItem.Name)
            {
                if (!throwOnFailure)
                    return false;
                throw new UserFacingException(string.Format("The file '{0}' does not contain the required assembly '{1}'", location, activityAssemblyItem.Name));
            }

            if (assemblyName.Version != activityAssemblyItem.AssemblyName.Version)
            {
                if (!throwOnFailure)
                    return false;
                // ideally this message would tell you WHO requires it
                throw new UserFacingException(string.Format("The file '{0}' is {1}, but {2} is required",
                    location, assemblyName.Version.IfNotNull(v => "v" + v.ToString()) ?? "unsigned",
                    activityAssemblyItem.AssemblyName.Version.IfNotNull(v => "v" + v.ToString()) ?? "an unsigned assembly"));
            }

            return true;
        }

        /// <summary>
        ///  Iterate through each of our current imports, and set the category to newCategory.
        ///  Iterate through all of the assemblies held by each if the entries in Imports, and set the 
        ///  Category property to newCategory. This has the effect of setting everything in the tree represented by
        ///  Imports to the new category.
        ///  This default category can be further overridden on the Review form for each assembly that is reviewable.
        /// </summary>
        /// <param name="category">The category to which we are going to change the imports</param>
        public void ChangeDefaultCategory(string category)
        {
            AssembliesToImport.ToList().ForEach(assembly =>
            {
                assembly.Category = category;
                assembly.ActivityItems.ToList().ForEach(item => item.Category = category);
            });
        }

        private void SortAssembliesToImport()
        {
            var assemblies = new List<ActivityAssemblyItemViewModel>(AssembliesToImport
                                                                     .OrderByDescending(item => item.Location == String.Empty)
                                                                     .ThenByDescending(item => item.IsResolved)
                                                                     .ThenBy(item => item.Name));

            AssembliesToImport.Clear();

            assemblies.ForEach(item => AssembliesToImport.Add(item));
        }

        private void MarkResolvedAssembly(ActivityAssemblyItemViewModel newAssembly)
        {
            if (newAssembly.Location != SelectedActivityAssemblyItem.Location) // we don't want to mark the main item we're importing as 'resolved'
                resolvedAssemblies.Add(newAssembly.Location);

            AssembliesToImport
                  .Where(item => resolvedAssemblies.Contains(item.Location))
                  .ToList()
                  .ForEach(item => item.IsResolved = true);
        }

    }
}
