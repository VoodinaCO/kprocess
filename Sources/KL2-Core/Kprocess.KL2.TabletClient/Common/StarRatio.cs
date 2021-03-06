﻿using System.Windows;

namespace Kprocess.KL2.TabletClient.Common
{
    public static class StarRatio
    {
        public static int GetColumnRatio(DependencyObject obj) =>
            (int)obj.GetValue(ColumnRatioProperty);

        public static void SetColumnRatio(DependencyObject obj, int value) =>
            obj.SetValue(ColumnRatioProperty, value);

        public static readonly DependencyProperty ColumnRatioProperty = DependencyProperty.RegisterAttached("ColumnRatio", typeof(int), typeof(StarRatio), new PropertyMetadata(1, null));
    }
}
