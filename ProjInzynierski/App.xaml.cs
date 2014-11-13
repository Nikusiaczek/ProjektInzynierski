using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ProjInzynierski
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            //base.OnStartup(e);
            //MainBootstrapper bootstrapper = new MainBootstrapper();
            //bootstrapper.Run();

            Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            LoginScreen logon = new LoginScreen();
            bool? res = logon.ShowDialog();
            Application.Current.MainWindow = null;
            Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;

            if (res.HasValue && res.Value )//&& Authenticate(logon.UserName, logon.Password))
            {
                MainBootstrapper bootStrapper = new MainBootstrapper();
                bootStrapper.Run();
            }
            else
            {
                MessageBox.Show(
                    "Application is exiting due to invalid credentials",
                    "Application Exit",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Application.Current.Shutdown(1);
            }
        }
    }
}
