using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace AddIn
{
    [Serializable]
    public class ActivityFocuceEventArgs : EventArgs
    {
        public WorkflowOutlineNode Node { get; private set; }
        public ActivityFocuceEventArgs(WorkflowOutlineNode node)
        {
            this.Node = node;
        }
    }

    [Serializable]
    [ComVisible(true)]
    public delegate void ActivityFocuceEventHandler(object sender, ActivityFocuceEventArgs e);
}
