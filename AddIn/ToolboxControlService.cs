using AddIn.Converters;
using System;
using System.Activities.Core.Presentation.Factories;
using System.Activities.Presentation.Toolbox;
using System.Activities.Statements;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AddIn
{
    public static class ToolboxControlService
    {
        // The Format of the Error Messages from trying to load assemblies at application startup
        private const string LoadExceptionErrorMessageFormat = "Error loading the authoring tool. Click Ok and try to open tool again.\n\n Error while loading: {0} Details: {1}\n";

        // Generic Exception error for Assembly load exceptions that do not contain LoaderExceptions.
        private const string ExceptionErrorMessageFormat = "Error loading: {0} Message: {1} Source: {2}\n";

        // List of all built in activity types.
        private static List<Type> allBuiltInActivityTypes = Utility.GetAllVisibleBuiltInActivityTypes();

        // List of basic activity types.
        private static readonly string[] basicActivityTypesFilter =
                                                            {
                                                                 "Assign",             "Delay",        "DoWhile",  "WriteLine",  "Sequence", 
                                                                 "ForEach`1",          "If",           "Parallel",  "Pick",      "PickBranch", 
                                                                 "ParallelForEach`1",  "While",        "Switch`1",  "Flowchart", 
                                                                 "FlowDecision",       "FlowSwitch`1", "Receive",   "SendReply"
                                                            };

        /// <summary>
        /// Convert ActivityAssemblyItems to ToolboxWrappers
        /// </summary>
        public static ToolboxControl CreateToolboxes(ObservableCollection<ActivityAssemblyItem> activityAssemblyItems)
        {
            var toolboxWrappers = new List<ToolboxCategory>();
            var toolbox = new ToolboxControl();

            CreateUserActivitiesToolbox(activityAssemblyItems, toolboxWrappers);
            CreateBasicActivitiesToolbox(toolboxWrappers);
            CreateAdvancedActivitiesToolbox(toolboxWrappers);

            foreach (var category in toolboxWrappers)
            {
                toolbox.Categories.Add(category);
            }

            return toolbox;
        }


        /// <summary>
        /// The initialize user activities toolbox.
        ///  Include the Favorite toolbox
        /// </summary>
        private static bool CreateUserActivitiesToolbox(IEnumerable<ActivityAssemblyItem> activityAssemblyItems, ICollection<ToolboxCategory> toolboxCategories)
        {
            var errorMessage = new StringBuilder();
            string fullActivityLibraryName = null;
            var isToolboxInitialized = false;
            var allCachedAssemblies = new List<Assembly>();

            // Load all local assemblies
            foreach (ActivityAssemblyItem assemblyItem in activityAssemblyItems)
            {
                // See if we even NEED to load this Activity Assembly
                if (assemblyItem.ActivityItems.All(assemblyItem1 => !assemblyItem1.UserSelected))
                {
                    // None of the Activities are marked as Selected then we skip loading this assembly
                    continue;
                }

                if (assemblyItem.Assembly == null)
                {
                    continue; // Caching is still busy refreshing assemblies. ToolboxControlService will be called again after refresh finishes.
                }

                // Get the FullName of the Activity Assembly
                fullActivityLibraryName = assemblyItem.AssemblyName.FullName;

                // Load the Referenced non .NET Assemblies
                var loadedAssembly = assemblyItem.Assembly;

                // Add the Assembly to the Cached assemblies
                allCachedAssemblies.Add(loadedAssembly);

                // Register designer metadata. If you don't do this, the design
                RegisterMetadtaInAssembly(assemblyItem);
            }
            // Load all cached ActivityItems
            var allCachedActivityItems = new List<ActivityItem>();

            foreach (ActivityAssemblyItem item in activityAssemblyItems)
            {
                if (item.UserSelected == false)
                {
                    continue;
                }

                IEnumerable<ActivityItem> userSelectedActivityItems = item.ActivityItems.Where(ai => ai.UserSelected);
                allCachedActivityItems.AddRange(userSelectedActivityItems);
            }

            var groups = allCachedActivityItems.GroupBy(ai => ai.Category).OrderBy(group => group.Key);

            // Generate toolbox categories
            ActivityTypeToToolTipConverter.ToolTipDictionary.Clear();

            var favoriteToolboxCategory = new ToolboxCategory("Favorite");
            toolboxCategories.Add(favoriteToolboxCategory);
            foreach (var group in groups)
            {
                var userToolboxCategory = new ToolboxCategory(group.Key);

                foreach (ActivityItem activityItem in group.OrderBy(act => act.Name))
                {
                    var parent = activityAssemblyItems.First(aai => aai.Matches(activityItem.ParentAssemblyName));
                    if (parent.Assembly == null)
                        continue;
                    var hostAssembly = parent.Assembly;
                    var activityType = hostAssembly.GetType(activityItem.FullName);

                    if (null == activityType || activityType.IsVisible == false)
                    {
                        continue;
                    }
                    // Make sure we check the dictionary to avoid inserting duplicate keys which throws an exception
                    if (!ActivityTypeToToolTipConverter.ToolTipDictionary.ContainsKey(activityType))
                    {
                        string versionString = parent.AssemblyName.Version == null
                            ? "No version"
                            : string.Format("v{0}", parent.Version);
                        var tooltip = string.IsNullOrWhiteSpace(activityItem.Description)
                            ? string.Format("{0}", versionString)
                            : string.Format("{0}, {1}", versionString, activityItem.Description);
                        ActivityTypeToToolTipConverter.ToolTipDictionary.Add(activityType, tooltip);
                        // TODO - We need to distinguish between Activity Libraries.
                        var userToolboxItemWrapper = new ToolboxItemWrapper(activityType, GetDisplayName(activityItem, activityType));
                        // AppDomain.CurrentDomain.AssemblyResolve event will be triggered here.
                        userToolboxCategory.Add(userToolboxItemWrapper);
                    }

                    if (activityItem.IsUserFavorite)
                    {
                        var favoriteToolboxItemWrapper = new ToolboxItemWrapper(activityType, GetDisplayName(activityItem, activityType));
                        favoriteToolboxCategory.Add(favoriteToolboxItemWrapper);
                    }
                }

                if (userToolboxCategory.Tools.Count > 0)
                {
                    toolboxCategories.Add(userToolboxCategory);
                }

            }

            isToolboxInitialized = true;

            return isToolboxInitialized;
        }



        /// <summary>
        /// The initialize developer activities toolbox.
        /// </summary>
        private static void CreateAdvancedActivitiesToolbox(ICollection<ToolboxCategory> toolboxWrappers)
        {
            var advancedActivityTypes = allBuiltInActivityTypes
                                            .Where(activityType => !basicActivityTypesFilter.Contains(activityType.Name))
                                            .ToList();
            var userControl = CreateToolboxControlFromActivityTypeList("Advanced", advancedActivityTypes);
            toolboxWrappers.Add(userControl);
        }

        /// <summary>
        /// The initialize basic activities toolbox.
        /// </summary>
        private static void CreateBasicActivitiesToolbox(ICollection<ToolboxCategory> toolboxWrappers)
        {
            var basicActivityTypes = allBuiltInActivityTypes
                                        .Where(activityType => basicActivityTypesFilter.Contains(activityType.Name) && activityType.IsVisible)
                                        .ToList();
            var userControl = CreateToolboxControlFromActivityTypeList("Basic Logic", basicActivityTypes);
            toolboxWrappers.Add(userControl);
        }

        /// <summary>
        /// Create toolbox control from activity type list, wrap each activity type into a ToolboxItemWrapper. Group them into ToolboxCategory against ActivityItem.Category property.
        /// </summary>
        /// <param name="activityTypes">
        /// The activity types.
        /// </param>
        /// <returns>
        /// The Toolbox control contains activity types input from parameter.
        /// </returns>
        private static ToolboxCategory CreateToolboxControlFromActivityTypeList(string categoryName, List<Type> activityTypes)
        {
            var toolboxCategory = new ToolboxCategory(categoryName);

            IOrderedEnumerable<ToolboxItemWrapper> wrappers =
                activityTypes.Select(activityType => new ToolboxItemWrapper(ReplaceWithActivityFactory(activityType), GetDisplayName(null, activityType)))
                             .OrderBy(wrapper => wrapper.DisplayName);

            foreach (ToolboxItemWrapper wrapper in wrappers)
            {
                toolboxCategory.Add(wrapper);
            }

            return toolboxCategory;
        }

        private static Type ReplaceWithActivityFactory(Type activityType)
        {
            if (activityType == typeof(ForEach<>))
                return typeof(ForEachWithBodyFactory<>);

            if (activityType == typeof(ParallelForEach<>))
                return typeof(ParallelForEachWithBodyFactory<>);

            return activityType;
        }


        /// <summary>
        /// The register metadata in assembly.
        /// </summary>
        /// <param name="assembly">
        /// The assembly.
        /// </param>
        private static void RegisterMetadtaInAssembly(ActivityAssemblyItem activityAssemblyItem)
        {
            var allMetadataTypes = activityAssemblyItem.Assembly
                                                          .GetAsManyTypesAsPossible(activityAssemblyItem)
                                                          .Where(t => t.GetInterface("IRegisterMetadata") != null)
                                                          .ToArray();
            foreach (var type in allMetadataTypes)
            {
                object instance = Activator.CreateInstance(type, null);
                MethodInfo registerMethodInfo = type.GetMethod("Register");
                registerMethodInfo.Invoke(instance, null);
            }
        }

        private static string GetDisplayName(ActivityItem activityItem, Type activityType)
        {
            string result = null;

            // If the activityItem record has a DisplayName, use that.
            if ((null != activityItem) && (!string.IsNullOrEmpty(activityItem.DisplayName)))
                result = activityItem.DisplayName;

            // Otherwise, if we can reflect over the type and pull out a property named "DisplayName", use that.
            if (string.IsNullOrEmpty(result) && !activityType.ContainsGenericParameters) // nothing else has set the result - try reflecting.
            {
                try
                {
                    var properties = activityType.GetProperties();
                    dynamic toolboxEntry = Activator.CreateInstance(activityType);

                    if (properties.Any(property => property.Name == "DesignerCaption")) // Some objects have this property...
                        result = toolboxEntry.DesignerCaption;

                    else if (properties.Any(property => property.Name == "DisplayName")) // ..but this property seems to be preferred by the .Net framework.
                        result = toolboxEntry.DisplayName;
                }
                catch
                {
                    // we could have failed because the class has generic (type) parameters, because it does not have a 
                    // public parameterless constructor, or any other number of reasons. If we do fail here, it's
                    // OK, we'll just make a human readable display out of the type name, later on in this method.
                }
            }

            // If all else fails, make a human readable display out of the type name.
            if (string.IsNullOrEmpty(result)) // nothing else has set the result - make the type name pretty.
                result = GetHumanReadableName(Utility.TranslateActivityTypeName(activityType.Name));

            return result;
        }

        /// <summary>
        /// for a given string, perform some cleanup to make it more human-readable. Insert spaces before capital letters, trim it, and so forth.
        /// </summary>
        /// <param name="rawName"></param>
        /// <returns>a more human readable version of the given string</returns>
        public static string GetHumanReadableName(string rawName)
        {
            const string ActivityPostFix = "Activity";
            const string SkipList = "01234567890.<>;':\\!@#$%^&*()_-+=?/\" ";
            var builder = new StringBuilder();
            string result;
            string[] sections;

            rawName = rawName.Trim();
            sections = rawName.Split(".".ToCharArray());
            rawName = sections[sections.Length - 1];

            // remove the "Activity" postfix, if the name has it
            if (rawName.EndsWith(ActivityPostFix, StringComparison.InvariantCultureIgnoreCase))
                rawName = rawName.Substring(0, rawName.Length - ActivityPostFix.Length);

            // put spaces in front of any capital letters, skipping any chars in the skip list
            rawName
                .ToList()
                .ForEach(c =>
                {
                    if ((c == c.ToString().ToUpper()[0]) && SkipList.IndexOf(c) < 0)
                        builder.Append(" ");

                    builder.Append(c);
                });

            result = builder.ToString();

            // remove spaces after anything in the skip list
            SkipList
                .ToList()
                .ForEach(c => result = result.Replace(c + " ", c.ToString()));


            // remove any spaces trailing after capital letters
            Enumerable.Range('A', 26)
                      .ToList()
                      .ForEach(char_as_int =>
                      {
                          string toShorten = ((char)char_as_int).ToString();
                          result = result.Replace(toShorten + " ", toShorten);
                      });

            return result.Trim();
        }

    }
}
