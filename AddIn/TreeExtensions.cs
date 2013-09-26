using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AddIn
{
    public static class TreeExtensions
    {
        /// <summary>
        /// Returns a collection of descendant elements.
        /// </summary>
        public static IEnumerable<XamlTreeNode> Descendants(this XamlTreeNode item)
        {
            ILinqTree<XamlTreeNode> adapter = new XamlTreeNodeAdapter(item);
            foreach (var child in adapter.Children())
            {
                yield return child;

                foreach (var grandChild in child.Descendants())
                {
                    yield return grandChild;
                }
            }
        }

        /// <summary>
        /// Returns a collection containing this element and all descendant elements.
        /// </summary>
        public static IEnumerable<XamlTreeNode> DescendantsAndSelf(this XamlTreeNode item)
        {
            yield return item;

            foreach (var child in item.Descendants())
            {
                yield return child;
            }
        }

        /// <summary>
        /// Returns a collection of ancestor elements.
        /// </summary>
        public static IEnumerable<XamlTreeNode> Ancestors(this XamlTreeNode item)
        {
            ILinqTree<XamlTreeNode> adapter = new XamlTreeNodeAdapter(item);

            var parent = adapter.Parent;
            while (parent != null)
            {
                yield return parent;
                adapter = new XamlTreeNodeAdapter(parent);
                parent = adapter.Parent;
            }
        }

        /// <summary>
        /// Returns a collection containing this element and all ancestor elements.
        /// </summary>
        public static IEnumerable<XamlTreeNode> AncestorsAndSelf(this XamlTreeNode item)
        {
            yield return item;

            foreach (var ancestor in item.Ancestors())
            {
                yield return ancestor;
            }
        }

        /// <summary>
        /// Returns a collection of child elements.
        /// </summary>
        public static IEnumerable<XamlTreeNode> Elements(this XamlTreeNode item)
        {
            ILinqTree<XamlTreeNode> adapter = new XamlTreeNodeAdapter(item);
            foreach (var child in adapter.Children())
            {
                yield return child;
            }
        }

        /// <summary>
        /// Returns a collection of the sibling elements before this node, in document order.
        /// </summary>
        public static IEnumerable<XamlTreeNode> ElementsBeforeSelf(this XamlTreeNode item)
        {
            if (item.Ancestors().FirstOrDefault() == null)
                yield break;
            foreach (var child in item.Ancestors().First().Elements())
            {
                if (child.Equals(item))
                    break;
                yield return child;
            }
        }

        /// <summary>
        /// Returns a collection of the elements after this node, in document order.
        /// </summary>
        public static IEnumerable<XamlTreeNode> ElementsAfterSelf(this XamlTreeNode item)
        {
            if (item.Ancestors().FirstOrDefault() == null)
                yield break;
            bool afterSelf = false;
            foreach (var child in item.Ancestors().First().Elements())
            {
                if (afterSelf)
                    yield return child;

                if (child.Equals(item))
                    afterSelf = true;
            }
        }

        /// <summary>
        /// Returns a collection containing this element and all child elements.
        /// </summary>
        public static IEnumerable<XamlTreeNode> ElementsAndSelf(this XamlTreeNode item)
        {
            yield return item;

            foreach (var child in item.Elements())
            {
                yield return child;
            }
        }

        /// <summary>
        /// Returns a collection of descendant elements which match the given type.
        /// </summary>
        public static IEnumerable<XamlTreeNode> Descendants<T>(this XamlTreeNode item)
        {
            return item.Descendants().Where(i => i is T).Cast<XamlTreeNode>();
        }



        /// <summary>
        /// Returns a collection of the sibling elements before this node, in document order
        /// which match the given type.
        /// </summary>
        public static IEnumerable<XamlTreeNode> ElementsBeforeSelf<T>(this XamlTreeNode item)
        {
            return item.ElementsBeforeSelf().Where(i => i is T).Cast<XamlTreeNode>();
        }

        /// <summary>
        /// Returns a collection of the after elements after this node, in document order
        /// which match the given type.
        /// </summary>
        public static IEnumerable<XamlTreeNode> ElementsAfterSelf<T>(this XamlTreeNode item)
        {
            return item.ElementsAfterSelf().Where(i => i is T).Cast<XamlTreeNode>();
        }

        /// <summary>
        /// Returns a collection containing this element and all descendant elements
        /// which match the given type.
        /// </summary>
        public static IEnumerable<XamlTreeNode> DescendantsAndSelf<T>(this XamlTreeNode item)
        {
            return item.DescendantsAndSelf().Where(i => i is T).Cast<XamlTreeNode>();
        }

        /// <summary>
        /// Returns a collection of ancestor elements which match the given type.
        /// </summary>
        public static IEnumerable<XamlTreeNode> Ancestors<T>(this XamlTreeNode item)
        {
            return item.Ancestors().Where(i => i is T).Cast<XamlTreeNode>();
        }

        /// <summary>
        /// Returns a collection containing this element and all ancestor elements
        /// which match the given type.
        /// </summary>
        public static IEnumerable<XamlTreeNode> AncestorsAndSelf<T>(this XamlTreeNode item)
        {
            return item.AncestorsAndSelf().Where(i => i is T).Cast<XamlTreeNode>();
        }

        /// <summary>
        /// Returns a collection of child elements which match the given type.
        /// </summary>
        public static IEnumerable<XamlTreeNode> Elements<T>(this XamlTreeNode item)
        {
            return item.Elements().Where(i => i is T).Cast<XamlTreeNode>();
        }

        /// <summary>
        /// Returns a collection containing this element and all child elements.
        /// which match the given type.
        /// </summary>
        public static IEnumerable<XamlTreeNode> ElementsAndSelf<T>(this XamlTreeNode item)
        {
            return item.ElementsAndSelf().Where(i => i is T).Cast<XamlTreeNode>();
        }

    }
}
