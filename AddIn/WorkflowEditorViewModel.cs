using Microsoft.Practices.Prism.ViewModel;
using System;
using System.Activities.Presentation;
using System.Activities.Presentation.Model;
using System.Activities.Presentation.Services;
using System.Activities.Presentation.View;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace AddIn
{
    [Serializable]
    public class WorkflowEditorViewModel : NotificationObject
    {
        [NonSerialized]
        private WorkflowDesigner workflowDesigner;
        private string xamlCode;
        private string name;
        private ObservableCollection<WorkflowOutlineNode> workflowOutlineNode;
        private XamlIndexNode xamlNode;
        private List<XamlIndexNode> nodes;

        public event ActivityFocuceEventHandler ActivityFocuceChanged;

        private void RaiseDesignerChanged(WorkflowOutlineNode node)
        {
            if (this.ActivityFocuceChanged != null)
                ActivityFocuceChanged(this, new ActivityFocuceEventArgs(node));
        }

        public ObservableCollection<WorkflowOutlineNode> WorkflowOutlineNodes
        {
            get { return this.workflowOutlineNode; }
            set
            {
                this.workflowOutlineNode = value;
                RaisePropertyChanged(() => this.WorkflowOutlineNodes);
            }
        }
        /// <summary>
        /// Gets or sets XamlCode.
        /// </summary>
        public string XamlCode
        {
            get
            {
                return xamlCode;
            }
            set
            {
                xamlCode = value;
                //xamlNode = XamlTreeHelper.Create(xamlCode);
                XamlIndexTreeHelper.CreateIndexTree(xamlCode);
                RaisePropertyChanged(() => XamlCode);
            }
        }

        /// <summary>
        /// Gets or sets WorkflowDesigner.
        /// </summary>
        public WorkflowDesigner WorkflowDesigner
        {
            get
            {
                return workflowDesigner;
            }
            set
            {
                workflowDesigner = value;
                ConfigureWorkflowDesigner(workflowDesigner);
                ConfigureWorkflowOutlineNode(workflowDesigner);
                WorkflowDesigner.ModelChanged += WorkflowDesigner_ModelChanged;

            }
        }

        private void ConfigureWorkflowOutlineNode(WorkflowDesigner workflowDesigner)
        {
            if (workflowDesigner == null || workflowDesigner.Context.Services.GetService<ModelService>() == null)
                return;

            var root = this.workflowDesigner.Context.Services.GetService<ModelService>().Root;
            this.WorkflowOutlineNodes = new ObservableCollection<WorkflowOutlineNode>() 
            { 
                new WorkflowOutlineNode(root) 
            };
            workflowDesigner.Context.Items.Subscribe<Selection>(ActivityFocusedChanged);
        }

        private void ActivityFocusedChanged(Selection s)
        {
            WorkflowOutlineNode node = this.FindFocusedWorkflowOutlineNode();
            RaiseDesignerChanged(node);
        }

        private WorkflowOutlineNode FindFocusedWorkflowOutlineNode()
        {
            ModelItem focusedActivity = this.WorkflowDesigner.Context.Items.GetValue<Selection>().PrimarySelection;
            WorkflowOutlineNode root = this.WorkflowOutlineNodes.FirstOrDefault();
            Queue<WorkflowOutlineNode> queue = new Queue<WorkflowOutlineNode>();
            queue.Enqueue(root);
            while (queue.Count > 0)
            {
                WorkflowOutlineNode top = queue.Dequeue();
                if (top.Model == focusedActivity)
                {
                    return top;
                }
                else if (top.Children != null)
                {
                    top.Children.ToList().ForEach(i => queue.Enqueue(i));
                }
            }
            return null;
        }

        private void WorkflowDesigner_ModelChanged(object sender, EventArgs e)
        {
            ConfigureWorkflowOutlineNode(this.WorkflowDesigner);
        }

        private void ConfigureWorkflowDesigner(WorkflowDesigner workflowDesigner)
        {
            workflowDesigner.Text = xamlCode; // setup for workflowDesigner.Load()
            workflowDesigner.Load(); // initialize workflow based on Text property
        }

        public void Init(string name, string projectXamlCode)
        {
            this.name = name;
            // Set the XAML code
            XamlCode = projectXamlCode;
            WorkflowDesigner = new WorkflowDesigner();
        }
    }
}
