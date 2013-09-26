using AddIn.Common.ExceptionHandling;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace AddIn
{
    public class Caching
    {
        /// <summary>
        /// Initializes static members of the <see cref="Caching"/> class.
        /// </summary>
        static Caching()
        {
            ActivityAssemblyItems = new ObservableCollection<ActivityAssemblyItem>();
            ActivityItems = new ObservableCollection<ActivityItem>();
        }

        /// <summary>
        /// Gets or sets all cached ActivityAssemblyItems.
        /// </summary>
        public static ObservableCollection<ActivityAssemblyItem> ActivityAssemblyItems { get; set; }

        /// <summary>
        /// Gets or sets all cached ActivityItems.
        /// </summary>
        public static ObservableCollection<ActivityItem> ActivityItems { get; set; }

        /// <summary>
        /// Reload data from local caching categories.
        /// </summary>
        public static void Refresh()
        {
            // Serialize and save
            using (var stream = File.Open(Utility.GetActivityAssemblyCatalogFileName(), FileMode.Create))
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, ActivityAssemblyItems.ToList());
            }

            LoadFromLocal();   // Reload from the local cache.
        }

        /// <summary>
        /// Load caching from local hard drive.
        /// </summary>
        public static void LoadFromLocal()
        {
            ActivityAssemblyItems.Clear();
            ActivityItems.Clear();

            if (File.Exists(Utility.GetActivityAssemblyCatalogFileName()))
            {
                var deserialized = Utility.DeserializeSavedContent(Utility.GetActivityAssemblyCatalogFileName()) as IEnumerable<ActivityAssemblyItem>;
                ActivityAssemblyItems = new ObservableCollection<ActivityAssemblyItem>(deserialized);
                ActivityAssemblyItems
                    .SelectMany(activityAssemblyItem => activityAssemblyItem.ActivityItems)
                    .ToList()
                    .ForEach(item => 
                    { 
                        ActivityItems.Add(item); 
                        item.UserSelected = true; 
                        item.Category = "Unassigned"; 
                    });
                ActivityAssemblyItems.ToList().ForEach(item => 
                {
                    item.Assembly = Assembly.LoadFrom(item.Location);
                });
            }
            else
            {
                Load();
            }
        }

        private static void Load()
        {
            var root = Utility.GetAssembliesDirectoryPath();
            foreach (var file in Directory.GetFiles(root))
            {
                InspectAssembly(file);
            }

            Refresh();
        }

        private static void InspectAssembly(string assemblyPath)
        {
            var inspection = new AssemblyInspectionService();

            if (!inspection.Inspect(assemblyPath))
            {
                throw new UserFacingException(inspection.OperationException.Message, inspection.OperationException);
            }

            var newAssembly = inspection.SourceAssembly;
            if (!ActivityAssemblyItems.Any(
                c => c.Name == newAssembly.Name && 
                    c.Version == newAssembly.Version))
                ActivityAssemblyItems.Add(newAssembly);

        }

        public static void CacheAssembly(List<ActivityAssemblyItem> activityAssemblyItems, bool isFromServer = false)
        {
            if (activityAssemblyItems == null)
            {
                throw new ArgumentNullException("activityAssemblyItems");
            }

            if (activityAssemblyItems.Any(item => item == null))
            {
                throw new ArgumentNullException("activityAssemblyItems");
            }

            foreach (ActivityAssemblyItem assemblyItem in activityAssemblyItems)
            {
                // Skip cached item
                if (assemblyItem.CachingStatus == CachingStatus.Latest)
                {
                    continue;
                }

                // Check if a location is already in location catalog. If true, remove it first.
                ActivityAssemblyItem cachedAssembly;
                if (Utility.LoadCachedAssembly(ActivityAssemblyItems, assemblyItem.AssemblyName, out cachedAssembly))
                {
                    ActivityAssemblyItems.Remove(cachedAssembly);
                }

                // Copy assemblies to local caching directory
                string destFileName = Utility.CopyAssemblyToLocalCachingDirectory(assemblyItem.AssemblyName, assemblyItem.Location, false);
                // break link to original location by resetting Location and AssemblyName.CodeBase
                assemblyItem.Location = destFileName;
                assemblyItem.AssemblyName.CodeBase = null;
                assemblyItem.UpdateDateTime = DateTime.Now;
                assemblyItem.CachingStatus = CachingStatus.Latest;

                if (isFromServer)
                {
                    var inspection = Utility.GetAssemblyInspectionService();
                    inspection.Inspect(destFileName);
                    assemblyItem.ActivityItems = inspection.SourceAssembly.ActivityItems;
                    assemblyItem.UserSelected = true;
                    assemblyItem.ActivityItems.ToList().ForEach(i =>
                    {
                        i.UserSelected = true;
                        i.Category = "Unassigned";
                    });
                }

                // Make ActivityItem read only. Note: ActivityItem's metadata can be edited only when imported.
                foreach (var activityItem in assemblyItem.ActivityItems)
                {
                    activityItem.IsReadOnly = true;
                    activityItem.CachingStatus = CachingStatus.Latest;
                }

                ActivityAssemblyItems.Add(assemblyItem);
            }
        }
    }
}
