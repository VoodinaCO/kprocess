using KProcess.Ksmed.Models;
using System.Windows;
using System.Windows.Controls;

namespace Kprocess.KL2.TabletClient.Views
{
    public class QualifiedStyleSelector : StyleSelector
    {
        #region Properties

        /// <summary>
        /// Obtient ou définit si la sélection de style est pour le champs IsQualified ou IsNotQualified
        /// </summary>
        public bool IsQualified { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Style QualifiedStyle { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Style NothingStyle { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Style NotQualifiedStyle { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Style NotQualifiedImportantStyle { get; set; }

        #endregion

        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (item is PublishedAction data)
            {
                if (IsQualified)
                    return (data.IsQualified == true) ? QualifiedStyle : NothingStyle;
                return (data.IsQualified == false) ? ((data.IsKeyTask) ? NotQualifiedImportantStyle : NotQualifiedStyle) : NothingStyle;
            }

            return base.SelectStyle(item, container);
        }
    }
}
