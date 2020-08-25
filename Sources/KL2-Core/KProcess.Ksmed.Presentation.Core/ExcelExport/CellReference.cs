using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KProcess.Ksmed.Presentation.Core.ExcelExport
{

    /// <summary>
    /// Représente la référence d'une cellule.
    /// </summary>
    public class CellReference
    {
        /// <summary>
        /// Obtient l'index de la colonne. Débute à 1.
        /// </summary>
        public uint ColumnIndex { get; private set; }

        /// <summary>
        /// Obtient la référence de la colonne.
        /// </summary>
        public string ColumnReference { get; private set; }

        /// <summary>
        /// Obtient l'index de la ligne. Débute à 1.
        /// </summary>
        public uint RowIndex { get; private set; }

        /// <summary>
        /// Obtient la référence de la cellule.
        /// </summary>
        public string Reference { get; private set; }

        /// <summary>
        /// Obtient les dimensions max des cellules visitées.
        /// </summary>
        public string Dimensions { get; private set; }

        private uint _minColumn;
        private uint _maxColumn;
        private uint _minRow;
        private uint _maxRow;

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="CellReference"/>.
        /// </summary>
        public CellReference()
        {
            this.ColumnIndex = 1;
            this.RowIndex = 1;
            UpdateReference();
        }

        /// <summary>
        /// Copie une référence.
        /// </summary>
        /// <param name="other">L'autre référence.</param>
        public CellReference(CellReference other)
        {
            this.ColumnIndex = other.ColumnIndex;
            this.RowIndex = other.RowIndex;
            this.Reference = other.Reference;
            this.ColumnReference = other.ColumnReference;
            _minColumn = other._minColumn;
            _maxColumn = other._maxColumn;
            _minRow = other._minRow;
            _maxRow = other._maxRow;
            this.Dimensions = other.Dimensions;
        }

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="CellReference"/>.
        /// </summary>
        /// <param name="cell">La valeur de la cellule.</param>
        /// <example>
        /// A1
        /// E2
        /// AZ340
        /// </example>
        public CellReference(string cell)
        {
            cell = cell.ToUpper();

            uint columns = 0;

            uint rows = 0;

            int firstLetterValue = (int)'A'; 

            foreach (var c in cell)
            {
                if (Char.IsLetter(c))
                {
                    var columnValue = (int)c - firstLetterValue + 1;
                    columns *= 26;
                    columns += (uint)columnValue;
                }
                else if (Char.IsNumber(c))
                {
                    rows *= 10;
                    rows += (uint)Char.GetNumericValue(c);
                }
            }

            this.ColumnIndex = columns;
            this.RowIndex = rows;
            UpdateReference();
        }

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="CellReference"/>.
        /// </summary>
        /// <param name="columnIndex">L'index de la colonne.</param>
        /// <param name="rowIndex">L'index de la ligne.</param>
        public CellReference(uint columnIndex, uint rowIndex)
        {
            if (columnIndex == 0)
                throw new ArgumentOutOfRangeException("columnIndex");
            if (rowIndex == 0)
                throw new ArgumentOutOfRangeException("rowIndex");

            this.ColumnIndex = columnIndex;
            this.RowIndex = rowIndex;
            UpdateReference();
        }

        /// <summary>
        /// Déplace le curseur à droite.
        /// </summary>
        public void MoveRight()
        {
            ColumnIndex++;
            UpdateReference();
        }

        /// <summary>
        /// Déplace le curseur vers une nouveau ligne.
        /// </summary>
        public void NewLine()
        {
            RowIndex++;
            ColumnIndex = 1;
            UpdateReference();
        }

        /// <summary>
        /// Met à jours les références et dimensions.
        /// </summary>
        private void UpdateReference()
        {
            string columnReference;
            this.Reference = GetCellReference(this.ColumnIndex, this.RowIndex, out columnReference);
            this.ColumnReference = columnReference;

            // mettre à jour les dimensions
            _minColumn = Math.Min(_minColumn, this.ColumnIndex);
            _maxColumn = Math.Max(_maxColumn, this.ColumnIndex);
            _minRow = Math.Min(_minRow, this.RowIndex);
            _maxRow = Math.Max(_maxRow, this.RowIndex);

            this.Dimensions = string.Format("{0}:{1}", GetCellReference(_minColumn, _minRow, out columnReference), GetCellReference(_maxColumn, _maxRow, out columnReference));
        }

        /// <summary>
        /// Obtient la référence d'une cellule à partir de sa colonne et de sa ligne.
        /// </summary>
        /// <param name="column">La colonne.</param>
        /// <param name="row">la ligne.</param>
        /// <returns>La référence.</returns>
        private string GetCellReference(uint column, uint row, out string columReference)
        {
            int dividend = (int)column;
            columReference = String.Empty;
            int modulo;

            while (dividend > 0)
            {
                modulo = (dividend - 1) % 26;
                columReference = Convert.ToChar(65 + modulo).ToString() + columReference;
                dividend = (int)((dividend - modulo) / 26);
            }

            return columReference + this.RowIndex.ToString();
        }

    }
}
