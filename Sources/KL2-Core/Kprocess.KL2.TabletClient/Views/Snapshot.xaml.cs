using System.Windows.Controls;

namespace Kprocess.KL2.TabletClient.Views
{
    /// <summary>
    /// Logique d'interaction pour Snapshot.xaml
    /// </summary>
    public partial class Snapshot : UserControl
    {
        public Snapshot()
        {
            InitializeComponent();
            CapturePlayer.MediaInitializing += CapturePlayer_MediaInitializing;
        }

        void CapturePlayer_MediaInitializing(object sender, Unosquare.FFME.Common.MediaInitializingEventArgs e)
        {
            //e.Options.Input["video_size"] = FFMEWebcam.InputVideoSize;
        }
    }
}
