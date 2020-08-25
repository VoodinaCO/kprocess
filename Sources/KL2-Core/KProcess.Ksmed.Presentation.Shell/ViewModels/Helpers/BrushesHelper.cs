using KProcess.Ksmed.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace KProcess.Ksmed.Presentation.ViewModels
{
    /// <summary>
    /// Fournit des méthodes d'aide de gestion des pinceaux.
    /// </summary>
    public static class BrushesHelper
    {
        private static readonly Color DefaultColor = Colors.LightBlue;

        private static List<BrushReference> _cache = new List<BrushReference>();

        /// <summary>
        /// Crée des pinceaux de remplissage et de bordure pour la couleur spécifiée.
        /// </summary>
        /// <param name="colorCode">Le code de couleur.</param>
        /// <param name="addTransparency"><c>true</c> pour ajouter de la transparence.</param>
        /// <param name="fillBrush">Le pinceau de remplissage.</param>
        /// <param name="strokeBrush">Le pinceau de bordure.</param>
        public static void GetBrush(string colorCode, bool addTransparency, out Brush fillBrush, out Brush strokeBrush)
        {
            var cache = GetFromCache(colorCode, addTransparency);
            if (cache != null)
            {
                fillBrush = cache.FillBrush;
                strokeBrush = cache.StrokeBrush;
                return;
            }

            Color color = default(Color);
            var converter = new ColorConverter();

            if (!string.IsNullOrEmpty(colorCode))
            {
                try
                {
                    // Tenter de convertir la couleur dans la catégorie
                    color = (Color)converter.ConvertFrom(colorCode);
                }
                catch (Exception) { }
            }

            if (color == default(Color))
                color = DefaultColor;

            // On crée deux couleurs dérivées : une à moitié transparent, une plus foncée
            Color startColor;
            if (addTransparency)
                startColor = Color.FromArgb((byte)128, color.R, color.G, color.B);
            else
                startColor = ColorsHelper.ChangeLightness(color, 1.5);

            var endColor = Color.FromRgb((byte)((short)color.R * .8), (byte)((short)color.G * .8), (byte)((short)color.B * .8));

            var gb = new LinearGradientBrush() { StartPoint = new Point(0, 0), EndPoint = new Point(0, 1) };
            gb.GradientStops.Add(new GradientStop(startColor, 0));
            gb.GradientStops.Add(new GradientStop(color, .75));
            gb.GradientStops.Add(new GradientStop(endColor, 1));
            gb.Freeze();

            fillBrush = gb;
            strokeBrush = new SolidColorBrush(endColor);

            SaveIntoCache(colorCode, addTransparency, fillBrush, strokeBrush);
        }

        /// <summary>
        /// Sauve les pinceaux dans le cache.
        /// </summary>
        /// <param name="colorCode">Le code de couleur.</param>
        /// <param name="addTransparency"><c>true</c> pour ajouter de la transparence.</param>
        /// <param name="fillBrush">Le pinceau de remplissage.</param>
        /// <param name="strokeBrush">Le pinceau de bordure.</param>
        private static void SaveIntoCache(string colorCode, bool addTransparency, Brush fillBrush, Brush strokeBrush)
        {
            _cache.Add(new BrushReference
            {
                ColorCode = colorCode ?? string.Empty,
                AddTransparency = addTransparency,
                FillBrush = fillBrush,
                StrokeBrush = strokeBrush,
            });
        }

        /// <summary>
        /// Obtient les pinceaux depuis le cache.
        /// </summary>
        /// <param name="colorCode">Le code de couleur.</param>
        /// <param name="addTransparency"><c>true</c> pour ajouter de la transparence.</param>
        /// <returns>Les pinceaux ou null si pas trouvé.</returns>
        private static BrushReference GetFromCache(string colorCode, bool addTransparency)
        {
            return _cache.FirstOrDefault(b => b.ColorCode == (colorCode ?? string.Empty) && b.AddTransparency == addTransparency);
        }

        private class BrushReference
        {
            public string ColorCode { get; set; }
            public bool AddTransparency { get; set; }
            public Brush FillBrush { get; set; }
            public Brush StrokeBrush { get; set; }
        }
    }
}
