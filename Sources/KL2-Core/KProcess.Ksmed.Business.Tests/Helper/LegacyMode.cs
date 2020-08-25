using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace KProcess.Ksmed.Business.Tests
{
    /// <summary>
    /// Extrait de code => http://reedcopsey.com/2011/09/15/setting-uselegacyv2runtimeactivationpolicy-at-runtime/
    /// Permet de définir au runtime l'attribut useLegacyV2RuntimeActivationPolicy de l'élément Startup du fichier de configuration
    /// useLegacyV2RuntimeActivationPolicy => http://web.archive.org/web/20130128072944/http://www.marklio.com/marklio/PermaLink,guid,ecc34c3c-be44-4422-86b7-900900e451f9.aspx
    /// 
    /// En cas d'erreur "A runtime has already been bound for legacy activation policy use.", tuer le processus "vstest.executionengine.exe".
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class LegacyMode : Attribute
    {
        public static void UseLegacy()
        {
            ICLRRuntimeInfo clrRuntimeInfo =
                (ICLRRuntimeInfo)RuntimeEnvironment.GetRuntimeInterfaceAsObject(
                    Guid.Empty,
                    typeof(ICLRRuntimeInfo).GUID);
            clrRuntimeInfo.BindAsLegacyV2Runtime();
        }
    }
}
