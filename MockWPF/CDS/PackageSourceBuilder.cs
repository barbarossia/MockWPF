using NuGet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MockWPF.CDS
{
    internal static class PackageSourceBuilder
    {
        internal static PackageSourceProvider CreateSourceProvider(ISettings settings)
        {
            var packageSourceProvider = new PackageSourceProvider(settings);
            return packageSourceProvider;
        }
    }
}
