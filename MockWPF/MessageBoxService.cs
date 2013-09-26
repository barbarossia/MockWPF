using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;

namespace MockWPF
{
    public class MessageBoxService
    {
        public static void CannotUncheckAssemblyForReferenced(AssemblyName name, AssemblyName[] references)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("You cannot uncheck \"{0}\" because following assemblies referenced it:", name.Name));
            foreach (AssemblyName parent in references)
            {
                sb.AppendLine(string.Format("\"{0}\" version {1}", parent.Name, parent.Version));
            }
            Show(sb.ToString(), "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        /// <summary>
        /// Cannot check assembly because multiple versions used
        /// </summary>
        public static void CannotCheckAssemblyItself()
        {
            Show("You cannot import an assembly which has same name of current workflow.",
                "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        /// <summary>
        /// Cannot check assembly because another version of its dependencies is checked
        /// </summary>
        public static void CannotCheckAssemblyForAnotherVersionSelected(string assemblyName, string versionToCheck, string checkedVersion)
        {
            Show(string.Format("The assembly you checked needs \"{0}\" version {1}. You cannot import it unless \"{0}\" version {2} is unchecked.",
                assemblyName, versionToCheck, checkedVersion), "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }


        public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon)
        {
            return ShowFunc(messageBoxText, caption, button, icon, MessageBoxResult.OK);
        }

        public static Func<string, string, MessageBoxButton, MessageBoxImage, MessageBoxResult, MessageBoxResult> ShowFunc = ((msg, caption, button, icon, defaultResult) => MessageBox.Show(msg, caption, button, icon, defaultResult));
    }
}
