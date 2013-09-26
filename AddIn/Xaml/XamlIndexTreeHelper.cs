using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AddIn
{
    public static class XamlIndexTreeHelper
    {
        public static List<XamlIndexNode> Nodes { get; private set; }

        public static void CreateIndexTree(string xaml)
        {
            List<XamlIndexNode> result = new List<XamlIndexNode>();
            XamlIndexNode current = null;
            XamlIndexNode parent = null;
            Stack<XamlIndexNode> stack = new Stack<XamlIndexNode>();
            int offset = 0;
            string node = string.Empty;
            char x;

            stack.Push(new XamlIndexNode() { Name = "root" });

            for (int i = 0; i < xaml.Length; i++)
            {
                x = xaml[i];
                switch (x)
                {
                    case '<':
                        offset = i;
                        break;
                    case '>':
                        node = xaml.Substring(offset, i - offset + 1);
                        if (current != null && node.Contains("/" + current.Name))
                        {
                            current.Length = i - current.Offset + 1;
                            current.Name = current.Name.GetTypeName();
                            result.Add(current);
                            stack.Pop();
                        }
                        else if (node.Contains("/>"))
                        {
                            parent = current;
                            XamlIndexNode newNode = new XamlIndexNode()
                            {
                                Offset = offset,
                                Length = i - offset + 1,
                                Parent = parent.Offset,
                                Name = node.GetName().GetTypeName(),
                                DisplayName = node.GetDisplayName(),
                            };

                            result.Add(newNode);
                        }
                        else
                        {
                            parent = stack.Peek();

                            current = new XamlIndexNode()
                            {
                                Offset = offset,
                                Parent = parent.Offset,
                                Name = node.GetName(),
                                DisplayName = node.GetDisplayName(),
                            };

                            stack.Push(current);
                        }

                        current = stack.Peek();
                        node = string.Empty;
                        break;
                }
            }

            //string tree = result.Aggregate("",
            //    (t, i) => t + string.Format("Name = {0}, DisplayName = {1}, Offset = {2}, Parent = {3}", i.Name, i.DisplayName, i.Offset, i.Parent) + Environment.NewLine);

            Nodes = result;
        }

        public static XamlIndexNode Search(WorkflowOutlineNode activity)
        {
            return Search(activity, Nodes);
        }

        private static string GetWorkflowOutlineTypeName(WorkflowOutlineNode node)
        {
            //string typeName;
            //if (node.Activity != null && node.Activity is Activity)
            //{
            //    typeName = ((Activity)node.Activity).DisplayName;
            //}
            //else
            //{
            //    typeName = node.ActivityType.Name;
            //}
            return node.ActivityType.Name;
        }

        private static XamlIndexNode Search(WorkflowOutlineNode activity, List<XamlIndexNode> nodes)
        {
            if (activity == null)
            {
                throw new ApplicationException("selection is null.");
            }

            List<XamlIndexNode> candidate = Match(GetWorkflowOutlineTypeName(activity), activity.NodeName);
            int count = candidate.Count;
            if (count == 1)
            {
                return candidate.Single();
            }
            else if (count > 1)
            {
                XamlIndexNode parent = Search(activity.Parent, candidate);
                return FilterInChildren(parent, candidate, activity);
            }
            else
            {
                throw new ApplicationException(string.Format("No match selection {0}.", activity.NodeName));
            }
        }

        private static XamlIndexNode FilterInChildren(XamlIndexNode parent, List<XamlIndexNode> children, WorkflowOutlineNode activity)
        {
            var descendant = RetrieveChildrenOrDescendant(parent, children);
            return FilterInChildren(descendant, activity);
        }

        private static XamlIndexNode FilterInChildren(List<XamlIndexNode> children, WorkflowOutlineNode activity)
        {
            if (children.Count == 1)
                return children[0];
            else
            {
                int index = GetChildrenIndex(activity);
                return children.Where(c => c.Name == GetWorkflowOutlineTypeName(activity)).ToList()[index];
            }
        }

        private static int GetChildrenIndex(WorkflowOutlineNode activity)
        {
            List<WorkflowOutlineNode> sameTypeActivies;
            if (activity.PropertyNameOfNodeName == "DisplayName" &&
                activity.NodeName != GetWorkflowOutlineTypeName(activity))
            {
                sameTypeActivies = activity.Parent.Children.Where(c => c.NodeName == activity.NodeName).ToList();
            }
            else
            {
                sameTypeActivies = activity.Parent.Children.Where(c => GetWorkflowOutlineTypeName(c) == GetWorkflowOutlineTypeName(activity)).ToList();
            }

            return sameTypeActivies.IndexOf(activity);
        }

        private static List<XamlIndexNode> RetrieveChildrenOrDescendant(XamlIndexNode parent, List<XamlIndexNode> children)
        {
            List<XamlIndexNode> result = new List<XamlIndexNode>();
            var query = from c in children
                        where c.Parent == parent.Offset
                        select c;
            if (query.Any())
            {
                result = query.ToList();
            }
            else
            {
                var descendants = from c in Nodes
                                  where c.Parent == parent.Offset
                                  select c;

                foreach (var descendant in descendants)
                {
                    List<XamlIndexNode> childrenOfDescendant = RetrieveChildrenOrDescendant(descendant, children);
                    result.AddRange(childrenOfDescendant);
                }
            }

            return result;
        }

        private static List<XamlIndexNode> Match(string type, string displayName)
        {
            string typeName = type.RemoveGenericsName();
            if (typeName == displayName)
                return Match(typeName);

            var query = from x in Nodes
                        where x.Name == typeName && x.DisplayName == displayName
                        select x;
            if (!query.Any())
                return Match(typeName);

            return query.ToList();
        }

        private static List<XamlIndexNode> Match(string type)
        {
            return (from x in Nodes
                    where x.Name == type
                    select x).ToList();
        }

    }
}
