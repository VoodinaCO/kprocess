using System;
using System.Threading;
using System.Threading.Tasks;

namespace Kprocess.KL2.FileTransfer
{
    public interface ITransferOperation
    {
        event EventHandler<JobType> OnTransferFinished;

        JobType JobType { get; }

        string Group { get; }

        Guid Guid { get; }

        BackgroundTransferProgress Progress { get; }

        string Error { get; }

        TransferStatus State { get; }

        bool IsFinished { get; set; }

        bool CanResume { get; set; }

        bool CanPause { get; set; }

        bool CanCancel { get; set; }

        void Resume();

        void Pause();

        TaskResult Cancel();

        Task<TaskResult> WaitTransferFinished(CancellationTokenSource cts = null);
    }
}
