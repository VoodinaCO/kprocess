using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KProcess.Ksmed.Presentation.Core.ExcelExport
{
    /// <summary>
    /// Représente une plage de celulles.
    /// </summary>
    public class CellRange
    {
        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="CellRange"/>.
        /// </summary>
        public CellRange()
        {

        }

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="CellRange"/>.
        /// </summary>
        public CellRange(uint fromColumn, uint fromRow, uint toColumn, uint toRow)
        {
            this.From = new CellReference(fromColumn, fromRow);
            this.To = new CellReference(toColumn, toRow);
        }

        /// <summary>
        /// Obtient ou définit le point de départ.
        /// </summary>
        public CellReference From { get; set; }

        /// <summary>
        /// Obtient ou définit le point d'arrivée.
        /// </summary>
        public CellReference To { get; set; }

        /// <summary>
        /// Obtient toutes les cellules de la plage.
        /// </summary>
        /// <returns>Les cellules.</returns>
        public IEnumerable<CellReference> GetCells()
        {
            for (uint i = From.ColumnIndex; i < To.ColumnIndex; i++)
            {
                for (uint j = From.RowIndex; j < To.RowIndex; j++)
                {
                    yield return new CellReference(i, j);
                }
            }
        }

    }
}
