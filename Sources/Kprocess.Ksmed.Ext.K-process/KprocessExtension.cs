using KProcess.Globalization;
using KProcess.Ksmed.Ext.Kprocess.ViewModels.Interfaces;
using KProcess.Ksmed.Presentation;
using KProcess.Ksmed.Presentation.Core;
using KProcess.Presentation.Windows;
using System;
using System.ComponentModel.Composition;
using System.IO;

namespace KProcess.Ksmed.Ext.Kprocess
{
    public class KprocessExtension : IExtension
    {
        public static readonly Guid Id = new Guid("{90CB437A-B742-4F44-8B4F-A13A1FCBDEB6}");

        /// <summary>
        /// Obtient le dossier où se trouve l'assembly Core. 
        /// </summary>
        public static readonly string AssemblyDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

        /// <summary>
        /// Obtient le Guid unique de l'extension.
        /// </summary>
        public Guid ExtensionId => Id;

        /// <summary>
        /// Obtient le libellé de l'extension.
        /// </summary>
        public string Label =>
            LocalizationManager.GetString("ExtKp_Label", ResourceExport.Id);

        /// <summary>
        /// Obtient une valeur indiquant si l'extension est configurable.
        /// </summary>
        public bool HasConfiguration => true;

        /// <summary>
        /// Obtient la version minimale de l'application requise au chargement.
        /// </summary>
        public Version MinimumApplicationVersion => new Version("3.0.0");

        /// <summary>
        /// Obtient ou définit la factory des views et viewModels
        /// </summary>
        [Import]
        protected IUXFactory UXFactory { get; set; }

        /// <summary>
        /// Obtient le couple VM/View de configuration.
        /// </summary>
        /// <param name="extensionConfigurationView">La vue.</param>
        /// <returns>
        /// Le ViewModel.
        /// </returns>
        public IExtensionConfigurationViewModel GetExtensionConfiguration(out IView extensionConfigurationView)
        {
            IConfigurationViewModel vm;
            extensionConfigurationView = UXFactory.GetView(out vm);

            vm.Load();

            //var vm = new ConfigurationViewModel();
            //var view = new ConfigurationView { DataContext = vm };

            //extensionConfigurationView = view;
            //vm.Load();

            return vm;
        }
    }
}
