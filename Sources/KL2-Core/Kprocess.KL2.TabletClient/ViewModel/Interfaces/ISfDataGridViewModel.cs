using Syncfusion.UI.Xaml.Grid;
using System.Threading.Tasks;

namespace Kprocess.KL2.TabletClient.ViewModel
{
    /// <summary>
    /// Interface marqueuse permettant de gérer une SfDataGrid
    /// </summary>
    public interface ISfDataGridViewModel<T>
    {
        SfDataGrid DataGrid { get; set; }

        Task ScrollTo(T element);
    }
}
