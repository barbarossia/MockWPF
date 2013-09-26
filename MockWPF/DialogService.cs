using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace MockWPF
{
    public static class DialogService
    {
        /// <summary>
        /// Show open file dialog with title
        /// </summary>
        /// <param name="filter">filter for opening file</param>
        /// <param name="dialogTitle">title of dialog</param>
        /// <returns>full name of opened file</returns>
        public static string ShowOpenFileDialogAndReturnResult(string filter, string dialogTitle)
        {
            string fileName = string.Empty;
            var openFileDialog = CreateOpenFileDialogAndReturnResult(filter, dialogTitle, true);

            bool result = openFileDialog.ShowDialog().GetValueOrDefault();

            if (result)
            {
                fileName = openFileDialog.FileName;
            }

            return fileName;
        }

        /// <summary>
        /// Show open file dialog with title
        /// </summary>
        /// <param name="filter">filter for opening file</param>
        /// <param name="dialogTitle">title of dialog</param>
        /// <returns>full name of opened file</returns>
        public static string[] ShowOpenFileDialogAndReturnMultiResult(string filter, string dialogTitle)
        {
            string[] fileNames = null;

            var openFileDialog = CreateOpenFileDialogAndReturnResult(filter, dialogTitle, true);

            bool result = openFileDialog.ShowDialog().GetValueOrDefault();

            if (result)
            {
                fileNames = openFileDialog.FileNames;
            }

            return fileNames;
        }

        private static OpenFileDialog CreateOpenFileDialogAndReturnResult(string filter, string dialogTitle, bool isMultiselect)
        {
            var openFileDialog = CreateOpenFileDialog();
            openFileDialog.Filter = filter;
            openFileDialog.Multiselect = isMultiselect;

            if (string.IsNullOrEmpty(dialogTitle))
            {
                dialogTitle = "Open";
            }

            openFileDialog.Title = dialogTitle;

            return openFileDialog;
        }

        public static Func<OpenFileDialog> CreateOpenFileDialog = () => new OpenFileDialog();

        public static bool? ShowDialog(object viewModel)
        {
            return ShowDialogFunc(viewModel);
        }

        /// <summary>
        /// Extracted fucntionality from ShowDialog.
        /// This Func is rewritable so we can suppress dialogs during automation and gain access to the viewModel object.       
        /// </summary>
        public static Func<object, bool?> ShowDialogFunc = viewModel =>
        {
            Type viewType = ViewViewModelMappings.GetViewTypeFromViewModelType(viewModel.GetType());

            // Create new view and set the datacontext to the corresponding viewmodel
            var dialog = (Window)Activator.CreateInstance(viewType);
            dialog.Owner = Application.Current.MainWindow;
            dialog.DataContext = viewModel;

            // Show dialog
            return dialog.ShowDialog();
        };

    }
}
