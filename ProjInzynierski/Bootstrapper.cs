using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.UnityExtensions;
using System.Windows;
using Microsoft.Practices.Prism.Modularity;

namespace ProjInzynierski
{
    class MainBootstrapper: UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return this.Container.Resolve<Shell>();
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();

            App.Current.MainWindow = (Window)this.Shell;
            App.Current.MainWindow.Show();
        }

    }
}
