using System;
using System.Activities;
using System.Activities.Presentation;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AddIn
{
    public static class XamlHelper
    {
        /// <summary>
        /// Eliminate chained nullchecks for expressions a la LINQ "Select"
        /// </summary>
        public static TResult IfNotNull<T, TResult>(this T val, Func<T, TResult> select)
                where T : class
        {
            return val == null ? default(TResult) : @select(val);
        }

        /// <summary>
        /// Eliminate chained nullchecks for statements a la LINQ
        /// </summary>
        public static void IfNotNull<T>(this T val, Action<T> action)
                where T : class
        {
            if (val != null)
                action(val);
        }

        /// <summary>
        /// Query all types derives from System.Activities.Activity class
        /// </summary>
        /// <param name="assembly">
        /// Source assembly
        /// </param>
        /// <param name="activityAssemblyItem">
        /// The ActivityAssemblyItem, if any, which will own the given assembly. If there is a type load error 
        /// we will set activityAssemblyItem.NotSafeForTypeLoad = true to signal the WorkflowDesigner not to 
        /// load this assembly.
        /// </param>
        /// <returns>
        /// All activity types in the assembly.
        /// </returns>
        public static List<Type> GetAllActivityTypesInAssembly(Assembly assembly, ActivityAssemblyItem activityAssemblyItem = null)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException("assembly");
            }

            Type[] rawTypeList;

            if (assembly.ReflectionOnly)
            {
                Assembly targetAssembly = Assembly.LoadFrom(assembly.Location);
                rawTypeList = targetAssembly.GetAsManyTypesAsPossible(activityAssemblyItem);
            }
            else
            {
                rawTypeList = assembly.GetAsManyTypesAsPossible(activityAssemblyItem);
            }

            var activityTypeList = rawTypeList
                                      .Where(type => type.IsVisible && (type.IsSubclassOf(typeof(Activity))
                                                  || typeof(IActivityTemplateFactory).IsAssignableFrom(type)))
                                      .ToList();

            return activityTypeList;
        }

        /// <summary>
        /// Helper method
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="owningAssemblyItem">
        /// The ActivityAssemblyItem, if any, which will own the given assembly. If there is a type load error 
        /// we will set activityAssemblyItem.NotSafeForTypeLoad = true to signal the WorkflowDesigner not to 
        /// load this assembly. Will be null for built-in assemblies like System.Activities.
        /// </param>
        /// <returns></returns>
        public static Type[] GetAsManyTypesAsPossible(this Assembly assembly, ActivityAssemblyItem owningAssemblyItem = null)
        {
            try
            {
                var types = assembly.GetTypes();
                if (owningAssemblyItem != null)
                {
                    owningAssemblyItem.NotSafeForTypeLoad = false;
                }
                return types;
            }
            catch (ReflectionTypeLoadException e)
            {
                owningAssemblyItem.IfNotNull(owner => owner.NotSafeForTypeLoad = true);
                return e.Types.Where(t => t != null).ToArray();
            }
        }
    }
}
