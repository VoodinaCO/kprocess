using System;
using System.Runtime.InteropServices;

namespace AForge.Video.DirectShow.Internals
{
    /// <summary>
	/// DirectShow class IDs.
	/// </summary>
    [ComVisible(false)]
    static internal class Clsid
    {
        /// <summary>
        /// System device enumerator.
        /// </summary>
        /// 
        /// <remarks>Equals to CLSID_SystemDeviceEnum.</remarks>
        /// 
        public static readonly Guid SystemDeviceEnum =
            new Guid(0x62BE5D10, 0x60EB, 0x11D0, 0xBD, 0x3B, 0x00, 0xA0, 0xC9, 0x11, 0xCE, 0x86);

        /// <summary>
        /// Filter graph.
        /// </summary>
        /// 
        /// <remarks>Equals to CLSID_FilterGraph.</remarks>
        /// 
        public static readonly Guid FilterGraph =
            new Guid(0xE436EBB3, 0x524F, 0x11CE, 0x9F, 0x53, 0x00, 0x20, 0xAF, 0x0B, 0xA7, 0x70);

        /// <summary>
        /// Sample grabber.
        /// </summary>
        /// 
        /// <remarks>Equals to CLSID_SampleGrabber.</remarks>
        /// 
        public static readonly Guid SampleGrabber =
            new Guid(0xC1F400A0, 0x3F08, 0x11D3, 0x9F, 0x0B, 0x00, 0x60, 0x08, 0x03, 0x9E, 0x37);

        /// <summary>
        /// Capture graph builder.
        /// </summary>
        /// 
        /// <remarks>Equals to CLSID_CaptureGraphBuilder2.</remarks>
        /// 
        public static readonly Guid CaptureGraphBuilder2 =
            new Guid(0xBF87B6E1, 0x8C27, 0x11D0, 0xB3, 0xF0, 0x00, 0xAA, 0x00, 0x37, 0x61, 0xC5);

        /// <summary>
        /// Async reader.
        /// </summary>
        /// 
        /// <remarks>Equals to CLSID_AsyncReader.</remarks>
        /// 
        public static readonly Guid AsyncReader =
            new Guid(0xE436EBB5, 0x524F, 0x11CE, 0x9F, 0x53, 0x00, 0x20, 0xAF, 0x0B, 0xA7, 0x70);
    }

    /// <summary>
    /// DirectShow format types.
    /// </summary>
    /// 
    [ComVisible(false)]
    static internal class FormatType
    {
        /// <summary>
        /// VideoInfo.
        /// </summary>
        /// 
        /// <remarks>Equals to FORMAT_VideoInfo.</remarks>
        /// 
        public static readonly Guid VideoInfo =
            new Guid(0x05589F80, 0xC356, 0x11CE, 0xBF, 0x01, 0x00, 0xAA, 0x00, 0x55, 0x59, 0x5A);

        /// <summary>
        /// VideoInfo2.
        /// </summary>
        /// 
        /// <remarks>Equals to FORMAT_VideoInfo2.</remarks>
        /// 
        public static readonly Guid VideoInfo2 =
            new Guid(0xf72A76A0, 0xEB0A, 0x11D0, 0xAC, 0xE4, 0x00, 0x00, 0xC0, 0xCC, 0x16, 0xBA);
    }
}
