using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace KProcess.Presentation.Windows.Helpers
{
    /// <summary>
    /// Fournit des méthodes d'aide au drag &amp; drop.
    /// </summary>
    public static class DragDropHelper
    {

        /// <summary>
        /// Obtient les données de drag &amp; drop en fonction du type générique spécifié.
        /// </summary>
        /// <typeparam name="TData">Le type de données à récupérer.</typeparam>
        /// <param name="e">Les données de l'évènement.</param>
        /// <returns>
        /// Les données de drag &amp; drop, ou <c>null</c> si le type n'y est pas présent.
        /// </returns>
        public static TData GetData<TData>(DragEventArgs e)
        {
            return GetData<TData>(null, e);
        }

        /// <summary>
        /// Obtient les données de drag &amp; drop en fonction du type générique spécifié.
        /// </summary>
        /// <typeparam name="TData">Le type de données à récupérer.</typeparam>
        /// <param name="format">Le format des données.</param>
        /// <param name="e">Les données de l'évènement.</param>
        /// <returns>
        /// Les données de drag &amp; drop, ou <c>null</c> si le type ou le format n'y sont pas présent.
        /// </returns>
        public static TData GetData<TData>(string format, DragEventArgs e)
        {
            TData draggedData = default(TData);

            var formats = e.Data.GetFormats();
            if (formats != null)
            {
                if (format == null)
                    format = e.Data.GetFormats().FirstOrDefault();

                if (format != null && e.Data.GetDataPresent(format))
                {
                    var data = e.Data.GetData(format);
                    if (data is TData)
                        draggedData = (TData)data;
                }
            }

            return draggedData;
        }

    }
}
