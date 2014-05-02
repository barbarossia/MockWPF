using AddIn;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

namespace MockWPF
{
    public sealed class MainWindowViewModel : ViewModelBase
    {
        public DelegateCommand OpenWorkflowCommand { get; private set; }
        public DelegateCommand CDSViewCommand { get; private set; }
        private WorkflowItem focusedWorkflowItem;
        private ObservableCollection<WorkflowItem> workflowItems = new ObservableCollection<WorkflowItem>();
        public DelegateCommand DisableCommand { get; private set; }
        public ObservableCollection<WorkflowItem> WorkflowItems
        {
            get { return workflowItems; }

            set
            {
                workflowItems = value;
                RaisePropertyChanged(() => WorkflowItems);
            }
        }

        public WorkflowItem FocusedWorkflowItem
        {
            get { return focusedWorkflowItem; }
            set
            {
                focusedWorkflowItem = value;
                RaisePropertyChanged(() => FocusedWorkflowItem);
                if (focusedWorkflowItem != null && FocusedWorkflowItem.WorkflowDesigner == null)
                    FocusedWorkflowItem.InitializeWorkflowDesigner();
            }
        }

        public MainWindowViewModel()
        {
            OpenWorkflowCommand = new DelegateCommand(OpenWorkflowCommandExecute);
            CDSViewCommand = new DelegateCommand(CDSViewCommandExecute);
            DisableCommand = new DelegateCommand(DisableCommandExecute);
        }

        private void DisableCommandExecute()
        {
            FocusedWorkflowItem.SetupCompositeActivityDesinger();
        }

        private void CDSViewCommandExecute()
        {
            CDSPackagesManagerViewModel vm = new CDSPackagesManagerViewModel();
            DialogService.ShowDialog(vm);
        }

        private void OpenWorkflowCommandExecute()
        {
            string workflowFileName = ShowOpenFileDialogAndReturnResult("Workflow files (*.xaml)|*.xaml", "Open Workflow File");
            if (!string.IsNullOrEmpty(workflowFileName))
            {
                OpenWorkflowFromLocal(workflowFileName);
            }
        }

            
        private void OpenWorkflowFromLocal(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException("fileName");
            }

            if (String.IsNullOrEmpty(Path.GetExtension(fileName)))
            {
                fileName += ".xaml";
            }

            string xaml = new FileInfo(fileName).OpenText().ReadToEnd();
            // Recover WorkflowItem from XAML or XML file
            var recoverdWorkflow = new WorkflowItem()
            {
                Name = Path.GetFileNameWithoutExtension(fileName),
                XamlCode = xaml,
                References = Caching.ActivityAssemblyItems.ToList(),
            };
            CheckIsAlreadyInListOrAdd(recoverdWorkflow);

        }

        public bool CheckIsAlreadyInListOrAdd(WorkflowItem itemToCheck)
        {
            //var itemToFind = WorkflowItems.FirstOrDefault(wfi => 0 == wfi.CompareTo(itemToCheck));
            //if (null != itemToFind)
            //{
            //    FocusedWorkflowItem = itemToFind;
            //    return false;
            //}
            //else
            //{
            //    WorkflowItems.Add(itemToCheck);
            //    FocusedWorkflowItem = itemToCheck;
            //    return true;
            //}

            WorkflowItems.Add(itemToCheck);
            FocusedWorkflowItem = itemToCheck;
            return true;
        }

        public static string ShowOpenFileDialogAndReturnResult(string filter, string dialogTitle)
        {
            string fileName = string.Empty;
            var openFileDialog = CreateOpenFileDialog();
            openFileDialog.Filter = filter;

            if (string.IsNullOrEmpty(dialogTitle))
            {
                dialogTitle = "Open";
            }

            openFileDialog.Title = dialogTitle;
            bool result = openFileDialog.ShowDialog().GetValueOrDefault();

            if (result)
            {
                fileName = openFileDialog.FileName;
            }

            return fileName;
        }

        public static Func<OpenFileDialog> CreateOpenFileDialog = () => new OpenFileDialog();

    }
}
