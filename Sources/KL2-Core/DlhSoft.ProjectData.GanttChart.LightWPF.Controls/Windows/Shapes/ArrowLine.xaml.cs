using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DlhSoft.Windows.Shapes
{
    /// <summary>
    /// Interaction logic for ArrowLine.xaml
    /// </summary>
    public partial class ArrowLine : UserControl
    {
        public ArrowLine()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ArrowSizeProperty = DependencyProperty.Register("ArrowSize", typeof(double), typeof(ArrowLine), new PropertyMetadata(5.0, new PropertyChangedCallback(ArrowLine.OnPropertyChanged)));
        public static readonly DependencyProperty EndCapProperty = DependencyProperty.Register("EndCap", typeof(PenLineCap), typeof(ArrowLine), new PropertyMetadata(PenLineCap.Triangle, new PropertyChangedCallback(ArrowLine.OnPropertyChanged)));
        public static readonly DependencyProperty StartCapProperty = DependencyProperty.Register("StartCap", typeof(PenLineCap), typeof(ArrowLine), new PropertyMetadata(PenLineCap.Flat, new PropertyChangedCallback(ArrowLine.OnPropertyChanged)));
        public static readonly DependencyProperty StrokeDashArrayProperty = DependencyProperty.Register("StrokeDashArray", typeof(DoubleCollection), typeof(ArrowLine), new PropertyMetadata(null, new PropertyChangedCallback(ArrowLine.OnPropertyChanged)));
        public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register("Stroke", typeof(Brush), typeof(ArrowLine), new PropertyMetadata(null, new PropertyChangedCallback(ArrowLine.OnPropertyChanged)));
        public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register("StrokeThickness", typeof(double), typeof(ArrowLine), new PropertyMetadata(1.0, new PropertyChangedCallback(ArrowLine.OnPropertyChanged)));
        public static readonly DependencyProperty X1Property = DependencyProperty.Register("X1", typeof(double), typeof(ArrowLine), new PropertyMetadata(0.0, new PropertyChangedCallback(ArrowLine.OnPropertyChanged)));
        public static readonly DependencyProperty X2Property = DependencyProperty.Register("X2", typeof(double), typeof(ArrowLine), new PropertyMetadata(0.0, new PropertyChangedCallback(ArrowLine.OnPropertyChanged)));
        public static readonly DependencyProperty Y1Property = DependencyProperty.Register("Y1", typeof(double), typeof(ArrowLine), new PropertyMetadata(0.0, new PropertyChangedCallback(ArrowLine.OnPropertyChanged)));
        public static readonly DependencyProperty Y2Property = DependencyProperty.Register("Y2", typeof(double), typeof(ArrowLine), new PropertyMetadata(0.0, new PropertyChangedCallback(ArrowLine.OnPropertyChanged)));


        private static void OnPropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ArrowLine line = sender as ArrowLine;
            if (line != null)
            {
                line.Update();
            }
        }


        private void Update()
        {
            double num = this.X1;
            double num2 = this.Y1;
            double num3 = this.X2;
            double num4 = this.Y2;
            double a = Math.Atan2(num4 - num2, num3 - num);
            double num6 = Math.Sin(a);
            double num7 = Math.Cos(a);
            double num8 = this.ArrowSize / 2.0;
            if (this.StartCap != PenLineCap.Flat)
            {
                num += num8 * num7;
                num2 += num8 * num6;
            }
            if (this.EndCap != PenLineCap.Flat)
            {
                num3 -= num8 * num7;
                num4 -= num8 * num6;
            }
            this.Line.X1 = num;
            this.Line.Y1 = num2;
            this.Line.X2 = num3;
            this.Line.Y2 = num4;
            this.Line.StrokeThickness = this.StrokeThickness;
            this.Line.Stroke = this.Stroke;
            this.Line.StrokeDashArray = this.StrokeDashArray;
            a *= 57.295779513082323;
            this.StartCapLine.X1 = num;
            this.StartCapLine.Y1 = num2;
            this.StartCapLine.X2 = num;
            this.StartCapLine.Y2 = num2;
            this.StartCapLine.StrokeStartLineCap = this.StartCap;
            this.StartCapLine.StrokeThickness = this.ArrowSize;
            this.StartCapLine.Stroke = this.Stroke;
            this.StartRotateTransform.Angle = a;
            this.StartRotateTransform.CenterX = num;
            this.StartRotateTransform.CenterY = num2;
            this.EndCapLine.X1 = num3;
            this.EndCapLine.Y1 = num4;
            this.EndCapLine.X2 = num3;
            this.EndCapLine.Y2 = num4;
            this.EndCapLine.StrokeEndLineCap = this.EndCap;
            this.EndCapLine.StrokeThickness = this.ArrowSize;
            this.EndCapLine.Stroke = this.Stroke;
            this.EndRotateTransform.Angle = a;
            this.EndRotateTransform.CenterX = num3;
            this.EndRotateTransform.CenterY = num4;
        }

        public double ArrowSize
        {
            get
            {
                return (double)base.GetValue(ArrowSizeProperty);
            }
            set
            {
                base.SetValue(ArrowSizeProperty, value);
            }
        }

        public PenLineCap EndCap
        {
            get
            {
                return (PenLineCap)base.GetValue(EndCapProperty);
            }
            set
            {
                base.SetValue(EndCapProperty, value);
            }
        }

        public PenLineCap StartCap
        {
            get
            {
                return (PenLineCap)base.GetValue(StartCapProperty);
            }
            set
            {
                base.SetValue(StartCapProperty, value);
            }
        }

        public Brush Stroke
        {
            get
            {
                return (Brush)base.GetValue(StrokeProperty);
            }
            set
            {
                base.SetValue(StrokeProperty, value);
            }
        }

        public DoubleCollection StrokeDashArray
        {
            get
            {
                return (DoubleCollection)base.GetValue(StrokeDashArrayProperty);
            }
            set
            {
                base.SetValue(StrokeDashArrayProperty, value);
            }
        }

        public double StrokeThickness
        {
            get
            {
                return (double)base.GetValue(StrokeThicknessProperty);
            }
            set
            {
                base.SetValue(StrokeThicknessProperty, value);
            }
        }

        public double X1
        {
            get
            {
                return (double)base.GetValue(X1Property);
            }
            set
            {
                base.SetValue(X1Property, value);
            }
        }

        public double X2
        {
            get
            {
                return (double)base.GetValue(X2Property);
            }
            set
            {
                base.SetValue(X2Property, value);
            }
        }

        public double Y1
        {
            get
            {
                return (double)base.GetValue(Y1Property);
            }
            set
            {
                base.SetValue(Y1Property, value);
            }
        }

        public double Y2
        {
            get
            {
                return (double)base.GetValue(Y2Property);
            }
            set
            {
                base.SetValue(Y2Property, value);
            }
        }

    }
}
