using NuGet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MockWPF.CDS
{
    public class CDSInstalledRepository : CDSRepositoryBase
    {
        protected override IPackageRepository GetRepository(string source)
        {
            return GetLocalRepository();
        }
    }
}
