namespace KProcess.Ksmed.Models
{
    partial class CutVideo
    {
        public CutVideo(KAction action)
        {
            Start = action.Start;
            End = action.Finish;
        }
    }
}
