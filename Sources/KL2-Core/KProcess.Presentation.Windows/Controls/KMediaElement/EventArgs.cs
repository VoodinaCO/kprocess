using System;
using System.Windows;

#pragma warning disable 1591

namespace KProcess.Presentation.Windows.Controls
{
    public class MediaFailedEventArgs : EventArgs
    {
        public MediaFailedEventArgs(string message, string fileSource, Exception exception)
        {
            Message = message;
            FileSource = fileSource;
            Exception = exception;
        }

        public string FileSource { get; set; }
        public Exception Exception { get; private set; }
        public string Message { get; private set; }
    }

    public delegate void RoutedMediaFailedEventHandler(object sender, RoutedMediaFailedEventArgs e);

    public class RoutedMediaFailedEventArgs : RoutedEventArgs
    {
        public RoutedMediaFailedEventArgs(RoutedEvent routedEvent, string fileSource, Exception exception)
            : base(routedEvent)
        {
            FileSource = fileSource;
            Exception = exception;
        }

        public string FileSource { get; set; }
        public Exception Exception { get; private set; }
    }
}

#pragma warning restore 1591