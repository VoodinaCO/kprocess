using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using KProcess.Ksmed.Presentation.Core.Behaviors;

namespace KProcess.Ksmed.Presentation.ViewModels.Restitution
{
    public abstract class ReferentialBase<TReferentialItemBase>
        where TReferentialItemBase : ReferentialItemBase
    {

        private TReferentialItemBase[] _items;

        /// <summary>
        /// Obtient ou définit le nom du scénario.
        /// </summary>
        public string Scenario { get; set; }

        /// <summary>
        /// Obtient ou définit les sous éléments.
        /// </summary>
        public TReferentialItemBase[] Items
        {
            get { return _items; }
            set
            {
                _items = value;
                ItemsReversed = _items != null ? _items.Reverse().ToArray() : null;
            }
        }


        /// <summary>
        /// Obtient ou définit les sous éléments.
        /// </summary>
        public TReferentialItemBase[] ItemsReversed { get; set; }
    }


    public abstract class ReferentialItemBase : IComparable<ReferentialItemBase>, IComparable, IConvertible
    {
        public string ReferentialName { get; set; }

        public long ReferentialDuration { get; set; }

        public int ReferentialOccurrences { get; set; }

        public string ReferentialDurationFormatted { get; set; }

        public double ReferentialPercentage { get; set; }

        public string ReferentialPercentageFormatted { get; set; }

        public string Description { get; set; }

        public bool IsStandard { get; set; }

        public string DurationAndPercentageFormatted
        {
            get
            {
                if (this.ValueMode == RestitutionValueMode.Occurences)
                    return ReferentialOccurrences != 0 ? ReferentialDurationFormatted + Environment.NewLine + ReferentialOccurrences :
                        string.Empty;
                else
                    return ReferentialDuration != 0 || ReferentialPercentage != 0 ?
                        ReferentialDurationFormatted + Environment.NewLine + ReferentialPercentageFormatted :
                        string.Empty;
            }
        }

        public RestitutionValueMode ValueMode { get; set; }

        /// <summary>
        /// Obtient ou définit la couleur de remplissage.
        /// </summary>
        public Brush FillBrush { get; set; }

        /// <summary>
        /// Obtient ou définit la couleur de la bordure.
        /// </summary>
        public Brush StrokeBrush { get; set; }

        public int CompareTo(object obj)
        {
            return this.CompareTo((ReferentialItemBase)obj);
        }

        public int CompareTo(ReferentialItemBase other)
        {
            return ReferentialDuration.CompareTo(other.ReferentialDuration);
        }

        public override string ToString()
        {
            return ReferentialName;
        }

        #region IConvertible Members

        public double ToDouble(IFormatProvider provider)
        {
            switch (ValueMode)
            {
                case RestitutionValueMode.Absolute:
                    return this.ReferentialDuration;
                case RestitutionValueMode.Relative:
                    return this.ReferentialPercentage;
                case RestitutionValueMode.Occurences:
                    return this.ReferentialOccurrences;
                default:
                    throw new ArgumentOutOfRangeException("ValueMode");
            }
        }

        public TypeCode GetTypeCode()
        {
            throw new NotImplementedException();
        }

        public bool ToBoolean(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public byte ToByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public char ToChar(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public short ToInt16(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public int ToInt32(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public long ToInt64(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public sbyte ToSByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public float ToSingle(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public string ToString(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public ushort ToUInt16(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public uint ToUInt32(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public ulong ToUInt64(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
