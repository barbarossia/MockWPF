using AddIn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace MockWPF
{
    public class Startup
    {
        private static App app = new App();
        public static bool IsInitialized { get; set; }

        [STAThread]
        //[System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [LoaderOptimization(LoaderOptimization.MultiDomainHost)]
        public static void Main()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            app.DispatcherUnhandledException += UnhandledException;
            ApplicationStartup(null, null);
        }

        private static void InitializationFinished()
        {
            var window = new MainWindow();
            var viewModel = new MainWindowViewModel();
            window.DataContext = viewModel;
            app.MainWindow = window;
            app.MainWindow.Show();
            // From this point on, if there is an unhandled exception it won't leave a phantom app running
            // because MainWindow has been shown, so we can change our error handling to not shut down the app.
            // Work around WPF limitation: there is no event to tell you when frame at StartUri has loaded successfully
            // (Application.LoadCompleted does not do what you think it does) by Manually creating the MainWindow during
            // startup.
            IsInitialized = true;
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            return Utility.Resolve(args.Name);
        }

        private static void UnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs unhandledExceptionEvent)
        {
            // In the long-run, only UserFacingExceptions should be surfaced here.
            var e = unhandledExceptionEvent.Exception;
            unhandledExceptionEvent.Handled = true;
        }

        private static void ApplicationStartup(object sender, StartupEventArgs e)
        {
            //Check User Level to grant/deny access to app
            AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);
           
            app.Resources.Source = new Uri("pack://application:,,,/Resources/MergedDictionary.Xaml");
            //InitializationFinished();
            Task.Factory.StartNew(() =>
            {
                //Caching.LoadFromLocal();
                app.Dispatcher.BeginInvoke(new Action(InitializationFinished), DispatcherPriority.Background);
            });
            app.Run();
        }
    }
}
