using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace IPManagementInterface
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            if (MainWindow != null)
            {
                MainWindow.Show();
                MainWindow.Activate();
            }
        }
    }
}
