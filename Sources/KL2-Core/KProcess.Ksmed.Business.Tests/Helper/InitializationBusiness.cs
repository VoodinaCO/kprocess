using KProcess.Globalization;
using KProcess.Ksmed.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KProcess.Ksmed.Tests.Helper
{

    partial class Initialization
    {
        /// <summary>
        /// Initialise la localisation.
        /// </summary>
        static partial void InitLocalizationImpl()
        {
            var resources = new KProcess.Ksmed.Business.Impl.AppResourceService().GetResources("fr-FR");

            LocalizationManager.ResourceProvider = new DatabaseResourceProvider("Main", resources);
        }
    }
}
