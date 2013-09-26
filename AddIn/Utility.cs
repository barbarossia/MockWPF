using System;
using System.Activities;
using System.Activities.Presentation;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.ServiceModel.Activities;
using System.Text;

namespace AddIn
{
    public static class Utility
    {
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
            return builtInTokens.Contains(token) && BuiltinAssemblies.Contains(name);
        }

        public static string GetNameFromAssemblyFullName(string assemblyFullName)
        {
            if (null == assemblyFullName)
                return string.Empty;

            //// Example -- "System.Activities, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
            string[] parts = assemblyFullName.Split(',');
            return parts[0];
        }

        public static string GetPublicTokenFromAssemblyFullName(string assemblyFullName)
        {
            string token = assemblyFullName.Substring(assemblyFullName.LastIndexOf('=') + 1).ToLower();
            return token;
        }

        public static string[] BuiltinAssemblies
        {
            get
            {
                return new string[] 
                {
                    "System",
                    "System.Configuration",
                    "Gsfx", 
                    "Microsoft.Practices",
                    "Accessibility",
                    "AdoNetDiag",
                    "alink",
                    "AspNetMMCExt",
                    "aspnet_filter",
                    "aspnet_isapi",
                    "Aspnet_perf",
                    "aspnet_rc",
                    "clr",
                    "clretwrc",
                    "clrjit",
                    "CORPerfMonExt",
                    "Culture",
                    "CustomMarshalers",
                    "dfdll",
                    "diasymreader",
                    "EventLogMessages",
                    "FileTracker",
                    "fusion",
                    "InstallUtilLib",
                    "ISymWrapper",
                    "Microsoft.Build.Conversion.v4.0",
                    "Microsoft.Build",
                    "Microsoft.Build.Engine",
                    "Microsoft.Build.Framework",
                    "Microsoft.Build.Tasks.v4.0",
                    "Microsoft.Build.Utilities.v4.0",
                    "Microsoft.CSharp",
                    "Microsoft.Data.Entity.Build.Tasks",
                    "Microsoft.JScript",
                    "Microsoft.Transactions.Bridge",
                    "Microsoft.Transactions.Bridge.Dtc",
                    "Microsoft.VisualBasic.Activities.Compiler",
                    "Microsoft.VisualBasic.Compatibility.Data",
                    "Microsoft.VisualBasic.Compatibility",
                    "Microsoft.VisualBasic",
                    "Microsoft.VisualBasic.Vsa",
                    "Microsoft.VisualC",
                    "Microsoft.VisualC.STLCLR",
                    "Microsoft.Vsa",
                    "Microsoft.Windows.ApplicationServer.Applications",
                    "Microsoft_VsaVb",
                    "MmcAspExt",
                    "mscordacwks",
                    "mscordbi",
                    "mscoreei",
                    "mscoreeis",
                    "mscorlib",
                    "mscorpe",
                    "mscorpehost",
                    "mscorrc",
                    "mscorsecimpl",
                    "mscorsn",
                    "mscorsvc",
                    "nlssorting",
                    "normalization",
                    "PerfCounter",
                    "peverify",
                    "SbsNclPerf",
                    "ServiceModelEvents",
                    "ServiceModelInstallRC",
                    "ServiceModelPerformanceCounters",
                    "ServiceModelRegUI",
                    "ServiceMonikerSupport",
                    "SMDiagnostics",
                    "SOS",
                    "sysglobl",
                    "System.Activities.Core.Presentation",
                    "System.Activities",
                    "System.Activities.DurableInstancing",
                    "System.Activities.Presentation",
                    "System.AddIn.Contract",
                    "System.AddIn",
                    "System.ComponentModel.Composition",
                    "System.ComponentModel.DataAnnotations",
                    "System.configuration",
                    "System.Configuration.Install",
                    "System.Core",
                    "System.Data.DataSetExtensions",
                    "System.Data",
                    "System.Data.Entity.Design",
                    "System.Data.Entity",
                    "System.Data.Linq",
                    "System.Data.OracleClient",
                    "System.Data.Services.Client",
                    "System.Data.Services.Design",
                    "System.Data.Services",
                    "System.Data.SqlXml",
                    "System.Deployment",
                    "System.Design",
                    "System.Device",
                    "System.DirectoryServices.AccountManagement",
                    "System.DirectoryServices",
                    "System.DirectoryServices.Protocols",
                    "System",
                    "System.Drawing.Design",
                    "System.Drawing",
                    "System.Dynamic",
                    "System.EnterpriseServices",
                    "System.EnterpriseServices.Thunk",
                    "System.EnterpriseServices.Wrapper",
                    "System.IdentityModel",
                    "System.IdentityModel.Selectors",
                    "System.IO.Log",
                    "System.Management",
                    "System.Management.Instrumentation",
                    "System.Messaging",
                    "System.Net",
                    "System.Net.Http",
                    "System.Numerics",
                    "System.Runtime.Caching",
                    "System.Runtime.DurableInstancing",
                    "System.Runtime.Remoting",
                    "System.Runtime.Serialization",
                    "System.Runtime.Serialization.Formatters.Soap",
                    "System.Security",
                    "System.ServiceModel.Activation",
                    "System.ServiceModel.Activities",
                    "System.ServiceModel.Channels",
                    "System.ServiceModel.Discovery",
                    "System.ServiceModel.Internals",
                    "System.ServiceModel",
                    "System.ServiceModel.Routing",
                    "System.ServiceModel.ServiceMoniker40",
                    "System.ServiceModel.WasHosting",
                    "System.ServiceModel.Web",
                    "System.ServiceProcess",
                    "System.Transactions",
                    "System.Web.Abstractions",
                    "System.Web.ApplicationServices",
                    "System.Web.DataVisualization.Design",
                    "System.Web.DataVisualization",
                    "System.Web",
                    "System.Web.DynamicData.Design",
                    "System.Web.DynamicData",
                    "System.Web.Entity.Design",
                    "System.Web.Entity",
                    "System.Web.Extensions.Design",
                    "System.Web.Extensions",
                    "System.Web.Mobile",
                    "System.Web.RegularExpressions",
                    "System.Web.Routing",
                    "System.Web.Services",
                    "System.Windows.Forms.DataVisualization.Design",
                    "System.Windows.Forms.DataVisualization",
                    "System.Windows.Forms",
                    "System.Workflow.Activities",
                    "System.Workflow.ComponentModel",
                    "System.Workflow.Runtime",
                    "System.WorkflowServices",
                    "System.Xaml",
                    "System.Xaml.Hosting",
                    "System.Xml",
                    "System.Xml.Linq",
                    "System.Xml.Serialization",
                    "TLBREF",
                    "webengine",
                    "webengine4",
                    "WMINet_Utils",
                    "XamlBuildTask",
                    // WPF
                    "NaturalLanguage6",
                    "NlsData0009",
                    "NlsLexicons0009",
                    "PenIMC",
                    "PresentationBuildTasks",
                    "PresentationCore",
                    "PresentationFramework.Aero",
                    "PresentationFramework.Classic",
                    "PresentationFramework",
                    "PresentationFramework.Luna",
                    "PresentationFramework.Royale",
                    "PresentationHost_v0400",
                    "PresentationNative_v0400",
                    "PresentationUI",
                    "ReachFramework",
                    "System.Printing",
                    "System.Speech",
                    "System.Windows.Input.Manipulations",
                    "System.Windows.Presentation",
                    "UIAutomationClient",
                    "UIAutomationClientsideProviders",
                    "UIAutomationProvider",
                    "UIAutomationTypes",
                    "WindowsBase",
                    "WindowsFormsIntegration",
                    "wpfgfx_v0400"
                };
            }
        }

        public static List<Type> GetAllVisibleBuiltInActivityTypes()
        {
            // Load activities from System.Activities assembly
            Assembly activitiesAssembly = typeof(Activity).Assembly;

            List<Type> allSystemActivitiesTypes = GetAllActivityTypesInAssembly(activitiesAssembly);

            var usefulNamespace = new List<string> { "System.Activities.Statements", "System.Activities.Expressions", };

            var filter = new List<string>
                {
                    ////"AbortInstanceFlagValidator",
                    "Activity", 
                    "Activity`1", 
                    "ActivityWithResult", 
                    "ActivityWithResultWrapper`1", 
                    "Add`3", 
                    "AddToCollection`1", 
                    "AddValidationError", 
                    "And`3", 
                    "AndAlso", 
                    "ArgumentReference`1", 
                    "ArgumentValue`1", 
                    "ArrayItemReference`1", 
                    "ArrayItemValue`1", 
                    "As`2", 
                    "AssertValidation", 
                    "Assign", 
                    "Assign`1", 
                    "AsyncCodeActivity", 
                    "AsyncCodeActivity`1", 
                    "CancellationScope", 
                    "Cast`2", 
                    "ClearCollection`1", 
                    "CodeActivity", 
                    "CodeActivity`1", 
                    "CompensableActivity", 
                    "Compensate", 
                    ////"CompensationParticipant",
                    "Confirm", 
                    "Constraint", 
                    "Constraint`1", 
                    "CreateBookmarkScope", 
                    ////"DefaultCompensation",
                    ////"DefaultConfirmation",
                    "Delay", 
                    "DelegateArgumentReference`1", 
                    "DelegateArgumentValue`1", 
                    "DeleteBookmarkScope", 
                    "Divide`3", 
                    "DoWhile", 
                    "DynamicActivity", 
                    "DynamicActivity`1", 
                    "EmptyDelegateActivity", 
                    "Equal`3", 
                    "ExistsInCollection`1", 
                    "FieldReference`2", 
                    "FieldValue`2", 
                    "Flowchart", 
                    "ForEach`1", 
                    "GetChildSubtree", 
                    "GetParentChain", 
                    "GetWorkflowTree", 
                    "GreaterThan`3", 
                    "GreaterThanOrEqual`3", 
                    "HandleScope`1", 
                    "If", 
                    "IndexerReference`2", 
                    ////"InternalCompensate",
                    ////"InternalConfirm",
                    "InvokeAction", 
                    "InvokeAction`1", 
                    "InvokeAction`10", 
                    "InvokeAction`11", 
                    "InvokeAction`12", 
                    "InvokeAction`13", 
                    "InvokeAction`14", 
                    "InvokeAction`15", 
                    "InvokeAction`16", 
                    "InvokeAction`2", 
                    "InvokeAction`3", 
                    "InvokeAction`4", 
                    "InvokeAction`5", 
                    "InvokeAction`6", 
                    "InvokeAction`7", 
                    "InvokeAction`8", 
                    "InvokeAction`9", 
                    "InvokeDelegate", 
                    "InvokeFunc`1", 
                    "InvokeFunc`10", 
                    "InvokeFunc`11", 
                    "InvokeFunc`12", 
                    "InvokeFunc`13", 
                    "InvokeFunc`14", 
                    "InvokeFunc`15", 
                    "InvokeFunc`16", 
                    "InvokeFunc`17", 
                    "InvokeFunc`2", 
                    "InvokeFunc`3", 
                    "InvokeFunc`4", 
                    "InvokeFunc`5", 
                    "InvokeFunc`6", 
                    "InvokeFunc`7", 
                    "InvokeFunc`8", 
                    "InvokeFunc`9", 
                    "InvokeMethod", 
                    "InvokeMethod`1", 
                    ////"IsolationLevelValidator",
                    ////"LambdaReference`1",
                    ////"LambdaValue`1",
                    "LessThan`3", 
                    "LessThanOrEqual`3", 
                    "Literal`1", 
                    ////"LocationReferenceValue`1",
                    "MultidimensionalArrayItemReference`1", 
                    "Multiply`3", 
                    "NativeActivity", 
                    "NativeActivity`1", 
                    "New`1", 
                    "NewArray`1", 
                    "Not`2", 
                    "NotEqual`3", 
                    ////"ObtainType",
                    "Or`3", 
                    "OrElse", 
                    "Parallel", 
                    "ParallelForEach`1", 
                    "Persist", 
                    "Pick", 
                    ////"PickBranchBody",
                    "Pop", 
                    "PropertyReference`2", 
                    "PropertyValue`2", 
                    "RemoveFromCollection`1", 
                    "Rethrow", 
                    ////"RethrowBuildConstraint",
                    "Sequence", 
                    "Subtract`3", 
                    "Switch`1", 
                    "TerminateWorkflow", 
                    "Throw", 
                    "TransactionScope", 
                    "TryCatch", 
                    "ValueTypeFieldReference`2", 
                    "ValueTypeIndexerReference`2", 
                    "ValueTypePropertyReference`2", 
                    "VariableReference`1", 
                    "VariableValue`1", 
                    "VisualBasicReference`1", 
                    "VisualBasicValue`1", 
                    "While", 
                    ////"WorkflowCompensationBehavior",
                    "WriteLine", 
                };

            List<Type> availableActivityTypes = allSystemActivitiesTypes
                                                   .Where(type => usefulNamespace.Contains(type.Namespace) && filter.Contains(type.Name))
                                                   .ToList();
            availableActivityTypes.Add(typeof(System.Activities.Statements.PickBranch)); // not really an activity but it belongs in the toolbox anyway
            availableActivityTypes.Add(typeof(System.Activities.Statements.FlowDecision)); // not really an activity but it belongs in the toolbox anyway
            availableActivityTypes.Add(typeof(System.Activities.Statements.FlowSwitch<>)); // not really an activity but it belongs in the toolbox anyway
            availableActivityTypes.AddRange(GetAllActivityTypesInAssembly(typeof(Send).Assembly)); // don't need to filter the ones in System.ServiceModel.Activities

            return availableActivityTypes;
        }

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

        public static string TranslateActivityTypeName(string activityTypeName)
        {
            int position = activityTypeName.IndexOf('`');

            if (position < 0)
            {
                return activityTypeName;
            }

            string className = activityTypeName.Substring(0, position);
            string number = activityTypeName.Substring(position + 1);
            int x = int.Parse(number);
            var strArr = new string[x];

            for (int i = 0; i < x; i++)
            {
                strArr[i] = string.Format("T{0}", i + 1);
            }

            return string.Format("{0} <{1}>", className, string.Join(",", strArr));
        }

        public static string GetActivityAssemblyCatalogFileName()
        {
            string assembliesDirPath = GetAssembliesDirectoryPath();
            string catalogFilePath = string.Format(@"{0}\{1}.txt", assembliesDirPath, "ActivityAssemblyCatalog");
            return catalogFilePath;
        }

        public static object DeserializeSavedContent(string sourceFileName)
        {
            object result = null;
            if (File.Exists(sourceFileName))
            {
                using (Stream stream = File.Open(sourceFileName, FileMode.Open))
                {
                    var formatter = new BinaryFormatter();
                    result = formatter.Deserialize(stream);
                }
                return result;
            }

            throw new SerializationException(string.Format("File not found: {0}", sourceFileName));
        }

        public static Func<AssemblyInspectionService> GetAssemblyInspectionService = () => new AssemblyInspectionService();

        /// <summary>
        /// Get the local assemblies directory path. If not exists, create it.
        /// </summary>
        /// <returns>
        /// The assemblies directory path.
        /// </returns>
        public static string GetAssembliesDirectoryPath()
        {
            string assembliesDirPath = GetLocalDirectory("Assemblies");
            return assembliesDirPath;
        }

        public static string GetLocalDirectory(string directoryName)
        {
            string currentAssemblyPath = GetExecutingAssemblyPath();
            string result = Path.Combine(currentAssemblyPath, directoryName);

            if (!Directory.Exists(result))
            {
                Directory.CreateDirectory(result);
            }

            return result;
        }

        public static string GetExecutingAssemblyPath()
        {
            ////string path = Environment.CurrentDirectory;
            ////string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            const string path = ".";
            return path;
        }

        public static bool LoadCachedAssembly(
            IEnumerable<ActivityAssemblyItem> activityAssemblyItems,
            AssemblyName assemblyName,
            out ActivityAssemblyItem cachedAssembly)
        {
            cachedAssembly = activityAssemblyItems.FirstOrDefault(assembly => assembly.Matches(assemblyName));
            return cachedAssembly != null;
        }

        public static string CopyAssemblyToLocalCachingDirectory(
            AssemblyName assemblyName, string sourceFileName, bool forceOverride)
        {
            string destFileName = string.Empty;
            CopyAssembly(assemblyName, sourceFileName, out destFileName, false, forceOverride);
            return destFileName;
        }

        private static bool CopyAssembly(
            AssemblyName assemblyName,
            string sourceFileName,
            out string destFileName,
            bool removeSourceFile,
            bool forceOverride)
        {
            string assembliesDirPath = GetAssembliesDirectoryPath();
            string level1 = string.Format(@"{0}\{1}", assembliesDirPath, assemblyName.Name);

            if (!Directory.Exists(level1))
            {
                Directory.CreateDirectory(level1);
            }

            string level2 = string.Format(@"{0}\{1}", level1, assemblyName.Version.IfNotNull(v => v.ToString()) ?? "None");
            destFileName = string.Format(@"{0}\{1}.dll", level2, assemblyName.Name);

            if (Directory.Exists(level2))
            {
                if (forceOverride == false)
                {
                    return false; // Activity assembly already be there. And don't override.
                }
                else
                {
                    FileService.DeleteDirectory(level2);
                }
            }

            Directory.CreateDirectory(level2);

            if (removeSourceFile)
            {
                File.Move(sourceFileName, destFileName);
            }
            else
            {
                File.Copy(sourceFileName, destFileName);
            }

            return true;
        }

        public static Assembly Resolve(string requiredAssemblyFullName, IEnumerable<ActivityAssemblyItem> cache = null)
        {
            var reqAssemblyName = new AssemblyName(requiredAssemblyFullName);
            string assemblyName = reqAssemblyName.Name;
            // Resource files do not need to be resolved for the reshosted designer to work
            if (assemblyName.ToLower().EndsWith("resources"))
            {
                return null;
            }

            Assembly[] loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            Assembly result = loadedAssemblies.FirstOrDefault(a => a.FullName == requiredAssemblyFullName);
            // If the Assembly is NOT loaded in the requested Domain
            if (result == null)
            {
                // We first see if it an ActivityAssemblyItem in our cache
                ActivityAssemblyItem aai;
                result = LoadCachedAssembly(cache ?? Caching.ActivityAssemblyItems, reqAssemblyName, out aai) ? Assembly.LoadFrom(aai.Location) : null;
            }

            // TODO: Conditionally load the assembly from the Repository

            return result;
        }

    }
}
