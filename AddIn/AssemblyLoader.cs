using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AddIn
{
    public class AssemblyLoader : MarshalByRefObject
    {
        private string assemblyFullPath;
        private readonly HashSet<AssemblyName> referencedAssemblies;

        /// <summary>
        /// List of the referenced assemblies for the assembly being analyzed.
        /// </summary>
        public IEnumerable<AssemblyName> ReferencedAssemblies
        {
            get { return referencedAssemblies.AsEnumerable(); }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public AssemblyLoader()
        {
            referencedAssemblies = new HashSet<AssemblyName>();
            //Event that fires when an assembly cannot be resolved
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainAssemblyResolve;
        }

        /// <summary>
        /// Loads an assembly to check for its references and activities.
        /// </summary>
        /// <param name="assemblyPath">Location of the assembly</param>
        /// <param name="item">Assembly item loaded with reflection and xaml buildtask analysis</param>
        public void LoadReflectedInfoInSubdomain(string assemblyPath, out ActivityAssemblyItem item)
        {
            List<AssemblyName> xamlReferences = new List<AssemblyName>();

            item = null;

            if (string.IsNullOrEmpty(assemblyPath))
            {
                throw new ArgumentNullException("assemblyPath");
            }

            assemblyFullPath = assemblyPath;

            // Get assembly and use it to compute activities and references.
            // Any exception in this method should be caught in the inspection service for exception  bubbling.
            var assembly = Assembly.ReflectionOnlyLoadFrom(assemblyFullPath);
            item = new ActivityAssemblyItem(assembly);

            if (assembly.GetType("XamlStaticHelperNamespace._XamlStaticHelper") != null)
            {
                xamlReferences = GetXamlReferences();
            }

            var reflectedReferencedAssemblies = (from assemblyName in assembly.GetReferencedAssemblies()
                                                 select assemblyName)
                                                .Union(xamlReferences);

            //Store the references of the current assembly
            item.ReferencedAssemblies = new ObservableCollection<AssemblyName>(reflectedReferencedAssemblies);
        }

        /// <summary>
        /// When a reference cannot be resolved, we need to add it to the list of references to check.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        Assembly CurrentDomainAssemblyResolve(object sender, ResolveEventArgs eventArgs)
        {
            var referenceName = ActivityAssemblyItem.TreatUnsignedAsUnversioned(new AssemblyName(eventArgs.Name));
            referencedAssemblies.Add(referenceName);
            return null;
        }

        private List<AssemblyName> GetXamlReferences()
        {
            // XAML dependencies don't show up in Assembly.GetReferencedAssemblies.
            // However, XamlBuildTask creates an internal class _XamlStaticHelper in every
            // XAML-compiled assembly which does know which assemblies it needs to load for 
            // the XAML to work. Normally this class is just called to create a SchemaContext
            // during InitializeComponent, but here in the authoring tool we will call it
            // directly via private reflection. We will keep track of every assembly it 
            // attempts to load, and those are the XamlRefs.

            // Create non-ReflectionOnly assembly so we can run XamlStaticHelperNamespace._XamlStaticHelper.LoadAssemblies
            var assemblyElement = Assembly.LoadFrom(assemblyFullPath);
            var xamlStaticHelperType = assemblyElement.GetType("XamlStaticHelperNamespace._XamlStaticHelper");
            List<AssemblyName> result = new List<AssemblyName>();

            try
            {
                var loadedAssemblies = xamlStaticHelperType.InvokeMember("LoadAssemblies", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.InvokeMethod, null, null, new object[0]) as IEnumerable<Assembly>;

                // Any XAML-referenced assemblies which were successfully loaded (e.g. because they were in the same directory), except for
                // the assembly we are analyzing (it is not its own dependency).
                var xamlReferences = from loadedAssembly in loadedAssemblies
                                     where loadedAssembly != assemblyElement
                                     select loadedAssembly.GetName();

                foreach (var item in xamlReferences)
                {
                    referencedAssemblies.Add(item);
                }

                result = xamlReferences.ToList();
            }
            catch (TargetInvocationException)
            {
                //The dependency checking should continue even if this assembly could not be loaded
                //The user would have to provide the location.
            }

            return result;
        }
    }
}
