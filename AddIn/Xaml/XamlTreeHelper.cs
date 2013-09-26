using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AddIn
{
    public static class XamlTreeHelper
    {
        public static XamlTreeNode Create(string xaml)
        {
            XamlTreeNode current = null;
            XamlTreeNode parent = null;
            XamlTreeNode root = new XamlTreeNode() { Name = "root" };
            Stack<XamlTreeNode> stack = new Stack<XamlTreeNode>();
            int offset = 0;
            string node = string.Empty;
            char x;

            stack.Push(root);

            for (int i = 0; i < xaml.Length; i++)
            {
                x = xaml[i];
                node += x;
                switch (x)
                {
                    case '<':
                        offset = i;
                        node = new string(x, 1);
                        break;
                    case '>':
                        if (current != null && node.Contains("/" + current.Name))
                        {
                            current.Length = i - current.Offset;
                            stack.Pop();
                        }
                        else if (node.Contains("/>"))
                        {
                            parent = current;
                            if (parent != null)
                            {
                                XamlTreeNode newNode = new XamlTreeNode()
                                    {
                                        Offset = offset,
                                        Length = i - offset,
                                        Parent = parent
                                    };
                                newNode.SetName(node);
                                parent.Children.Add(newNode);
                            }
                        }
                        else
                        {
                            current = new XamlTreeNode();

                            current.SetName(node);

                            current.Offset = offset;
                            if (stack.Count > 0)
                                parent = stack.Peek();
                            if (parent != null)
                                parent.Children.Add(current);

                            current.Parent = parent;
                            stack.Push(current);
                        }

                        current = stack.Peek();
                        node = string.Empty;
                        break;
                }
            }

            //string tree = root.DescendantsAndSelf().Aggregate("",
            //    (bc, n) => bc + n.Ancestors().Aggregate("", (ac, m) => (m.ElementsAfterSelf().Any() ? "| " : "  ") + ac,
            //    ac => ac + (n.ElementsAfterSelf().Any() ? "+-" : "\\-")) + n.DisplayName + "\n");

            if (root.Children.Any())
                return root.Children[0];
            return null;
        }


        private static XamlTreeNode FindXaml(WorkflowOutlineNode node, XamlTreeNode xaml)
        {
            XamlTreeNode result = null;
            if (xaml.Type == node.ActivityType.Name && (xaml.DisplayName == node.NodeName || xaml.Type == node.NodeName))
            {
                result = xaml;
            }
            else
            {
                foreach (var child in xaml.Children)
                {
                    result = FindXaml(node, child);
                    if (result != null)
                        break;
                }
            }

            return result;
        }

        
    }
}
