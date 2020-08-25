using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KProcess.Ksmed
{
    public static class AssertExt
    {
        public static void IsExceptionNull(Exception e)
        {
            if (e != null)
                throw e;
        }

        public static void Throws<TException>(Action action)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                if (e is TException)
                    return;
                throw new AssertFailedException(string.Format("Expected Exception {0}, caught Exception {1}", typeof(TException).FullName, e.GetType().FullName));
            }
            finally
            {
                throw new AssertFailedException(string.Format("Expected Exception {0}, caught nothing", typeof(TException).FullName));
            }

        }

    }
}
