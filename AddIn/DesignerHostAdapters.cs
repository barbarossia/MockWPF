using System;
using System.AddIn.Pipeline;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace AddIn
{
    [Serializable]
    public class DesignerHostAdapters : MarshalByRefObject
    {
        private IDesignerContract proxy;
        public FrameworkElement View
        {
            get
            {
                return FrameworkElementAdapters.ContractToViewAdapter(this.proxy.WorkflowEditorView);
            }
        }

        public FrameworkElement ToolboxView
        {
            get { return FrameworkElementAdapters.ContractToViewAdapter(this.proxy.ToolboxView); }
        }

        public DesignerHostAdapters() { }
        public DesignerHostAdapters(string name, string projectXamlCode)
            : base()
        {
            this.InitAddIn(name, projectXamlCode);
        }

        public void SetupCompositeActivityDesinger()
        {
            proxy.SetupCompositeActivityDesinger();
        }
        private void InitAddIn(string name, string projectXamlCode)
        {
            this.proxy = Create();
            this.proxy.InitWorkflow(name, projectXamlCode);
        }

        private IDesignerContract Create()
        {
            Type addinType = typeof(DesignerAddIn);
            AppDomain domain = AppDomain.CreateDomain(Guid.NewGuid().ToString(), null, new AppDomainSetup
            {
                LoaderOptimization = LoaderOptimization.MultiDomainHost,
                ApplicationBase = AppDomain.CurrentDomain.BaseDirectory,
            });
            domain.InitializeLifetimeService();
            return (IDesignerContract)domain.CreateInstanceAndUnwrap(addinType.Assembly.FullName, addinType.FullName);
        }

    }
}
