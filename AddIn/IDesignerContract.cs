using System;
using System.AddIn.Contract;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AddIn
{
    public interface IDesignerContract
    {
        INativeHandleContract WorkflowEditorView { get; }
        INativeHandleContract ToolboxView { get; }
        void InitWorkflow(string name, string xaml);
    }
}
