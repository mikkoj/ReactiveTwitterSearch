using System;
using System.Windows;
using GalaSoft.MvvmLight.Threading;

namespace TwitterSearch
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static App()
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += MyHandler;
            DispatcherHelper.Initialize();
        }

        private static void MyHandler(object sender, UnhandledExceptionEventArgs e)
        {

        }
    }
}
