using AddIn.Common.ExceptionHandling;
using AddIn.Common.Message;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;

namespace AddIn
{
    public class AssemblyInspectionService
    {

        /// <summary>
        /// Default extension to recognize as valid for assembly files.
        /// </summary>
        private const string ValidAssemblyExtension = ".dll";

        /// <summary>
        /// Referenced assemblies that have been analized
        /// </summary>
        private readonly Dictionary<Tuple<string, Version>, string> analyzedReferences;

        /// <summary>
        /// Temporary collection to contain the references that need to be analized 
        /// </summary>
        private readonly Dictionary<Tuple<string, Version>, string> referencesToCheck;

        /// <summary>
        /// List of assembly references of the source assembly
        /// </summary>
        private readonly HashSet<ActivityAssemblyItem> referencedAssemblies;

        /// <summary>
        /// Source assembly being analized (obtained from the file path)
        /// </summary>
        private ActivityAssemblyItem sourceAssembly;

        /// <summary>
        /// Stores any exception thrown during the inspection process.
        /// </summary>
        public Exception OperationException { get; set; }


        /// <summary>
        /// Assembly references of the source assembly 
        /// </summary>
        public IEnumerable<ActivityAssemblyItem> ReferencedAssemblies
        {
            get { return referencedAssemblies.AsEnumerable(); }
        }

        /// <summary>
        /// The assembly obtained by doing reflection in the file path specified 
        /// </summary>
        public ActivityAssemblyItem SourceAssembly
        {
            get { return sourceAssembly; }
        }

        /// <summary>
        /// Default constructor for the class
        /// </summary>
        public AssemblyInspectionService()
        {
            referencedAssemblies = new HashSet<ActivityAssemblyItem>();
            analyzedReferences = new Dictionary<Tuple<string, Version>, string>();
            referencesToCheck = new Dictionary<Tuple<string, Version>, string>();
        }

        /// <summary>
        /// Analizes an assembly (specified in the parameter), to discover all its references and
        /// the activities contained on it. 
        /// </summary>
        /// <param name="assemblyPath">Path of the assembly to analize</param>
        public bool Inspect(string assemblyPath)
        {

            if (string.IsNullOrEmpty(assemblyPath))
            {
                throw new ArgumentNullException("assemblyPath");
            }

            //Create a new appdomain, so we don't load assemblies in the main app domain and 
            //we can unload the assemblies from memory when the operation finishes.
            var appDomain = CreateTempAppDomain(AppDomain.CurrentDomain);

            // Setup
            var typeName = typeof(AssemblyLoader).FullName;
            var location = typeof(AssemblyLoader).Assembly.Location;

            if (!String.IsNullOrEmpty(typeName) && !String.IsNullOrEmpty(location))
            {
                // Run computation and tear down AppDomain
                var subloader = (AssemblyLoader)appDomain.CreateInstanceFromAndUnwrap(location, typeName);

                try
                {
                    ResolveReferences(subloader, assemblyPath, null, true);
                }
                catch (AssemblyInspectionException ex)
                {
                    //Could not analyze the main assembly, so we encapsulate the exception to return it as a result.
                    OperationException = ex;
                    return false;
                }

                string directory = Path.GetDirectoryName(assemblyPath);
                while (referencesToCheck.Any())
                {
                    Tuple<string, Version> element = referencesToCheck.First().Key;
                    if (element.Item1 == null || element.Item2 == null)
                    {
                        referencesToCheck.Remove(element);
                        continue;
                    }

                    string referenceName = element.Item1;
                    string referencePath = referenceName + ".dll"; //referenceName + ".dll";
                    string referenceVersion = element.Item2.ToString();
                    if (!string.IsNullOrEmpty(directory))
                    {
                        //Look for the missing assembly in the selected location and the subfolders
                        var matchesInDirectory = Directory.EnumerateFiles(directory, referencePath, SearchOption.AllDirectories);
                        if (matchesInDirectory.Any())
                        {
                            referencePath = Path.Combine(matchesInDirectory.First());
                        }
                        else
                        {
                            //var matchesInTargetDirectory = Directory.EnumerateFiles(FileService.GetAssembliesDirectoryPath(), referencePath, SearchOption.AllDirectories);
                            //if (matchesInTargetDirectory.Any())
                            //{
                            //    referencePath = Path.Combine(matchesInDirectory.First());
                            //}
                            referencePath = FileService.GetAssembliesDirectoryPath() + "\\" + referenceName + "\\" + referenceVersion + "\\" + referencePath;
                        }


                    }

                    try
                    {
                        if (File.Exists(referencePath))
                        {
                            ResolveReferences(subloader, referencePath, element.Item2, false);
                            analyzedReferences.Add(element, referencePath);
                        }
                        else
                        {
                            analyzedReferences.Add(element, null);
                            string name = Path.GetFileNameWithoutExtension(referencePath);
                            if (!string.IsNullOrEmpty(name))
                            {
                                var item = new ActivityAssemblyItem(new AssemblyName(name)) { Version = element.Item2 };
                                referencedAssemblies.Add(item);
                            }
                        }


                    }
                    catch (AssemblyInspectionException)
                    {
                        //Error resolving one reference, in this case we can continue the analysis because it's not the
                        //main assembly being analyzed. We add the assembly with an empty location to the collection.
                        analyzedReferences.Add(element, null);

                        //Assembly is not the main element being analyzed, so we just add it with empty properties to the list 
                        //(the inspection should continue for the other references).
                        string name = Path.GetFileNameWithoutExtension(referencePath);
                        if (!string.IsNullOrEmpty(name))
                        {
                            var item = new ActivityAssemblyItem(new AssemblyName(name)) { Version = element.Item2 };
                            referencedAssemblies.Add(item);
                        }
                    }

                    referencesToCheck.Remove(element);
                }

                AppDomain.Unload(appDomain);
            }

            return true;
        }

        /// <summary>
        /// Resolve the references of an assembly using the assembly loader
        /// </summary>
        /// <param name="assemblyLoader">Assembly loader object created in the app domain</param>
        /// <param name="assemblyPath">Path to the assembly</param>
        /// <param name="assemblyVersion">Version of the assembly to search for</param>
        /// <param name="isRootAssembly">Indicates if the assembly to analize is the main assembly of this inspection.</param>
        /// <returns></returns>
        private void ResolveReferences(AssemblyLoader assemblyLoader, string assemblyPath, Version assemblyVersion, bool isRootAssembly)
        {
            try
            {
                //Load the assembly in subdomain to analyze it with reflection.
                ActivityAssemblyItem assemblyItem;

                assemblyLoader.LoadReflectedInfoInSubdomain(assemblyPath, out assemblyItem);

                //Store root assembly information to return it to the main app domain
                if (isRootAssembly)
                {
                    sourceAssembly = assemblyItem;
                }
                else
                {
                    referencedAssemblies.Add(assemblyItem);
                }

                //Add the list of references discovered through reflection
                //to analyze them as part of the import process.
                foreach (var item in assemblyItem.ReferencedAssemblies.Where(item => !AssemblyIsBuiltIn(item)))
                {
                    if (!referencesToCheck.ContainsKey(Tuple.Create(item.Name, item.Version)) &&
                        (!analyzedReferences.ContainsKey(Tuple.Create(item.Name, item.Version))))
                    {
                        referencesToCheck.Add(Tuple.Create(item.Name, item.Version), null);
                    }
                }

                //if the inspection produced some references that were not resolved, add them too.
                if (assemblyLoader.ReferencedAssemblies.Any())
                {
                    foreach (var item in assemblyLoader.ReferencedAssemblies.Where(item => !AssemblyIsBuiltIn(item)))
                    {
                        if (!referencesToCheck.ContainsKey(Tuple.Create(item.Name, item.Version)) &&
                            (!analyzedReferences.ContainsKey(Tuple.Create(item.Name, item.Version))))
                        {
                            referencesToCheck.Add(Tuple.Create(item.Name, item.Version), null);
                        }
                    }
                }

            }
            catch (FileNotFoundException ex)
            {
                // assemblyName was not found.
                throw new AssemblyInspectionException(ex.Message, ex);
            }
            catch (TypeLoadException ex)
            {
                //typeName was not found in assemblyName.
                throw new AssemblyInspectionException(ex.Message, ex);
            }
            catch (BadImageFormatException ex)
            {
                //assemblyName is not a valid assembly.
                throw new AssemblyInspectionException(ex.Message, ex);
            }
            catch (FileLoadException ex)
            {
                //An assembly or module was loaded twice with two different evidences.
                throw new AssemblyInspectionException(ex.Message, ex);
            }
            catch (PathTooLongException ex)
            {
                //An assembly or module was loaded twice with two different evidences.
                throw new AssemblyInspectionException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Creates a new AppDomain based on the parent AppDomain
        /// </summary>
        /// <param name="parentAppDomain">The parent AppDomain</param>
        /// <returns>A newly created AppDomain</returns>
        private AppDomain CreateTempAppDomain(AppDomain parentAppDomain)
        {
            var evidence = new Evidence(parentAppDomain.Evidence);
            AppDomainSetup setupInfo = parentAppDomain.SetupInformation;
            return AppDomain.CreateDomain("AssemblyInspection", evidence, setupInfo);
        }

        /// <summary>
        /// The assembly is built in.
        /// </summary>
        /// <param name="assemblyName">
        /// The assembly name.
        /// </param>
        /// <returns>
        /// The bool value indicates if assembly is built in.
        /// </returns>
        public static bool AssemblyIsBuiltIn(AssemblyName assemblyName)
        {
            return AssemblyIsBuiltIn(assemblyName.FullName);
        }

        /// <summary>
        /// The assembly is built in.
        /// </summary>
        /// <param name="assemblyFullName">
        /// The assembly full name.
        /// </param>
        /// <returns>
        /// The value indicate if a assembly is .NET built-in.
        /// </returns>
        public static bool AssemblyIsBuiltIn(string assemblyFullName)
        {
            // This list contains public tokens used by .NET Framework
            var builtInTokens = new List<string> { "31bf3856ad364e35", "b77a5c561934e089", "b03f5f7f11d50a3a", };

            string token = GetPublicTokenFromAssemblyFullName(assemblyFullName);
            string name = GetNameFromAssemblyFullName(assemblyFullName);
            return builtInTokens.Contains(token) && Utility.BuiltinAssemblies.Contains(name);
        }

        /// <summary>
        /// To get public token from assemlby full name
        /// </summary>
        /// <param name="assemblyFullName">
        /// The full name of assembly
        /// </param>
        /// <returns>
        /// The public token of assmably
        /// </returns>
        public static string GetPublicTokenFromAssemblyFullName(string assemblyFullName)
        {
            string token = assemblyFullName.Substring(assemblyFullName.LastIndexOf('=') + 1).ToLower();
            return token;
        }

        /// <summary>
        /// The get name from assembly full name.
        /// </summary>
        /// <param name="assemblyFullName">
        /// The assembly full name.
        /// </param>
        /// <returns>
        /// The name (short name) of a assembly.
        /// </returns>
        public static string GetNameFromAssemblyFullName(string assemblyFullName)
        {
            if (null == assemblyFullName)
                return string.Empty;

            // Example -- "System.Activities, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
            string[] parts = assemblyFullName.Split(',');
            return parts[0];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assemblyPath"></param>
        public static void CheckAssemblyPath(string assemblyPath)
        {
            if (string.IsNullOrEmpty(assemblyPath))
            {
                throw new ArgumentNullException("assemblyPath");
            }

            if (Path.GetExtension(assemblyPath) != ValidAssemblyExtension)
            {
                throw new ArgumentOutOfRangeException("assemblyPath");
            }

            if (!File.Exists(assemblyPath))
            {
                throw new FileNotFoundException(ImportMessages.FileNotFound);
            }

            Assembly assemblyInfo;

            try
            {
                assemblyInfo = Assembly.ReflectionOnlyLoadFrom(assemblyPath);
            }
            catch (System.BadImageFormatException)
            {
                throw new AssemblyInspectionException(ImportMessages.AssemblyNotAnalyzed);
            }
            catch (System.IO.FileLoadException)
            {
                throw new AssemblyInspectionException(ImportMessages.AssemblyAlreadyImported);
            }

            if (Caching.ActivityAssemblyItems.Any(assemblyItem => assemblyItem.Name == assemblyInfo.GetName().Name &&
                                                    (assemblyItem.Version == assemblyInfo.GetName().Version)))
            {
                throw new AssemblyInspectionException(ImportMessages.AssemblyAlreadyImported);
            }

            if (assemblyInfo == null || assemblyInfo.GetName().GetPublicKeyToken().Length == 0)
            {
                throw new AssemblyInspectionException(assemblyInfo == null ? ImportMessages.AssemblyNull : ImportMessages.AssemblyUnsigned);
            }
        }
    }
}
