using LoginService;
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
            AuthenticationService service = new AuthenticationService();
            LoginViewModel loginViewModel = new LoginViewModel(service);
            LoginScreen login = new LoginScreen();
            service.RetrieveUsers();
            bool? res = login.ShowDialog();
            Application.Current.MainWindow = null;
            Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;

            if (res.HasValue && res.Value)//&& Authenticate(logon.UserName, logon.Password))
            {
                if (loginViewModel.IsAuthenticated)
                {
                    MainBootstrapper bootStrapper = new MainBootstrapper();
                    bootStrapper.Run();
                }
            }
            //else if (!Authenticate(logon.UserName, logon.Password))
            //{
            //MessageBox.Show("Wrong login password combination",
            //    "Authentication Unsuccessful",
            //    MessageBoxButton.OK,
            //    MessageBoxImage.Error););
            //}
            else
            {
                MessageBox.Show(
                    "Aplikacja zostanie zamknięta",
                    "Wyjście z aplikacji",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Application.Current.Shutdown(1);
            }
        }
    }
}
