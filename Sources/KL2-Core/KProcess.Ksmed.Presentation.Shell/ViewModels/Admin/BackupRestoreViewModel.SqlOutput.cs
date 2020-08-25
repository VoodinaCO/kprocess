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
        /// Définit un message à afficher dans la console
        /// </summary>
        public class SqlOutput
        {
            public SqlOutput()
            {

            }

            public SqlOutput(Exception e)
            {
                this.Message = e.Message;
                this.Kind = SqlOutputKind.Exception;
            }

            public SqlOutput(SqlInfoMessageEventArgs e)
            {
                this.Message = e.Message;
                this.Kind = SqlOutputKind.Server;
            }

            public SqlOutput(string message, SqlOutputKind kind = SqlOutputKind.Info)
            {
                this.Message = message;
                this.Kind = kind;
            }

            /// <summary>
            /// Obtient ou définit le message à afficher dans la console
            /// </summary>
            public string Message { get; set; }

            /// <summary>
            /// Contient le type de message
            /// </summary>
            public SqlOutputKind Kind { get; set; }
        }
    }
}
