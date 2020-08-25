using System.Windows.Input;
using Unosquare.FFME;

namespace Kprocess.KL2.TabletClient.ViewModel
{
    /// <summary>
    /// Interface marqueuse permettant de gérer le lecteur vidéo
    /// </summary>
    public interface IMediaElementViewModel
    {
        MediaElement MediaElement { get; set; }

        ICommand PlayPauseCommand { get; }

        ICommand MuteCommand { get; }

        ICommand MaximizeCommand { get; }
    }
}
