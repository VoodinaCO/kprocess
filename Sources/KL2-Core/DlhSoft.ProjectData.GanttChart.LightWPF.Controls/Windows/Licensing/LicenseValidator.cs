namespace DlhSoft.Windows.Licensing
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Windows;
    using System.Windows.Threading;

    internal static class LicenseValidator
    {
        private static string GetAssemblyName(Assembly assembly)
        {
            string str = assembly.ToString();
            int startIndex = 0;
            int index = str.IndexOf(',');
            str = (startIndex >= 0) ? ((index >= 0) ? str.Substring(startIndex, index - startIndex) : str.Substring(startIndex)) : string.Empty;
            return str.Trim();
        }

        private static string GetAssemblyPublicKeyToken(Assembly assembly)
        {
            string str = assembly.ToString();
            string str2 = "PublicKeyToken=";
            int index = str.IndexOf(str2);
            if (index >= 0)
            {
                index += str2.Length;
            }
            int num2 = (index >= 0) ? str.IndexOf(',', index) : -1;
            str = (index >= 0) ? ((num2 >= 0) ? str.Substring(index, num2 - index) : str.Substring(index)) : string.Empty;
            return str.Trim();
        }

        private static bool IsCallingAssemblyFriend(Assembly callingAssembly)
        {
            if (callingAssembly == null)
            {
                return false;
            }
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            if (callingAssembly == executingAssembly)
            {
                return true;
            }
            string assemblyPublicKeyToken = GetAssemblyPublicKeyToken(callingAssembly);
            string str2 = GetAssemblyPublicKeyToken(executingAssembly);
            return (assemblyPublicKeyToken == str2);
        }

        internal static bool IsEntryAssemblyLicenseCompiler()
        {
            Assembly entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly == null)
            {
                return false;
            }
            return ((GetAssemblyName(entryAssembly) == "lc") && (GetAssemblyPublicKeyToken(entryAssembly) == "b03f5f7f11d50a3a"));
        }

        private static bool IsInDesignMode(object instance)
        {
            DependencyObject element = instance as DependencyObject;
            return ((element != null) && DesignerProperties.GetIsInDesignMode(element));
        }

        private static bool IsLicenseValid(object instance, Type type)
        {
            License license = null;
            bool flag;
            try
            {
                flag = LicenseManager.IsValid(type, instance, out license);
            }
            finally
            {
                if (license != null)
                {
                    license.Dispose();
                }
            }
            return flag;
        }

        private static bool IsStackCallingAssemblyFriend(Assembly callingAssembly)
        {
            if (!IsEntryAssemblyLicenseCompiler())
            {
                StackTrace trace = new StackTrace(3, false);
                if (trace == null)
                {
                    return false;
                }
                Assembly assembly = null;
                List<Assembly> list = new List<Assembly>();
                for (int i = 0; i < trace.FrameCount; i++)
                {
                    StackFrame frame = trace.GetFrame(i);
                    MethodBase base2 = (frame != null) ? frame.GetMethod() : null;
                    Assembly item = ((base2 != null) && (base2.ReflectedType != null)) ? base2.ReflectedType.Assembly : null;
                    if (assembly == null)
                    {
                        assembly = item;
                    }
                    else if (((item != null) && (item != callingAssembly)) && !list.Contains(item))
                    {
                        if (IsCallingAssemblyFriend(item))
                        {
                            return true;
                        }
                        list.Add(item);
                    }
                }
            }
            return false;
        }

        internal static bool IsValid(object instance, Type type, Assembly callingAssembly, out SecurityException securityException)
        {
            securityException = null;
            return true;
            //try
            //{
            //    securityException = null;
            //    return (((IsInDesignMode(instance) || IsCallingAssemblyFriend(callingAssembly)) || IsStackCallingAssemblyFriend(callingAssembly)) || IsLicenseValid(instance, type));
            //}
            //catch (Exception exception)
            //{
            //    securityException = exception.GetBaseException() as SecurityException;
            //    if (securityException == null)
            //    {
            //        throw;
            //    }
            //    return false;
            //}
        }

        internal static void Validate(object instance, Type type)
        {
            //Validate(instance, type, null);
        }

        internal static void Validate(object instance, Type type, Assembly callingAssembly)
        {
            //SecurityException exception;
            //if (!IsValid(instance, type, callingAssembly, out exception))
            //{
            //    try
            //    {
            //        DispatcherFrame frame = new DispatcherFrame();
            //        Dispatcher.CurrentDispatcher.BeginInvoke((Action<object>) (sender =>
            //        {
            //            var  f = sender as DispatcherFrame;
            //            frame.Continue = false;
            //        }), frame);
            //        Dispatcher.PushFrame(frame);
            //    }
            //    catch (InvalidOperationException)
            //    {
            //    }
            //    if (exception != null)
            //    {
            //        LicenseWarningGenerator.Register(exception);
            //    }
            //    LicenseWarningGenerator.Register(type);
            //}
        }
    }
}

