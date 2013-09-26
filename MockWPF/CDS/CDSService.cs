using MockWPF.Common;
using NuGet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MockWPF.CDS
{
    public static class CDSService
    {
        public static PagedList<IPackage> SearchOnline(int startIndex,
            int pageSize,
            string source,
            CDSSortByType orderBy,
            string searchTerm,
            bool isLatestVersion = true)
        {
            CDSOnlineRepository repository = new CDSOnlineRepository();
            return repository.Search(startIndex, pageSize, source, orderBy, searchTerm, isLatestVersion);
        }

        public static PagedList<IPackage> SearchLocal(int startIndex,
            int pageSize,
            CDSSortByType orderBy,
            string searchTerm,
            bool isLatestVersion = true)
        {
            CDSInstalledRepository repository = new CDSInstalledRepository();
            return repository.Search(startIndex, pageSize, null, orderBy, searchTerm, isLatestVersion);
        }

        public static PagedList<IPackage> SearchUpdate(int startIndex,
            int pageSize,
            string source,
            CDSSortByType orderBy,
            string searchTerm,
            bool isLatestVersion = true)
        {
            CDSUpdateRepository repository = new CDSUpdateRepository();
            return repository.Search(startIndex, pageSize, source, orderBy, searchTerm, isLatestVersion);
        }

        public static bool Install(string source, string packageId, SemanticVersion version)
        {
            CDSOnlineRepository repository = new CDSOnlineRepository();
            return repository.InstallPackage(source, packageId, version);
        }

        public static bool Update(string source, string packageId, SemanticVersion version)
        {
            CDSUpdateRepository repository = new CDSUpdateRepository();
            return repository.UpdatePackage(source, packageId, version);
        }
    }
}
