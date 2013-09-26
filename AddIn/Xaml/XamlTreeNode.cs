using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AddIn
{
    [Serializable]
    public class XamlTreeNode
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Type { get; set; }
        public int Offset { get; set; }
        public int Length { get; set; }
        public XamlTreeNode Parent { get; set; }
        public List<XamlTreeNode> Children { get; set; }

        public XamlTreeNode()
        {
            this.Children = new List<XamlTreeNode>();
        }
    }
}
