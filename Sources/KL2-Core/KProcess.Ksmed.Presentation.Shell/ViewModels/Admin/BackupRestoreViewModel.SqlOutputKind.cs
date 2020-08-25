using KProcess.Ksmed.Presentation.Core;
using KProcess.Ksmed.Presentation.ViewModels.Interfaces;
using KProcess.Presentation.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Data.SqlClient;
using System.Configuration;
using KProcess.Globalization;
using System.Collections.ObjectModel;
using System.ComponentModel;
using KProcess.Ksmed.Business;
using System.Windows.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Windows;

namespace KProcess.Ksmed.Presentation.ViewModels
{
    partial class BackupRestoreViewModel
    {
        /// <summary>
        /// Définit les types de message à afficher dans la console
        /// </summary>
        public enum SqlOutputKind
        {
            Server,
            Info,
            Success,
            Error,
            Exception,
            Path
        }
    }
}
