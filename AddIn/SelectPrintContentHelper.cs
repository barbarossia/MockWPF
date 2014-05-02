using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace AddIn
{
    public class SelectPrintContentHelper
    {
        public static DependencyObject SearchDependencyObject(DependencyObject startObject, Type filterType)
        {
            Contract.Requires(startObject != null);
            Contract.Requires(filterType != null);

            return SearchDependencyObjects(startObject, filterType).FirstOrDefault();
        }

        public static Point GetRelativeOffset(Visual child, Visual parent)
        {
            Contract.Requires(child != null);
            Contract.Requires(parent != null);

            Point relativePoint = child.TransformToAncestor(parent)
                              .Transform(new Point(0, 0));
            return relativePoint;
        }

        public static IEnumerable<DependencyObject> SearchDependencyObjects(DependencyObject startObject, Type filterType)
        {
            Queue<DependencyObject> searchQueues = new Queue<DependencyObject>();
            searchQueues.Enqueue(startObject);

            while (searchQueues.Count > 0)
            {
                DependencyObject currentObj = searchQueues.Dequeue();
                if (currentObj == null)
                {
                    continue;
                }

                int count = VisualTreeHelper.GetChildrenCount(currentObj);
                for (int i = 0; i < count; i++)
                {
                    DependencyObject obj = VisualTreeHelper.GetChild(currentObj, i);
                    if (filterType.IsAssignableFrom(obj.GetType()))
                    {
                        yield return obj;
                    }
                    else
                    {
                        searchQueues.Enqueue(obj);
                    }
                }
            }
        }
    }
}
