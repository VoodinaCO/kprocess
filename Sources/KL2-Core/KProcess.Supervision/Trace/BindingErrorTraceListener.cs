using System.Diagnostics;

namespace KProcess.Supervision.Trace
{
    /// <summary>
    /// 
    /// </summary>
    public class BindingErrorTraceListener : TraceListener
    {
        readonly ITraceManager _traceManager;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="traceManager"></param>
        public BindingErrorTraceListener(ITraceManager traceManager)
        {
            _traceManager = traceManager;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public override void Write(string message) =>
            _traceManager.TraceDebug($"Binding Error : {message}");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public override void WriteLine(string message) =>
            _traceManager.TraceDebug($"Binding Error : {message}");
    }
}
