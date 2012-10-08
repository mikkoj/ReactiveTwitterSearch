using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

using GalaSoft.MvvmLight.Threading;

namespace TwitterSearch
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += ErrorHandler;
            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
            DispatcherUnhandledException += ApplicationOnDispatcherUnhandledException;
            DispatcherHelper.Initialize();
        }

        private static void ApplicationOnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs args)
        {
            var exception = args.Exception;
        }

        private static void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs args)
        {
            var exception = args.Exception;
        }

        private static void ErrorHandler(object sender, UnhandledExceptionEventArgs args)
        {
            var exception = args.ExceptionObject as Exception;
        }
    }
}
