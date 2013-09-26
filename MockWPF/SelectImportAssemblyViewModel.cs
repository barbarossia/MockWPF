using AddIn;
using AddIn.Common.ExceptionHandling;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MockWPF
{
    public class SelectImportAssemblyViewModel : ViewModelBase
    {
        private Dictionary<ActivityAssemblyItem, List<ActivityAssemblyItem>> referencedAssemblies = new Dictionary<ActivityAssemblyItem, List<ActivityAssemblyItem>>();
        private WorkflowItem focusedWorkflow;
        private string title;
        private bool? isHeaderChecked = false;
        private bool isHeaderThreeState = false;
        private bool shouldPopupMessage = true;

        public string Title
        {
            get { return this.title; }
            set
            {
                this.title = value;
                RaisePropertyChanged(() => this.Title);
            }
        }

        public bool? IsHeaderChecked
        {
            get { return isHeaderChecked; }
            set
            {
                isHeaderChecked = value;
                RaisePropertyChanged(() => IsHeaderChecked);

                IsHeaderThreeState = (value == null);
            }
        }

        public bool IsHeaderThreeState
        {
            get { return isHeaderThreeState; }
            set
            {
                isHeaderThreeState = value;
                RaisePropertyChanged(() => IsHeaderThreeState);
            }
        }

        private ObservableCollection<string> tabNames;
        public ObservableCollection<string> TabNames
        {
            get { return this.tabNames; }
            set
            {
                this.tabNames = value;
                RaisePropertyChanged(() => TabNames);
            }
        }

        private ObservableCollection<ActivityAssemblyItem> assemblies;
        public ObservableCollection<ActivityAssemblyItem> Assemblies
        {
            get { return assemblies; }
            set
            {
                this.assemblies = value;
                RaisePropertyChanged(() => this.Assemblies);
            }
        }

        private ActivityAssemblyItem selectedActivityAssemblyItem;
        public ActivityAssemblyItem SelectedActivityAssemblyItem
        {
            get { return selectedActivityAssemblyItem; }
            set
            {
                this.selectedActivityAssemblyItem = value;
                RaisePropertyChanged(() => this.SelectedActivityAssemblyItem);
            }
        }

        public bool ImportResult { get; set; }

        public DelegateCommand BrowseCommand { get; set; }
        public DelegateCommand ImportCommand { get; set; }

        public SelectImportAssemblyViewModel(WorkflowItem item)
        {
            this.focusedWorkflow = item;
            this.Title = string.Format("Import Assemblies to Workflow {0}", item.Name);
            this.BrowseCommand = new DelegateCommand(this.BrowseFile);
            this.ImportCommand = new DelegateCommand(this.ImportAssembly);
            this.InitializeAssemblies();
        }

        private void InitializeAssemblies()
        {
            Caching.LoadFromLocal();
            foreach (ActivityAssemblyItem item in Caching.ActivityAssemblyItems)
            {
                item.UserSelected = false;
            }
            Assemblies = new ObservableCollection<ActivityAssemblyItem>(Caching.ActivityAssemblyItems);
            if (focusedWorkflow.WorkflowDesigner != null)
            {
                //List<ActivityAssemblyItem> references = focusedWorkflow.WorkflowDesigner.DependencyAssemblies.ToList();                
                //foreach (ActivityAssemblyItem item in Assemblies)
                //{
                //    item.PropertyChanged += item_PropertyChanged;
                //    item.UserSelected = references.Any(r => r.Matches(item));
                //}
            }
        }

        private void item_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "UserSelected")
            {
                ActivityAssemblyItem item = sender as ActivityAssemblyItem;
                CheckAssemblyDependencies(item);
                SetHeaderCheckbox(item);
            }
        }

        public void Cleanup()
        {
            foreach (ActivityAssemblyItem item in Assemblies)
            {
                item.PropertyChanged -= item_PropertyChanged;
            }
        }

        public void ChangeDefaultCategory()
        {
            if (this.SelectedActivityAssemblyItem != null && this.SelectedActivityAssemblyItem.ActivityItems != null)
                this.SelectedActivityAssemblyItem.ActivityItems.ToList().ForEach(item => item.Category = this.SelectedActivityAssemblyItem.Category);
        }

        public void HeaderCheckboxClicked()
        {
            if (IsHeaderChecked.HasValue && !IsHeaderChecked.Value)
            {
                shouldPopupMessage = false;
                foreach (ActivityAssemblyItem item in Assemblies.Reverse())
                {
                    if (!item.UserSelected)
                        item.UserSelected = true;
                }
                shouldPopupMessage = true;
            }
            else
            {
                foreach (ActivityAssemblyItem item in Assemblies)
                {
                    item.PropertyChanged -= item_PropertyChanged;
                    item.UserSelected = false;
                    item.PropertyChanged += item_PropertyChanged;
                }
                IsHeaderChecked = false;
                referencedAssemblies.Clear();
            }
        }

        private void BrowseFile()
        {
            string[] assemblyFileNames = DialogService.ShowOpenFileDialogAndReturnMultiResult("Assembly files (*.dll)|*.dll", "Open Assembly File");
            if (assemblyFileNames != null)
            {
                try
                {
                    ImportAssemblyViewModel vm = new ImportAssemblyViewModel(assemblyFileNames);
                    DialogService.ShowDialog(vm);
                    if (vm.ImportResult)
                    {
                        List<ActivityAssemblyItem> assemblyToImport = new List<ActivityAssemblyItem>();
                        vm.AssembliesToImport.ToList().ForEach(item =>
                        {
                            if (!Assemblies.Any(asm => asm.Name == item.Name && asm.Version == Version.Parse(item.Version)))
                            {

                                Assemblies.Add(item.Source);
                                item.Source.UserSelected = false;
                                assemblyToImport.Add(item.Source);
                            }
                        });
                        assemblyToImport.ForEach(item => item.PropertyChanged += item_PropertyChanged);
                        assemblyToImport.ForEach(item => item.UserSelected = true);
                    }
                }
                catch (Exception ex)
                {
                    throw new UserFacingException(ex.Message);
                }
            }
        }

        private void ImportAssembly()
        {
            try
            {
                List<ActivityAssemblyItem> assembliesToImport = new List<ActivityAssemblyItem>();
                if (this.Assemblies != null)
                    this.Assemblies.ToList().ForEach(i =>
                    {
                        if (i.UserSelected)
                            assembliesToImport.Add(i);
                    });

                //assembliesToImport.ForEach(i => i.CachingStatus = CachingStatus.None);
                //if (this.focusedWorkflow.WorkflowDesigner != null)
                //    this.focusedWorkflow.WorkflowDesigner.ImportAssemblies(assembliesToImport);
                ImportResult = true;
            }
            catch (Exception ex)
            {
                ImportResult = false;
                throw new UserFacingException(ex.Message);
            }
        }

        private void SetHeaderCheckbox(ActivityAssemblyItem item)
        {
            if (item.UserSelected)
            {
                if (Assemblies.All(a => a.UserSelected))
                    IsHeaderChecked = true;
                else
                    IsHeaderChecked = null;
            }
            else
            {
                if (Assemblies.All(a => !a.UserSelected))
                    IsHeaderChecked = false;
                else
                    IsHeaderChecked = null;
            }
        }

        private void CheckAssemblyDependencies(ActivityAssemblyItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException();
            }

            if (item.UserSelected)
            {
                if (CheckIfAnyConflict(item))
                {
                    item.UserSelected = false;
                }
                else
                {
                    if (item.ReferencedAssemblies != null && item.ReferencedAssemblies.Count > 0)
                    {
                        foreach (AssemblyName assemblyName in item.ReferencedAssemblies)
                        {
                            if (!Utility.AssemblyIsBuiltIn(assemblyName))
                            {
                                ActivityAssemblyItem refItem = Assemblies.Single(i => i.Name == assemblyName.Name && i.Version == assemblyName.Version);
                                if (referencedAssemblies.ContainsKey(refItem))
                                {
                                    if (!referencedAssemblies[refItem].Contains(item))
                                        referencedAssemblies[refItem].Add(item);
                                }
                                else
                                {
                                    referencedAssemblies.Add(refItem, new[] { item }.ToList());
                                }
                                refItem.UserSelected = true;
                            }
                        }
                    }
                }
            }
            else
            {
                if (referencedAssemblies.ContainsKey(item)
                    && referencedAssemblies[item].Any())
                {
                    if (shouldPopupMessage)
                        MessageBoxService.CannotUncheckAssemblyForReferenced(item.AssemblyName, referencedAssemblies[item].Select(a => a.AssemblyName).ToArray());
                    item.UserSelected = true;
                }
                else
                {
                    if (item.ReferencedAssemblies != null && item.ReferencedAssemblies.Count > 0)
                    {
                        foreach (AssemblyName assemblyName in item.ReferencedAssemblies)
                        {
                            if (!Utility.AssemblyIsBuiltIn(assemblyName))
                            {
                                ActivityAssemblyItem refItem = Assemblies.Single(i => i.Name == assemblyName.Name && i.Version == assemblyName.Version);
                                if (referencedAssemblies.ContainsKey(refItem))
                                    referencedAssemblies[refItem].Remove(item);
                            }
                        }
                    }
                }
            }
        }

        private bool CheckIfAnyConflict(ActivityAssemblyItem item)
        {
            if (CheckIfConflict(item))
                return true;

            bool hasConflicts = false;
            if (item.ReferencedAssemblies != null && item.ReferencedAssemblies.Count > 0)
            {
                foreach (AssemblyName assemblyName in item.ReferencedAssemblies)
                {
                    if (!Utility.AssemblyIsBuiltIn(assemblyName))
                    {
                        ActivityAssemblyItem refItem = Assemblies.Single(i => i.Name == assemblyName.Name && i.Version == assemblyName.Version);
                        if (CheckIfAnyConflict(refItem))
                        {
                            hasConflicts = true;
                            break;
                        }
                    }
                }
            }
            return hasConflicts;
        }

        private bool CheckIfConflict(ActivityAssemblyItem item)
        {
            if (item.Name == focusedWorkflow.Name)
            {
                if (shouldPopupMessage)
                    MessageBoxService.CannotCheckAssemblyItself();
                return true;
            }

            ActivityAssemblyItem conflictAssembly = Assemblies.SingleOrDefault(a => a.UserSelected && a.Name == item.Name && a.Version != item.Version);
            if (conflictAssembly != null)
            {
                if (shouldPopupMessage)
                    MessageBoxService.CannotCheckAssemblyForAnotherVersionSelected(
                        item.Name, item.Version.ToString(), conflictAssembly.Version.ToString());
                return true;
            }

            return false;
        }
    }
}
