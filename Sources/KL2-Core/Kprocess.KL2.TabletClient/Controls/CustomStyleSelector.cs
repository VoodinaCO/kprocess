using System.Windows;
using System.Windows.Controls;

namespace Kprocess.KL2.TabletClient.Controls
{
    public class CustomStyleSelector : StyleSelector
    {
        #region Attributs

        static int _rowCounter;

        #endregion

        #region Properties

        public Style GroupRowStyle { get; set; }
        public Style RowStyle { get; set; }

        #endregion

        #region Override Methods

        public override Style SelectStyle(object item, DependencyObject container)
        {
            _rowCounter++;

            return RowStyle;
        }

        #endregion
    }
}
