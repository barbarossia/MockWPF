using AddIn;
using NuGet;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;

namespace MockWPF.CDS
{
    public class CDSPackage : ViewModelBase, IPackage
    {
        private string id;
        private string tags;
        private string description;
        private DateTime lastPublished;
        private string allAuthors;
        private string allDependencySets;

        public SemanticVersion Version { get; set; }
        public string Title { get; set; }
        public IEnumerable<string> Authors { get; set; }
        public IEnumerable<string> Owners { get; set; }
        public Uri IconUrl { get; set; }
        public Uri LicenseUrl { get; set; }
        public Uri ProjectUrl { get; set; }
        public bool RequireLicenseAcceptance { get; set; }
        public string Summary { get; set; }
        public string ReleaseNotes { get; set; }
        public string Language { get; set; }
        public string Copyright { get; set; }
        public bool IsAbsoluteLatestVersion { get; set; }
        public bool IsLatestVersion { get; set; }
        public bool Listed { get; set; }
        public Uri ReportAbuseUrl { get; set; }
        public int DownloadCount { get; set; }
        public DateTimeOffset? Published { get; set; }
        public IEnumerable<IPackageAssemblyReference> AssemblyReferences { get; set; }
        public IEnumerable<IPackageFile> GetFiles()
        {
            return null;
        }
        public IEnumerable<FrameworkName> GetSupportedFrameworks()
        {
            return null;
        }
        public Stream GetStream()
        {
            return null;
        }
        /// <summary>
        /// Specifies assemblies from GAC that the package depends on.
        /// </summary>
        public IEnumerable<FrameworkAssemblyReference> FrameworkAssemblies { get; set; }

        /// <summary>
        /// Returns sets of References specified in the manifest.
        /// </summary>
        public ICollection<PackageReferenceSet> PackageAssemblyReferences { get; set; }

        /// <summary>
        /// Specifies sets other packages that the package depends on.
        /// </summary>
        public IEnumerable<PackageDependencySet> DependencySets { get; set; }

        public Version MinClientVersion { get; set; }


        public string Id
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
                RaisePropertyChanged(() => Id);
            }
        }

        public string Tags
        {
            get
            {
                return tags;
            }
            set
            {
                tags = value;
                RaisePropertyChanged(() => Tags);
            }
        }

        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                description = value;
                RaisePropertyChanged(() => Description);
            }
        }

        public DateTime LastPublished
        {
            get
            {
                return lastPublished;
            }
            set
            {
                lastPublished = value;
                RaisePropertyChanged(() => LastPublished);
            }
        }

        public string AllAuthors
        {
            get
            {
                return allAuthors;
            }
            set
            {
                allAuthors = value;
                RaisePropertyChanged(() => AllAuthors);
            }
        }

        public string AllDependencySets
        {
            get
            {
                return allDependencySets;
            }
            set
            {
                allDependencySets = value;
                RaisePropertyChanged(() => AllDependencySets);
            }
        }


        public CDSPackage(IPackage package)
        {
            Id = package.Id;
            Tags = package.Tags;
            Description = package.Description;
            DownloadCount = package.DownloadCount;
            Published = package.Published;
            if (Published.HasValue)
                LastPublished = Published.Value.DateTime;
            Version = package.Version;
            Authors = package.Authors;
            AllAuthors = String.Join(",", Authors);
            List<string> dep = new List<string>();
            foreach (var d in package.DependencySets)
            {
                foreach (var ds in d.Dependencies)
                {
                    dep.Add(ds.ToString());
                }
            }
            AllDependencySets = String.Join(Environment.NewLine, dep);
        }
    }
}
