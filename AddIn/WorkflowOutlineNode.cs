using System;
using System.Activities;
using System.Activities.Presentation.Model;
using System.Activities.Statements;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace AddIn
{
    public class WorkflowOutlineNode
    {

        private object activity;
        /// <summary>
        /// Activity Data that the node represent
        /// </summary>
        public object Activity
        {
            get { return this.activity; }
            set
            {
                this.activity = value;
                if (this.activity != null)
                    this.SetId(this.activity);
            }
        }

        /// <summary>
        /// Activity id
        /// </summary>
        public string Id { get; set; }

        public WorkflowOutlineNode Parent
        {
            get;
            set;
        }

        /// <summary>
        /// the treeview node associated ModelItem
        /// Can set Model.Properties[""] = value to change the WF and WF view
        /// </summary>
        public ModelItem Model
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the Activity's type
        /// </summary>
        public Type ActivityType
        {
            get
            {
                if (this.Activity != null)
                {
                    return this.Activity.GetType();
                }
                return null;
            }
        }


        /// <summary>
        /// Gets or sets the property name of the treeviewnode value
        /// Can set Model.Properties[PropertyNameOfNode].SetValue(newname) to rename the node
        /// </summary>
        public string PropertyNameOfNodeName
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets the name of the treeviewnode
        /// </summary>
        public string NodeName
        {
            get;
            set;
        }

        /// <summary>
        /// The Activity’s subactivities
        /// </summary>
        public ObservableCollection<WorkflowOutlineNode> Children
        {
            get;
            set;
        }

        public int Offset { get; set; }
        public int Length { get; set; }

        /// <summary>
        ///For display Workflow in TreeView, Convert ModelItem to WorkflowOutlineNode collection
        /// </summary>
        /// <param name="model"></param>
        public WorkflowOutlineNode(ModelItem model)
        {
            if (model != null)
            {
                this.SetActivityFromModelItem(model);
                this.SetPropertyNameOfNodeNameFromModelItem(model);
                this.SetChildrenActivityFromModelItemProperty();
            }
        }

        /// <summary>
        /// Display Activity DisplayName for WorkflowoutlineNode
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return NodeName;
        }

        private void SetId(object data)
        {
            string id = string.Empty;
            Activity ac = data as Activity;
            if (ac != null)
                id = ac.Id;
            this.Id = id;
        }

        /// <summary>
        /// Set Activity according ModelItem
        /// </summary>
        /// <param name="model"></param>
        private void SetActivityFromModelItem(ModelItem model)
        {
            if (model.ItemType == (typeof(FlowStep)))
            {
                FlowStep act = model.GetCurrentValue() as FlowStep;
                Activity = act.Action;
                Model = model.Properties["Action"].Value as ModelItem;
            }
            else if (model.ItemType.IsSubclassOf(typeof(ActivityDelegate)))
            {
                ActivityDelegate act = model.GetCurrentValue() as ActivityDelegate;
                if (act.Handler != null)
                {
                    Activity = act.Handler;
                    Model = model.Properties["Handler"].Value as ModelItem;
                }
                else
                {
                    Activity = model.GetCurrentValue();
                    Model = model;
                }
            }
            else
            {
                Activity = model.GetCurrentValue();
                Model = model;
            }
        }

        /// <summary>
        /// set ChildrenActivity according current ModelItem
        /// </summary>
        private void SetChildrenActivityFromModelItemProperty()
        {
            Children = new ObservableCollection<WorkflowOutlineNode>();
            foreach (ModelProperty property in Model.Properties)
            {
                ModelItem value = property.Value;
                if (value == null)
                    continue;

                if (value.ItemType.IsSubclassOf(typeof(Activity)) || value.ItemType.IsSubclassOf(typeof(ActivityDelegate)))
                {
                    WorkflowOutlineNode newchild = new WorkflowOutlineNode(value);
                    Children.Add(newchild);
                    newchild.Parent = this;
                    continue;
                }

                ModelItemCollection items = value as ModelItemCollection;
                if (items != null)
                {
                    foreach (ModelItem i in items)
                    {
                        if (i.ItemType == typeof(FlowStep)
                            || i.ItemType.IsSubclassOf(typeof(Activity))
                            || i.ItemType.IsSubclassOf(typeof(ActivityDelegate))
                            || i.ItemType == typeof(PickBranch)
                            || i.ItemType == typeof(FlowDecision))
                        {
                            WorkflowOutlineNode newchild = new WorkflowOutlineNode(i);
                            Children.Add(newchild);
                            newchild.Parent = this;
                        }
                    }
                    continue;
                }

                ModelItemDictionary dictionary = value as ModelItemDictionary;
                if (dictionary != null)
                {
                    foreach (KeyValuePair<ModelItem, ModelItem> pair in dictionary)
                    {
                        ModelItem candidateItem = pair.Value;
                        if (candidateItem == null)
                            continue;
                        if (candidateItem.ItemType.IsSubclassOf(typeof(Activity))
                            || candidateItem.ItemType.IsSubclassOf(typeof(FlowStep))
                            || candidateItem.ItemType.IsSubclassOf(typeof(ActivityDelegate)))
                        {
                            WorkflowOutlineNode newchild = new WorkflowOutlineNode(candidateItem);
                            Children.Add(newchild);
                            newchild.Parent = this;
                        }
                    }
                    continue;
                }
            }
        }

        /// <summary>
        /// Set PropertyNameOfNodeName according ModelItem
        /// </summary>
        /// <param name="model"></param>
        private void SetPropertyNameOfNodeNameFromModelItem(ModelItem model)
        {
            if (model.ItemType == typeof(ActivityBuilder))
            {
                this.NodeName = this.Model.Properties["Name"].Value.ToString();
                PropertyNameOfNodeName = "Name";
            }
            else if (this.Model.Properties["DisplayName"] != null && this.Model.Properties["DisplayName"].Value != null)
            {
                this.NodeName = this.Model.Properties["DisplayName"].Value.ToString();
                PropertyNameOfNodeName = "DisplayName";
            }
            else
                this.NodeName = string.Empty;
        }

    }
}
