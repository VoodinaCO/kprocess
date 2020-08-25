namespace DlhSoft.Windows.Controls
{
    using DlhSoft.Windows.Data;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    public interface IGanttChartView
    {

        #region Ajouts Tekigo

        /// <summary>
        /// Obtient ou définit une valeur permettant de désactiver la coercion du Timing des parents.
        /// </summary>
        bool DisableParentTimingCoercion { get; set; }

        #endregion

        event ItemActivatedEventHandler ItemActivated;

        event NotifyCollectionChangedEventHandler ItemCollectionChanged;

        event PropertyChangedEventHandler ItemPropertyChanged;

        event EventHandler TimelinePageChanged;

        void DecreaseTimelinePage();
        void Export(Delegate action);
        IEnumerable<GanttChartItem> GetChildren(GanttChartItem item);
        DateTime GetDateTime(double position);
        TimeSpan GetEffort(DateTime start, DateTime finish);
        BitmapSource GetExportBitmapSource();
        BitmapSource[,] GetExportBitmapSources(Size pageSize);
        Size GetExportSize();
        DateTime GetFinish(DateTime start, TimeSpan effort);
        GanttChartItem GetItemAt(double position);
        int GetItemIndexAt(double position);
        double GetItemTop(GanttChartItem item);
        DateTime GetNextNonworkingTime(DateTime dateTime);
        DateTime GetNextVisibleTime(DateTime dateTime);
        DateTime GetNextWorkingTime(DateTime dateTime);
        GanttChartItem GetParent(GanttChartItem item);
        double GetPosition(DateTime dateTime);
        DateTime GetPreviousNonworkingTime(DateTime dateTime);
        DateTime GetPreviousVisibleTime(DateTime dateTime);
        DateTime GetPreviousWorkingTime(DateTime dateTime);
        DateTime GetStart(TimeSpan effort, DateTime finish);
        IEnumerable<PredecessorItem> GetSuccessorPredecessorItems(GanttChartItem item);
        DateTime GetUpdateScaleTime(DateTime dateTime);
        IEnumerable<TimeInterval> GetWorkingTimeIntervals(DateTime start, DateTime finish);
        void IncreaseTimelinePage();
        void Print();
        void Print(string documentName);
        void ScrollTo(GanttChartItem item);
        void ScrollTo(DateTime dateTime);
        void SetTimelinePage(DateTime start, DateTime finish);

        bool AreUpdateTimelinePageButtonsVisible { get; set; }

        DataTemplate AssignmentsTemplate { get; set; }

        double BarHeight { get; set; }

        double CompletedBarHeight { get; set; }

        Brush CurrentTimeLineStroke { get; set; }

        DlhSoft.Windows.Controls.DependencyCreationValidator DependencyCreationValidator { get; set; }

        object DependencyDeletionContextMenuItemHeader { get; set; }

        DlhSoft.Windows.Controls.DependencyDeletionValidator DependencyDeletionValidator { get; set; }

        Brush DependencyLineStroke { get; set; }

        double DependencyLineStrokeThickness { get; set; }

        DataTemplate DependencyLineTemplate { get; set; }

        DateTime DisplayedTime { get; set; }

        Brush HeaderBackground { get; set; }

        double HeaderHeight { get; set; }

        double HourWidth { get; set; }

        bool IsCurrentTimeLineVisible { get; set; }

        bool IsDependencyToolTipVisible { get; set; }

        bool IsNonworkingTimeHighlighted { get; set; }

        bool IsReadOnly { get; set; }

        bool IsTaskCompletedEffortVisible { get; set; }

        bool IsTaskToolTipVisible { get; set; }

        double ItemHeight { get; set; }

        ObservableCollection<GanttChartItem> Items { get; set; }

        GanttChartItemCollection ManagedItems { get; }

        Brush MilestoneBarFill { get; set; }

        Brush MilestoneBarStroke { get; set; }

        double MilestoneBarStrokeThickness { get; set; }

        DataTemplate MilestoneTaskTemplate { get; set; }

        ObservableCollection<TimeInterval> NonworkingIntervals { get; set; }

        Brush NonworkingTimeBackground { get; set; }

        DataTemplate PredecessorToolTipTemplate { get; set; }

        double ScaleHeaderHeight { get; set; }

        ScaleCollection Scales { get; set; }

        double StandardBarCornerRadius { get; set; }

        Brush StandardBarFill { get; set; }

        Brush StandardBarStroke { get; set; }

        double StandardBarStrokeThickness { get; set; }

        double StandardCompletedBarCornerRadius { get; set; }

        Brush StandardCompletedBarFill { get; set; }

        Brush StandardCompletedBarStroke { get; set; }

        double StandardCompletedBarStrokeThickness { get; set; }

        DataTemplate StandardTaskTemplate { get; set; }

        Brush SummaryBarFill { get; set; }

        Brush SummaryBarStroke { get; set; }

        double SummaryBarStrokeThickness { get; set; }

        DataTemplate SummaryTaskTemplate { get; set; }

        DateTime TimelinePageFinish { get; set; }

        DateTime TimelinePageStart { get; set; }

        DataTemplate ToolTipTemplate { get; set; }

        TimeSpan UpdateScaleInterval { get; set; }

        TimeSpan UpdateTimelinePageAmount { get; set; }

        TimeOfDay VisibleDayFinish { get; set; }

        TimeOfDay VisibleDayStart { get; set; }

        DayOfWeek VisibleWeekFinish { get; set; }

        DayOfWeek VisibleWeekStart { get; set; }

        TimeOfDay WorkingDayFinish { get; set; }

        TimeOfDay WorkingDayStart { get; set; }

        Brush WorkingTimeBackground { get; set; }

        DayOfWeek WorkingWeekFinish { get; set; }

        DayOfWeek WorkingWeekStart { get; set; }
    }
}

