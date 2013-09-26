using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvertAssemblyFolder
{
    class Program
    {
        static void Main(string[] args)
        {
            Convert(@"D:\Assemblies", @"D:\MockAssemblies");
            Console.ReadLine();
        }

        static void Convert(string source, string target)
        {
            var files = Directory.GetDirectories(source)
                .SelectMany(path => Directory.GetDirectories(path))
                .SelectMany(dir => Directory.GetFiles(dir));
            foreach (var file in files)
            {
                var targetFileName = Path.GetFileName(file);
                var targetPath = Path.Combine(target, targetFileName);
                Console.WriteLine("Copy {0} To {1}", targetFileName, targetPath);
                File.Copy(file, targetPath);
            }
        }
    }
}
