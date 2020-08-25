namespace Kprocess.KL2.FileTransfer
{
    //
    // Résumé :
    //     Defines constant values for the different states of a job.
    public enum TransferStatus
    {
        //
        // Résumé :
        //     Specifies that the job is in the queue and waiting to run. If a user logs off
        //     while their job is transferring, the job transitions to the queued state.
        Queued = 0,
        //
        // Résumé :
        //     Specifies that BITS is trying to connect to the server. If the connection succeeds,
        //     the state of the job becomes usis.Net.Bits.BackgroundCopyJobState.Transferring;
        //     otherwise, the state becomes usis.Net.Bits.BackgroundCopyJobState.Error.
        Connecting = 1,
        //
        // Résumé :
        //     Specifies that BITS is transferring data for the job.
        Transferring = 2,
        //
        // Résumé :
        //     Specifies that the job is suspended (paused). To suspend a job, call the usis.Net.Bits.BackgroundCopyJob.Suspend
        //     method. BITS automatically suspends a job when it is created. The job remains
        //     suspended until you call the usis.Net.Bits.BackgroundCopyJob.Resume, usis.Net.Bits.BackgroundCopyJob.Complete,
        //     or usis.Net.Bits.BackgroundCopyJob.Cancel method.
        Suspended = 3,
        //
        // Résumé :
        //     Specifies that a non-recoverable error occurred (the service is unable to transfer
        //     the file). If the error, such as an access-denied error, can be corrected, call
        //     the usis.Net.Bits.BackgroundCopyJob.Resume method after the error is fixed. However,
        //     if the error cannot be corrected, call the usis.Net.Bits.BackgroundCopyJob.Cancel
        //     method to cancel the job, or call the usis.Net.Bits.BackgroundCopyJob.Complete
        //     method to accept the portion of a download job that transferred successfully.
        Error = 4,
        //
        // Résumé :
        //     Specifies that a recoverable error occurred. BITS will retry jobs in the transient
        //     error state based on the retry interval you specify (see usis.Net.Bits.BackgroundCopyJob.MinimumRetryDelay).
        //     The state of the job changes to usis.Net.Bits.BackgroundCopyJobState.Error if
        //     the job fails to make progress (see usis.Net.Bits.BackgroundCopyJob.NoProgressTimeout).
        //     BITS does not retry the job if a network disconnect or disk lock error occurred
        //     (for example, chkdsk is running) or the MaxInternetBandwidth Group Policy is
        //     zero.
        TransientError = 5,
        //
        // Résumé :
        //     Specifies that your job was successfully processed. You must call the usis.Net.Bits.BackgroundCopyJob.Complete
        //     method to acknowledge completion of the job and to make the files available to
        //     the client.
        Transferred = 6,
        //
        // Résumé :
        //     Specifies that you called the usis.Net.Bits.BackgroundCopyJob.Complete method
        //     to acknowledge that your job completed successfully.
        Acknowledged = 7,
        //
        // Résumé :
        //     Specifies that you called the usis.Net.Bits.BackgroundCopyJob.Cancel method to
        //     cancel the job (remove the job from the transfer queue).
        Canceled = 8
    }
}
