using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AddIn.Common.Message
{
    /// <summary>
    /// Messages for the import functionality.
    /// </summary>
    public static class ImportMessages
    {
        /// <summary>
        /// The assembly parameter is null
        /// </summary>
        public readonly static string AssemblyNull = "Assembly parameter cannot be null.";
        /// <summary>
        /// The name of the assembly is null.
        /// </summary>
        public readonly static string AssemblyNameNull = "Assembly Name cannot be null.";
        /// <summary>
        /// The location (file path) of the parameter is null.
        /// </summary>
        public readonly static string AssemblyLocationNull = "Assembly Location cannot be null.";
        /// <summary>
        /// The location (file path) of the parameter is null.
        /// </summary>
        public readonly static string FileNotFound = "The file specified does not exist.";
        /// <summary>
        /// The assembly is not a compatible .NET assembly
        /// </summary>
        public readonly static string NotADotNetAssembly = "The file '{0}' is not a .NET assembly and cannot be imported.";
        /// <summary>
        /// The assembly could not be analyzed
        /// </summary>
        public readonly static string AssemblyNotAnalyzed = "The assembly could not be analyzed and cannot not be imported.";
        /// <summary>
        /// The asembly is not signed, and for security purposes is not supported.
        /// </summary>
        public static readonly string AssemblyUnsigned = "Unsigned assemblies are not supported.";
        /// <summary>
        /// No assembly was specified in the list of assemblies to import.
        /// </summary>
        public static readonly string AssembliesToImportNull = "The list of assemblies to import is null.";

        /// <summary>
        /// The assembly is already in the local cache.
        /// </summary>
        public static readonly string AssemblyAlreadyImported = "The assembly already exists in the local cache.";

        /// <summary>
        /// The assembly has already been inspected and exists in the list of assemblies to import
        /// </summary>
        public static readonly string AssemblyAlreadyInListToImport =
            "The selected assembly is already in the list of assemblies to import";

        /// <summary>
        /// The category is null
        /// </summary>
        public static readonly string CategoryNameNull =
           "The name of the category cannot be null";
        /// <summary>
        /// The category name is too long 
        /// </summary>
        public static readonly string CategoryNameOutOfRange =
            "The name of the category exceeds the maximum length for the value";

        /// <summary>
        /// The category name is too long 
        /// </summary>
        public static readonly string CategoryWithInvalidName =
            "The name of the category contains invalid characters";



    }
}
