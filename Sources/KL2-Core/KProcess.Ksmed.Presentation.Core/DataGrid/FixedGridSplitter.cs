using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace KProcess.Ksmed.Presentation.Core
{
    public class FixedGridSplitter : GridSplitter
    {
        private Grid _grid;
        private RowDefinition _topRow;
        private RowDefinition _bottomRow;
        private double _topRowMavedMaxLength;
        private double _bottomRowMavedMaxLength;

        #region static

        static FixedGridSplitter()
        {
            new GridSplitter();
            EventManager.RegisterClassHandler(typeof(FixedGridSplitter), Thumb.DragCompletedEvent, new DragCompletedEventHandler(FixedGridSplitter.OnDragCompleted));
            EventManager.RegisterClassHandler(typeof(FixedGridSplitter), Thumb.DragStartedEvent, new DragStartedEventHandler(FixedGridSplitter.OnDragStarted));
        }

        private static void OnDragStarted(object sender, DragStartedEventArgs e)
        {
            FixedGridSplitter splitter = (FixedGridSplitter)sender;
            splitter.OnDragStarted(e);
        }

        private static void OnDragCompleted(object sender, DragCompletedEventArgs e)
        {
            FixedGridSplitter splitter = (FixedGridSplitter)sender;
            splitter.OnDragCompleted(e);
        }

        #endregion

        private void OnDragStarted(DragStartedEventArgs sender)
        {
            _grid = Parent as Grid;

            if (_grid == null)
                return;

            int splitterIndex = (int)GetValue(Grid.RowProperty);

            _topRow = _grid.RowDefinitions[splitterIndex - 1];
            _bottomRow = _grid.RowDefinitions[splitterIndex + 1];

            _topRowMavedMaxLength = _topRow.MaxHeight;
            _bottomRowMavedMaxLength = _bottomRow.MaxHeight;

            double topRowMaxHeight = _topRow.ActualHeight + _bottomRow.ActualHeight - _bottomRow.MinHeight;
            _topRow.MaxHeight = topRowMaxHeight;

            double bottomRowMaxHeight = _topRow.ActualHeight + _bottomRow.ActualHeight - _topRow.MinHeight;
            _bottomRow.MaxHeight = bottomRowMaxHeight;
        }

        private void OnDragCompleted(DragCompletedEventArgs sender)
        {
            _topRow.MaxHeight = _topRowMavedMaxLength;
            _bottomRow.MaxHeight = _bottomRowMavedMaxLength;
            _grid = null;
            _topRow = null;
        }
    }
}
