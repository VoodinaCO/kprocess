using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    /// Interaction logic for DependencyArrowLine.xaml
    /// </summary>
    public partial class DependencyArrowLine : UserControl
    {
        public DependencyArrowLine()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ArrowSizeProperty = DependencyProperty.Register("ArrowSize", typeof(double), typeof(DependencyArrowLine), new PropertyMetadata(5.0, new PropertyChangedCallback(DependencyArrowLine.OnPropertyChanged)));
        public static readonly DependencyProperty EndCapProperty = DependencyProperty.Register("EndCap", typeof(PenLineCap), typeof(DependencyArrowLine), new PropertyMetadata(PenLineCap.Triangle, new PropertyChangedCallback(DependencyArrowLine.OnPropertyChanged)));
        public static readonly DependencyProperty PointsProperty = DependencyProperty.Register("Points", typeof(PointCollection), typeof(DependencyArrowLine), new PropertyMetadata(null, new PropertyChangedCallback(DependencyArrowLine.OnPropertyChanged)));
        public static readonly DependencyProperty StartCapProperty = DependencyProperty.Register("StartCap", typeof(PenLineCap), typeof(DependencyArrowLine), new PropertyMetadata(PenLineCap.Flat, new PropertyChangedCallback(DependencyArrowLine.OnPropertyChanged)));
        public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register("Stroke", typeof(Brush), typeof(DependencyArrowLine), new PropertyMetadata(null, new PropertyChangedCallback(DependencyArrowLine.OnPropertyChanged)));
        public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register("StrokeThickness", typeof(double), typeof(DependencyArrowLine), new PropertyMetadata(1.0, new PropertyChangedCallback(DependencyArrowLine.OnPropertyChanged)));
        public static readonly DependencyProperty StrokeDashArrayProperty = DependencyProperty.Register("StrokeDashArray", typeof(DoubleCollection), typeof(DependencyArrowLine), new PropertyMetadata(null, new PropertyChangedCallback(DependencyArrowLine.OnPropertyChanged)));



        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DependencyArrowLine line = d as DependencyArrowLine;
            if (line != null)
            {
                line.Update();
            }
        }

        // Modifs Tekigo : Amélioration des perfs : 
        // Mise en cache des segments
        // Changement de l'ItemsSource uniquement lorsque c'est nécessaire
        // Ajout d'INotifyPropertyChanged sur l'objet
        private LineSegment[] _linesSegments;
        private void Update()
        {
            List<LineSegment> list = new List<LineSegment>();

            if ((this.Points != null) && (this.Points.Count > 0))
            {
                var shouldCreate = _linesSegments == null || _linesSegments.Length != this.Points.Count - 1;

                Point point = this.Points[0];
                for (int i = 1; i < this.Points.Count; i++)
                {
                    Point point2 = this.Points[i];
                    LineSegment segment;
                    if (shouldCreate)
                        segment = new LineSegment();
                    else
                        segment = _linesSegments[i - 1];

                    segment.X1 = point.X;
                    segment.Y1 = point.Y;
                    segment.X2 = point2.X;
                    segment.Y2 = point2.Y;
                    segment.Parent = this;
                    segment.StartCap = (i == 1) ? this.StartCap : PenLineCap.Flat;
                    segment.EndCap = (i == (this.Points.Count - 1)) ? this.EndCap : PenLineCap.Flat;

                    if (shouldCreate)
                        list.Add(segment);

                    point = point2;
                }

                if (shouldCreate)
                {
                    _linesSegments = list.ToArray();
                    this.LineItemsControl.ItemsSource = list;
                }
            }
            else
            {
                _linesSegments = null;
                this.LineItemsControl.ItemsSource = null;
            }

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

        public PointCollection Points
        {
            get
            {
                return (PointCollection)base.GetValue(PointsProperty);
            }
            set
            {
                base.SetValue(PointsProperty, value);
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

        public class LineSegment : INotifyPropertyChanged
        {
            private PenLineCap _endCap;
            public PenLineCap EndCap
            {
                get { return _endCap; }
                internal set
                {
                    if (_endCap != value)
                    {
                        _endCap = value;
                        OnPropertyChanged("EndCap");
                    }
                }
            }

            private DependencyArrowLine _parent;
            public DependencyArrowLine Parent
            {
                get { return _parent; }
                internal set
                {
                    if (_parent != value)
                    {
                        _parent = value;
                        OnPropertyChanged("Parent");
                    }
                }
            }

            private PenLineCap _startCap;
            public PenLineCap StartCap
            {
                get { return _startCap; }
                internal set
                {
                    if (_startCap != value)
                    {
                        _startCap = value;
                        OnPropertyChanged("StartCap");
                    }
                }
            }

            private double _x1;
            public double X1
            {
                get { return _x1; }
                internal set
                {
                    if (_x1 != value)
                    {
                        _x1 = value;
                        OnPropertyChanged("X1");
                    }
                }
            }

            private double _x2;
            public double X2
            {
                get { return _x2; }
                internal set
                {
                    if (_x2 != value)
                    {
                        _x2 = value;
                        OnPropertyChanged("X2");
                    }
                }
            }

            private double _y1;
            public double Y1
            {
                get { return _y1; }
                internal set
                {
                    if (_y1 != value)
                    {
                        _y1 = value;
                        OnPropertyChanged("Y1");
                    }
                }
            }

            private double _y2;
            public double Y2
            {
                get { return _y2; }
                internal set
                {
                    if (_y2 != value)
                    {
                        _y2 = value;
                        OnPropertyChanged("Y2");
                    }
                }
            }

            #region INotifyPropertyChanged Members

            public event PropertyChangedEventHandler PropertyChanged;

            /// <summary>
            /// Déclenche l'événement <see cref="PropertyChanged"/>
            /// </summary>
            /// <param name="propertyName"></param>
            protected virtual void OnPropertyChanged(string propertyName)
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }


            #endregion
        }
    }
}
