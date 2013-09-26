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

namespace AddIn
{
    /// <summary>
    /// Interaction logic for WorkflowEditorView.xaml
    /// </summary>
    public partial class WorkflowEditorView : UserControl
    {

        public WorkflowEditorView()
        {
            InitializeComponent();
            //Loaded += OnLoaded;
        }

        private void ToggleBottomPanel(FrameworkElement control, Action onDisplay = null)
        {
            bool visible = control.Visibility == Visibility.Visible;

            XamlCodeEditor.Visibility
                = Visibility.Collapsed;

            if (!visible)
            {
                control.Visibility = Visibility.Visible;
                if (onDisplay != null)
                    onDisplay();
            }
        }

        private void ShowXamlButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleBottomPanel(XamlCodeEditor);
        }

        private void XamlCodeEditor_MightHaveBeenEdited(object sender, RoutedEventArgs e)
        {
            //var workflowItem = ((WorkflowEditorViewModel)DataContext);
            //// Since XamlCodeEditor.Text has a one-way binding to workflowItem.XamlCode,
            //// if the workflow's XamlCode doesn't match the actual text on the screen, 
            //// then the user edited the XAML so we need to refresh the designer to 
            //// display the XAML he typed.
            //if (workflowItem.XamlCode != XamlCodeEditor.Text)
            //{
            //    // copy back the Xaml to where the designer can read it from
            //    workflowItem.XamlCode = XamlCodeEditor.Text;
            //    workflowItem.RefreshDesignerFromXamlCode();
            //    ConfigureWorkflowDesigner(workflowItem, workflowItem.IsTask); // recompute steps for the new WorkflowDesigner
            //}
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == Control.DataContextProperty)
            {
                var workflowItem = e.NewValue as WorkflowEditorViewModel;
                if (workflowItem != null)
                {
                    ConfigureWorkItem(workflowItem);
                }
            }
        }

        private void ConfigureWorkItem(WorkflowEditorViewModel workflowItem)
        {
            workflowItem.ActivityFocuceChanged += OnDesignerChanged;
        }

        private void OnDesignerChanged(object sender, ActivityFocuceEventArgs e)
        {
            if (XamlCodeEditor.Visibility == Visibility.Visible)
            {
                WorkflowOutlineNode selection = e.Node;
                if (selection != null)
                {
                    if (selection.Offset == 0 || selection.Length == 0)
                    {
                        XamlIndexNode index = XamlIndexTreeHelper.Search(selection);
                        selection.Offset = index.Offset;
                        selection.Length = index.Length;
                    }

                    XamlCodeEditor.Focus();
                    XamlCodeEditor.Select(selection.Offset, selection.Length);
                }
            }
            else
            {
                XamlCodeEditor.SelectionLength = 0;
            }
        }
    }
}
