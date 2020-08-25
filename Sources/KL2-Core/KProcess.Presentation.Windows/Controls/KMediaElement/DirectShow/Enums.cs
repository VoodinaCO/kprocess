#pragma warning disable 1591

namespace KProcess.Presentation.Windows.Controls.DirectShow
{
    /// <summary>
    /// The types of position formats that
    /// are available for seeking media
    /// </summary>
    public enum MediaPositionFormat
    {
        MediaTime,
        Frame,
        Byte,
        Field,
        Sample,
        None
    }

    /// <summary>
    /// Specifies different types of DirectShow
    /// Video Renderers
    /// </summary>
    public enum VideoRendererType
    {
        VideoMixingRenderer9 = 0,
        EnhancedVideoRenderer
    }
}

#pragma warning restore 1591
