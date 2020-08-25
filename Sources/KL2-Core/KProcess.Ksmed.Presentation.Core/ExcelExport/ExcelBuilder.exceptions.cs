using System;
using System.IO;

namespace KProcess.Ksmed.Presentation.Core.ExcelExport
{
    partial class ExcelExporter
    {
        /// <summary>
        /// Correspond à une exception levé lorsque le fichier n'est pas accessible en écriture probablement parce qu'il est déjà utilisé par un autre process.
        /// </summary>
        public class FileAlreadyInUseExeption : IOException
        {
            public FileAlreadyInUseExeption(string message, Exception innerException)
                :base(message, innerException)
            {

            }

            public FileAlreadyInUseExeption(string message)
                : base(message)
            {

            }
        }
    }
}
