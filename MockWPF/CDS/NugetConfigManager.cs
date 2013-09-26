using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MockWPF.CDS
{
    public static class NugetConfigManager
    {
        private const string configName = "nuget.config";
        private static string configPath
        {
            get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configName); }
        }

        public static void Save(List<CDSRepository> repos)
        {
            XElement xConfig = new XElement("configuration",
                new XElement("packageRestore",
                    new XElement("add",
                        new XAttribute("key", "enabled"),
                        new XAttribute("value", "True")),
                    new XElement("add",
                        new XAttribute("key", "automatic"),
                        new XAttribute("value", "True"))),
                new XElement("packageSources",
                    from repo in repos
                    select new XElement("add",
                        new XAttribute("key", repo.Name),
                        new XAttribute("value", repo.Source))),
                new XElement("disabledPackageSources",
                    from repo in repos
                    where !repo.IsEnabled
                    select new XElement("add",
                        new XAttribute("key", repo.Name),
                        new XAttribute("value", "true"))),
                new XElement("activePackageSource",
                    new XElement("add",
                        new XAttribute("key", "All"),
                        new XAttribute("value", "(Aggregate source)"))));
            xConfig.Save(configPath);
        }

        public static List<CDSRepository> Load()
        {
            if (!File.Exists(configPath))
                return new List<CDSRepository>();

            XElement xConfig = XElement.Load(configPath);
            List<string> disabled = (from xDisabled in xConfig.Element("disabledPackageSources").Elements()
                                     select xDisabled.Attribute("key").Value).ToList();
            return (from xSource in xConfig.Element("packageSources").Elements()
                    select new CDSRepository()
                    {
                        IsEnabled = !disabled.Contains(xSource.Attribute("key").Value),
                        Name = xSource.Attribute("key").Value,
                        Source = xSource.Attribute("value").Value
                    }).ToList();
        }
    }
}
