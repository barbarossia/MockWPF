using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AddIn
{
    public class XamlTreeNodeAdapter : ILinqTree<XamlTreeNode>
    {
        private XamlTreeNode _item;

        public XamlTreeNodeAdapter(XamlTreeNode item)
        {
            _item = item;
        }

        public IEnumerable<XamlTreeNode> Children()
        {
            return _item.Children;
        }

        public XamlTreeNode Parent
        {
            get
            {
                return _item.Parent;
            }
        }
    }
}
