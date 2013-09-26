using MockWPF.Common;
using NuGet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MockWPF.CDS
{
    public abstract class CDSRepositoryBase
    {
        private List<string> sources;
        public IFileSystem FileSystem { get; set; }
        public IList<string> Arguments { get; private set; }
        public IMachineWideSettings MachineWideSettings { get; set; }
        public List<string> Source
        {
            get
            {
                if (sources == null)
                {
                    sources = NugetConfigManager.Load().Where(p => p.IsEnabled).Select(p => p.Name).ToList();
                }
                return sources;
            }
        }

        protected internal ISettings Settings { get; set; }
        protected internal IPackageSourceProvider SourceProvider { get; set; }
        protected internal IPackageRepositoryFactory RepositoryFactory { get; set; }

        protected CDSRepositoryBase()
        {
            InitNuGetRepository();
        }

        public virtual PagedList<IPackage> Search(
            int startIndex,
            int pageSize,
            string source,
            CDSSortByType orderBy,
            string searchTerm,
            bool isLatestVersion = true)
        {
            IPackageRepository packageRepository = GetRepository(source);

            IQueryable<IPackage> packages = packageRepository.Search(searchTerm, false);

            if (isLatestVersion)
            {
                packages = packages.Where(p => p.IsLatestVersion);
            }

            switch (orderBy)
            {
                case CDSSortByType.MostDownloads:
                    packages = packages.OrderByDescending(p => p.DownloadCount);
                    break;
                case CDSSortByType.PublishedDate:
                    packages = packages.OrderByDescending(p => p.Published);
                    break;
                case CDSSortByType.NameAscending:
                    packages = packages.OrderBy(p => p.Id);
                    break;
                case CDSSortByType.NameDescending:
                    packages = packages.OrderByDescending(p => p.Id);
                    break;
                default:
                    break;
            }

            return new PagedList<IPackage>(packages, startIndex, pageSize);
        }

        protected virtual IPackageRepository GetRepository(string source)
        {
            List<string> providers = Source;

            if (!string.IsNullOrEmpty(source))
                providers = Source.Where(s => s == source).ToList();

            if (providers == null || !providers.Any())
                throw new ArgumentException("Package Repository Source Cannot be empty.");

            var repository = AggregateRepositoryHelper.CreateAggregateRepositoryFromSources(RepositoryFactory, SourceProvider, providers);
            return repository;
        }

        protected IPackageRepository GetLocalRepository()
        {
            if (!Directory.Exists(CDSConstants.CDSLocalPath))
            {
                Directory.CreateDirectory(CDSConstants.CDSLocalPath);
            }

            var nugetFileSystem = CreateFileSystem(CDSConstants.CDSLocalPath);
            var pathResolver = new DefaultPackagePathResolver(nugetFileSystem, useSideBySidePaths: true);
            IPackageRepository localRepository = new LocalPackageRepository(pathResolver, nugetFileSystem);
            return localRepository;
        }

        protected IFileSystem CreateFileSystem(string path)
        {
            path = Path.GetFullPath(path);
            return new PhysicalFileSystem(path);
        }

        private void InitNuGetRepository()
        {
            RepositoryFactory = new PackageRepositoryFactory();

            var directory = Path.GetDirectoryName(Path.GetFullPath(CDSConstants.ConfigFile));
            var configFileName = Path.GetFileName(CDSConstants.ConfigFile);
            var configFileSystem = new PhysicalFileSystem(directory);
            Settings = NuGet.Settings.LoadDefaultSettings(
                configFileSystem,
                configFileName,
                MachineWideSettings);

            SourceProvider = PackageSourceBuilder.CreateSourceProvider(Settings);

            // Register an additional provider for the console specific application so that the user
            // will be prompted if a proxy is set and credentials are required
            var credentialProvider = new SettingsCredentialProvider(
                new ClientCredentialProvider(),
                SourceProvider);
            HttpClient.DefaultCredentialProvider = credentialProvider;
        }
    }
}
