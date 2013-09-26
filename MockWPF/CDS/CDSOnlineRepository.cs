using NuGet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MockWPF.CDS
{
    public class CDSOnlineRepository : CDSRepositoryBase
    {
        public bool NoCache { get; set; }

        /// <remarks>
        /// Meant for unit testing.
        /// </remarks>
        protected IPackageRepository CacheRepository { get; private set; }

        public CDSOnlineRepository()
            : this(MachineCache.Default)
        {
        }

        protected internal CDSOnlineRepository(IPackageRepository cacheRepository)
        {
            CacheRepository = cacheRepository;
        }

        protected IPackageRepository CreateRepository(string source)
        {
            IPackageRepository repository = base.GetRepository(source);

            if (NoCache)
            {
                return repository;
            }
            else
            {
                return new PriorityPackageRepository(CacheRepository, repository);
            }
        }

        public bool InstallPackage(string source, string packageId, SemanticVersion version)
        {
            var packageManager = CreatePackageManager(source);

            using (packageManager.SourceRepository.StartOperation(RepositoryOperationNames.Install, packageId))
            {
                packageManager.InstallPackage(packageId, version, ignoreDependencies: false, allowPrereleaseVersions: true);
                return true;
            }
        }

        protected IPackageManager CreatePackageManager(string source)
        {
            var cwfFilwSystem = CreateCWFFileSystem();
            var repository = CreateRepository(source);
            LocalPackageRepository localRepository = GetLocalRepository() as LocalPackageRepository;
            return new PackageManager(repository, localRepository.PathResolver, cwfFilwSystem, localRepository);
        }

        protected CWFFileSystem CreateCWFFileSystem()
        {
            var path = Path.GetFullPath(CDSConstants.AssemblyPath);
            return new CWFFileSystem(path);
        }
    }
}
