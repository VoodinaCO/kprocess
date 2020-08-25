using System;

#pragma warning disable 1591

namespace KProcess.Presentation.Windows.Controls.DirectShow
{
    /// <summary>
    /// EventArgs for the NewAllocatorSurfaceEvent event
    /// </summary>
    public class NewAllocatorSurfaceEventArgs : EventArgs
    {
        public NewAllocatorSurfaceEventArgs(IntPtr surface)
        {
            Surface = surface;
        }

        public IntPtr Surface { get; private set; }
    }
}

#pragma warning disable 1591