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
            MyPrincipal customPrincipal = new MyPrincipal();
            AppDomain.CurrentDomain.SetThreadPrincipal(customPrincipal);

            Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            LoginViewModel logViewModel = new LoginViewModel();
            LoginScreen login = new LoginScreen(logViewModel);
            bool? res = login.ShowDialog();
            Application.Current.MainWindow = null;
            Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;

            if (res.HasValue && res.Value && logViewModel.IsAuthenticated)
            {
                MainBootstrapper bootStrapper = new MainBootstrapper();
                bootStrapper.Run();
            }
            else if (!logViewModel.IsAuthenticated)
            {
                MessageBox.Show("Niepoprawne dane logowania.\n Spróbuj ponownie.",
                "Logowanie nie powiodło się",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
                Application.Current.Shutdown(1);
            }
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
