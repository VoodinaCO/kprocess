using KProcess.Ksmed.Models;
using KProcess.Ksmed.Presentation.Core;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace KProcess.Ksmed.Presentation.Shell
{
    /// <summary>
    /// Représente le service de gestion des couleurs des vidéos.
    /// </summary>
    public class VideoColorService : IVideoColorService
    {
        private IVideoColorPersistanceService _persistance;
        private Dictionary<string, Brush> _videoColorsBrushes = new Dictionary<string, Brush>();

        /// <summary>
        /// Constructeur.
        /// </summary>
        /// <param name="persistance">Le service de persistance.</param>
        public VideoColorService(IVideoColorPersistanceService persistance)
        {
            _persistance = persistance;
        }

        /// <inheritdoc />
        public Brush GetColor(Video video)
        {
            var colors = _persistance.VideoColors;

            if (colors == null)
                colors = new Dictionary<(int ProcessId, int VideoId), string>();

            var id = (video.ProcessId, video.VideoId);

            if (colors.ContainsKey(id))
            {
                var brushStr = colors[id];
                if (!_videoColorsBrushes.ContainsKey(brushStr))
                    _videoColorsBrushes[brushStr] = (Brush)new BrushConverter().ConvertFrom(brushStr);

                return _videoColorsBrushes[brushStr];
            }
            else
            {
                var otherBrushesKeys = colors.Keys.Where(k => k.ProjectId == video.ProcessId).Except(id).ToArray();
                var otherBrushes = new List<Brush>();

                foreach (var otherBrushKey in otherBrushesKeys)
                {
                    var otherBrushStr = colors[otherBrushKey];

                    if (!_videoColorsBrushes.ContainsKey(otherBrushStr))
                        _videoColorsBrushes[otherBrushStr] = (Brush)new BrushConverter().ConvertFrom(otherBrushStr);

                    otherBrushes.Add(_videoColorsBrushes[otherBrushStr]);
                }

                var brush = ColorsHelper.GetNextAvailableBrush(ColorsHelper.StandardTransparentColors, otherBrushes.Cast<SolidColorBrush>());

                var brushStr = brush.Color.ToString();

                colors[id] = brushStr;
                _videoColorsBrushes[brushStr] = brush;

                _persistance.VideoColors = colors;

                return brush;
            }
        }
    }

    /// <summary>
    /// Décrit le comportement du service de persistance des couleurs de vidéos.
    /// </summary>
    public interface IVideoColorPersistanceService : IService
    {
        /// <summary>
        /// Obtient ou définit les couleurs des vidéos.
        /// </summary>
        IDictionary<(int ProjectId, int VideoId), string> VideoColors { get; set; }
    }


}
