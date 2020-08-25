using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KProcess.Ksmed.Presentation.Tests
{
    [TestClass] // Requis pour [AssemblyInitialize]
    public class Initialization
    {

        /// <summary>
        /// Initialise les contexte de tests.
        /// </summary>
        /// <param name="tc">Le contexte de tests.</param>
        [AssemblyInitialize]
        public static void Initialize(TestContext tc)
        {
            global::KProcess.Ksmed.Tests.Helper.Initialization.SetTaskScheduler();
            global::KProcess.Ksmed.Tests.Helper.Initialization.InitLocalization();
            global::KProcess.Ksmed.Tests.Helper.Initialization.SetCurrentUser();
            global::KProcess.Ksmed.Tests.Helper.Initialization.InitDiagnostics(tc);
        }
    }
}
