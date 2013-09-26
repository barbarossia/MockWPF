using MockWPF.Common;
using NuGet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MockWPF.CDS
{
    public class CDSUpdateRepository : CDSOnlineRepository
    {
        public override PagedList<IPackage> Search(
            int startIndex,
            int pageSize,
            string source,
            CDSSortByType orderBy,
            string searchTerm,
            bool isLatestVersion = true)
        {
            IPackageRepository packageRepository = GetRepository(source);
            var localPackages = GetLocalPackages(searchTerm);
            var packages = packageRepository.GetUpdates(
                localPackages,
                includePrerelease: true,
                includeAllVersions: false).AsQueryable();

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

        public bool UpdatePackage(string source, string packageId, SemanticVersion version)
        {
            var packageManager = CreatePackageManager(source);

            using (packageManager.SourceRepository.StartOperation(RepositoryOperationNames.Update, packageId))
            {
                packageManager.UpdatePackage(packageId, version, updateDependencies: true, allowPrereleaseVersions: true);
                return true;
            }
        }

        private IQueryable<IPackage> GetLocalPackages(string searchTerm)
        {
            IPackageRepository lcoalRepository = GetLocalRepository();

            return lcoalRepository.Search(searchTerm, true);

        }
    }
}
