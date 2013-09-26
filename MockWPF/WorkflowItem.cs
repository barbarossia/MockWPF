using AddIn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace MockWPF
{
    public sealed class WorkflowItem : ViewModelBase
    {
        private FrameworkElement designerView;
        public FrameworkElement DesignerView
        {
            get { return designerView; }
            set
            {
                this.designerView = value;
                RaisePropertyChanged(() => this.DesignerView);
            }
        }

        private DesignerHostAdapters workflowDesigner;
        public DesignerHostAdapters WorkflowDesigner
        {
            get
            {
                return workflowDesigner;
            }
            set
            {
                this.workflowDesigner = value;
                if (workflowDesigner != null)
                {
                    this.DesignerView = this.workflowDesigner.View;
                    this.Toolbox = this.workflowDesigner.ToolboxView;
                }
                RaisePropertyChanged(() => WorkflowDesigner);
            }
        }

        private FrameworkElement toolbox;
        public FrameworkElement Toolbox
        {
            get { return this.toolbox; }
            set
            {
                this.toolbox = value;
                RaisePropertyChanged(() => this.Toolbox);
            }
        }

        public void InitializeWorkflowDesigner()
        {
            this.WorkflowDesigner = new DesignerHostAdapters(this.Name, this.XamlCode);
        }

        public string Name { get; set; }
        public string XamlCode { get; set; }

        public List<ActivityAssemblyItem> References { get; set; }


        public int CompareTo(WorkflowItem otherObject)
        {
            // Return less than if the inbound assembly is null, this is unexpected but possible
            if (null == otherObject)
                return -1;

            // Return the Compare on the name only
            return string.Compare(Name, otherObject.Name, StringComparison.InvariantCultureIgnoreCase);
        }

    }
}
