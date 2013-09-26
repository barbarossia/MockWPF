using System;
using System.Activities.Core.Presentation;
using System.AddIn.Contract;
using System.AddIn.Pipeline;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Threading;

namespace AddIn
{
    public class DesignerAddIn : MarshalByRefObject, IDesignerContract, IDisposable
    {
        private WorkflowEditorView workflowView;
        private ToolboxView toolbox;
        public WorkflowEditorViewModel WorkflowEditorVM { get; private set; }

        public DesignerAddIn()
        {
            AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly;
            new DesignerMetadata().Register();
            WorkflowEditorVM = new WorkflowEditorViewModel();
        }

        public void InitWorkflow(string name, string xaml)
        {
            //AddInCaching.ImportAssemblies(references);
            Caching.LoadFromLocal();

            workflowView = new WorkflowEditorView();
            workflowView.DataContext = WorkflowEditorVM;

            WorkflowEditorVM.Init(name, xaml);

            toolbox = new ToolboxView();
            toolbox.DataContext = new ToolboxViewModel();

        }

        public INativeHandleContract WorkflowEditorView
        {
            get
            {
                return FrameworkElementAdapters.ViewToContractAdapter(this.workflowView);
            }
        }

        public INativeHandleContract ToolboxView
        {
            get
            {
                return FrameworkElementAdapters.ViewToContractAdapter(this.toolbox);
            }
        }

        private Assembly ResolveAssembly(object sender, ResolveEventArgs args)
        {
            return Resolve(args.Name);
        }

        public Assembly Resolve(string requiredAssemblyFullName)
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
                //ActivityAssemblyItem aai;
                //result = LoadCachedAssembly(cache ?? AddInCaching.ActivityAssemblyItems, reqAssemblyName, out aai) ? Assembly.LoadFrom(aai.Location) : null;
                string location = Utility.GetAssembliesDirectoryPath() + "\\"+ requiredAssemblyFullName + ".dll";
                if (File.Exists(location))
                    result = Assembly.LoadFrom(location);
            }

            // TODO: Conditionally load the assembly from the Repository

            return result;
        }

        public void Close()
        {
            Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called. 
            if (disposing)
            {
                // Dispose managed resources.
                Dispatcher.CurrentDispatcher.InvokeShutdown();
            }
        }
    }
}
