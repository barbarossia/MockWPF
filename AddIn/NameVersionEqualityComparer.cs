using System;
using System.Activities.Presentation.Model;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AddIn
{
    [Serializable]
    public class NameVersionEqualityComparer : IEqualityComparer<AssemblyName>
    {
        public bool Equals(AssemblyName x, AssemblyName y)
        {
            var xVersion = ActivityAssemblyItem.GetVersionIfSigned(x);
            var yVersion = ActivityAssemblyItem.GetVersionIfSigned(y);
            return (x.Name == y.Name && xVersion == yVersion);
        }

        public int GetHashCode(AssemblyName assemblyName)
        {
            return assemblyName.Name.GetHashCode();
        }


    }

    [Serializable]
    public class ModelNameVersionEqualityComparer : NameVersionEqualityComparer, IEqualityComparer<ModelItem>
    {
        public bool Equals(ModelItem x, ModelItem y)
        {
            var xAssemblyName = x.ItemType.Assembly.GetName();
            var yAssemblyName = y.ItemType.Assembly.GetName();

            return base.Equals(xAssemblyName, yAssemblyName);
        }

        public int GetHashCode(ModelItem modelItem)
        {
            return base.GetHashCode(modelItem.ItemType.Assembly.GetName());
        }
    }


    [Serializable]
    public class ActivityAssemblyItemNameVersionEqualityComparer : IEqualityComparer<ActivityAssemblyItem>
    {
        public bool Equals(ActivityAssemblyItem x, ActivityAssemblyItem y)
        {
            return (x.Name == y.Name && x.Version == y.Version);
        }

        public int GetHashCode(ActivityAssemblyItem assemblyName)
        {
            return assemblyName.Name.GetHashCode();
        }


    }
}
