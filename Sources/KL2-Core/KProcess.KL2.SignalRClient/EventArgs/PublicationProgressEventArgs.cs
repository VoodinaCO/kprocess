using KProcess.Ksmed.Models;
using System;

namespace KProcess.KL2.SignalRClient
{
    public class PublicationProgressEventArgs : EventArgs
    {
        public int PublicationHistoryId { get; }

        public PublicationStatus Status { get; }

        public string Step { get; }

        public double? Progress { get; }

        public bool CanCancel { get; }

        public string ErrorMessage { get; }

        public PublicationProgressEventArgs(int publicationHistoryId,
            PublicationStatus status = PublicationStatus.Waiting,
            string step = null,
            double? progress = null,
            bool canCancel = false,
            string errorMessage = null)
        {
            PublicationHistoryId = publicationHistoryId;
            Status = status;
            Step = step;
            Progress = progress;
            CanCancel = canCancel;
            ErrorMessage = errorMessage;
        }

        public override string ToString() =>
            $"Id:{PublicationHistoryId}, Status:{Status}, CanCancel:{CanCancel}{(Step != null ? $", Step:{Step}" : "")}{(Progress != null ? $", Progress:{Progress}%" : "")}{(ErrorMessage != null ? $", ErrorMessage:{ErrorMessage}" : "")}";
    }
}
