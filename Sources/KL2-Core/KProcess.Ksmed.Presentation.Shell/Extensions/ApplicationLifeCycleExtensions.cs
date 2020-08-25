using KProcess.Presentation.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows;

namespace KProcess.Ksmed.Presentation.Shell
{
    /// <summary>
    /// Etend le comportement du cycle de vie de l'application
    /// </summary>
    public static class ApplicationLifeCycleExtensions
    {
        /// <summary>
        /// Redemarre l'application courante
        /// </summary>
        /// <param name="application">L'application a redemarer</param>
        /// <param name="notify">Determine si une notification doit appparaitre</param>
        public static void Restart(this Application application)
        {
            var assembly = Application.ResourceAssembly;
            var reviver = new Thread(new ThreadStart(() =>
            {
                Thread.Sleep(TimeSpan.FromSeconds(2));
                LaunchApplication(assembly);
            }));

            reviver.Name = "KL² reviver thread";
            reviver.TrySetApartmentState(ApartmentState.STA);
            App.Current.Exit += (sender, e) => reviver.Start();
            App.Current.Shutdown();
        }

        private static void LaunchApplication(Assembly assembly)
        {
            Process.Start(assembly.Location);
            Thread.CurrentThread.Abort();
        }
    }
}
