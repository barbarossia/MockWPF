using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AddIn
{
    public static class AddInCaching
    {
        public static ObservableCollection<ActivityAssemblyItem> ActivityAssemblyItems { get; set; }
        public static HashSet<ActivityAssemblyItem> Conflict { get; private set; }
        public static ObservableCollection<ActivityAssemblyItem> Used { get; private set; }
        private static ActivityAssemblyItemNameVersionEqualityComparer comparer;

        static AddInCaching()
        {
            ActivityAssemblyItems = new ObservableCollection<ActivityAssemblyItem>();
            comparer = new ActivityAssemblyItemNameVersionEqualityComparer();
            Conflict = new HashSet<ActivityAssemblyItem>(comparer);
            Used = new ObservableCollection<ActivityAssemblyItem>();

            Used.CollectionChanged += used_CollectionChanged;
            ActivityAssemblyItems.CollectionChanged += ActivityAssemblyItems_CollectionChanged;
        }

        /// <summary>
        /// Load caching from local hard drive.
        /// </summary>
        public static void Load(IEnumerable<ActivityAssemblyItem> references)
        {
            ActivityAssemblyItems.Clear();

            foreach (var item in references)
            {
                if (string.IsNullOrEmpty(item.Location))
                    continue;
                item.Assembly = Assembly.LoadFrom(item.Location);

                // recompute if just-downloaded
                if (null == item.NotSafeForTypeLoad)
                {
                    item.Assembly.GetAsManyTypesAsPossible(owningAssemblyItem: item);
                }

                ActivityAssemblyItems.Add(item);   // Then, make them all available (ToolboxControlService will start picking them up)
            }
        }

        public static bool ImportAssemblies(IEnumerable<ActivityAssemblyItem> importAssemblies)
        {
            if (importAssemblies == null)
            {
                throw new ArgumentNullException("activityAssemblyItems");
            }

            if (importAssemblies.Any(item => item == null))
            {
                throw new ArgumentNullException("activityAssemblyItems");
            }

            var check = CheckConflictForImport(importAssemblies);

            ActivityAssemblyItems.Clear();

            Load(check);

            if (CheckMultipleVersionForImport(ActivityAssemblyItems))
            {
                return false;
            }

            return true;
        }

        private static void used_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (object item in e.NewItems)
                    {
                        ActivityAssemblyItem asm = item as ActivityAssemblyItem;
                        if (asm != null && CheckVersionForConflicts(Used, asm))
                        {
                            Conflict.Add(Used.First(u => CheckVersionForConflict(u, asm)));
                        }
                    }

                    break;
            }
        }

        private static void ActivityAssemblyItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (object item in e.NewItems)
                    {
                        ActivityAssemblyItem asm = item as ActivityAssemblyItem;
                        if (asm != null && !Used.Contains(asm, comparer))
                        {
                            Used.Add(asm);
                        }
                    }

                    break;
            }
        }

        private static bool CheckVersionForConflicts(IEnumerable<ActivityAssemblyItem> collection, ActivityAssemblyItem obj)
        {
            return (from c in collection
                    where CheckVersionForConflict(c, obj)
                    select c).Any();
        }

        /// <summary>
        /// Prevent adding multiple version of an activity in add-in
        /// </summary>
        /// <returns></returns>
        private static bool CheckVersionForConflict(ActivityAssemblyItem left, ActivityAssemblyItem right)
        {
            return left.Name == right.Name
                && left.Version != right.Version;
        }

        private static bool CheckMultipleVersionForImport(IEnumerable<ActivityAssemblyItem> import)
        {
            return (from c in Conflict
                    from i in import
                    where c.Matches(i)
                    select c).Any();
        }

        private static IEnumerable<ActivityAssemblyItem> CheckConflictForImport(IEnumerable<ActivityAssemblyItem> import)
        {
            var result = new List<ActivityAssemblyItem>();

            var conflicts = import.GroupBy(i => i.Name)
                .Select(group =>
                new
                {
                    Key = group.Key,
                    Count = group.Count(),
                    Collection = group.OrderByDescending(x => x.Version)
                });

            foreach (var c in conflicts)
            {
                if (c.Count > 1)
                {
                    result.Add(c.Collection.First());
                }
                else
                {
                    result.AddRange(c.Collection);
                }
            }

            return result;
        }
    }
}
