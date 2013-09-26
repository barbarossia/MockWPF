using AddIn;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            //TestSilbing();
            TestPerform();
            Console.ReadLine();
        }

        static void TestSilbing()
        {
            string xml = @"<1><2/><3><6/></3></1><4><5></5></4>";
            XamlIndexTreeHelper.CreateIndexTree(xml);
        }

        static void TestPerform()
        {
            string xml = System.IO.File.ReadAllText(@"D:\CommonWorkflowSystem\Azure_victor.xaml");
            Stopwatch watch = new Stopwatch();
            watch.Start();
            XamlIndexTreeHelper.CreateIndexTree(xml);
            //XamlTreeHelper.Create(xml);
            watch.Stop();
            Console.WriteLine("\tTime Elapsed:\t" + watch.ElapsedMilliseconds.ToString("N0") + "ms");
        }
    }
}
