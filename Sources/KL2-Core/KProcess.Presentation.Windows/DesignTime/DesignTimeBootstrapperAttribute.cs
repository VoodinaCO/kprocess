using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KProcess.Presentation.Windows;
using KProcess.Globalization;

namespace KProcess.Presentation.Windows.DesignTime
{

    /// <summary>
    /// Un attribut qui initialize les ressoruces en mode design.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class DesignTimeBootstrapperAttribute : Attribute
    {
        static DesignTimeBootstrapperAttribute()
        {
            if (DesignMode.IsInDesignMode)
            {
                // Initialize the trace wrapper
                IoC.RegisterType<ITraceWrapper, DesignTraceWrapper>(false);

                LocalizationManager.ResourceProvider = new DesignTimeResourceProvider();
            }
        }


    }


}
