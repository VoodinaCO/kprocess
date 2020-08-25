using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace KProcess.Ksmed.Models
{
    /// <summary>
    /// Fournit des méthodes d'aide à la gestion des couleurs.
    /// </summary>
    public static class ColorsHelper
    {
        static readonly Random _random = new Random();

        // Liste des couleurs prédéfinies :
        // http://msdn.microsoft.com/en-us/library/system.windows.media.colors.aspx

        static string[] _standardOfficeColorsStr =
        {
            "#FFFFFF", // White
            "#1F497D", // Dark Blue
            "#00FF00", // Green
            "#FF0000", // Red
            "#FFFF00", // Yellow
            "#FF00FF", // Fuchia
            "#8064A2", // Purple
            "#4BACC6", // Aqua
            "#F79646" // Orange
        };

        static string[] _standardColors =
        {
            "#00B0F0", // LightBlue
            "#FF0000", // Red
            "#779B06", // Green
            //"#00B050", // Green
            "#FFC000", // Orange
            "#92D050", // Light Green
            "#FFFF00", // Yellow
            "#0C75BD", // Blue
            //"#0070C0", // Blue
            "#C00000", // Dark red
            "#07357B", // Dark Blue
            //"#002060", // Dark Blue
            "#7030A0", // Purple
            "#000000", // Black
        };

        static string[] _randomColors_Exclude_Green_Yellow_Orange_Red =
        {
            //"#FFFFFF", // White
            "#000000", // Black
            "#EEECE1", // Gray
            "#4A4C51", // Dark Gray
            "#07357B", // Dark Blue
            "#0C75BD", // Blue
            "#8064A2", // Purple
            "#4BACC6", // Aqua
            "#B5144D", // Pink
            "#7B4B23", // Brown
            "#FF69B4", // HotPink
            "#008080", // Teal
            "#F5DEB3", // Wheat
            "#E9967A", // DarkSalmon
            "#6A5ACD", // SlateBlue
        };

        /// <summary>
        /// Initialise la classe <see cref="ColorsHelper"/>.
        /// </summary>
        static ColorsHelper()
        {
            ColorConverter converter = new ColorConverter();

            StandardColors = _standardColors.Select(c => (Color)converter.ConvertFrom(c)).ToArray();

            StandardTransparentColors = new SolidColorBrush[StandardColors.Length];

            byte alpha = 128;
            for (int i = 0; i < StandardColors.Length; i++)
            {
                Color color = StandardColors[i];
                StandardTransparentColors[i] = new SolidColorBrush(Color.FromArgb(alpha, color.R, color.G, color.B));
            }

            StandardOfficeColors = new Color[_standardOfficeColorsStr.Length];
            for (int i = 0; i < _standardOfficeColorsStr.Length; i++)
                StandardOfficeColors[i] = (Color)converter.ConvertFrom(_standardOfficeColorsStr[i]);

            StandardColorsExcludedGreenYellowOrangeRed = new Color[_randomColors_Exclude_Green_Yellow_Orange_Red.Length];
            for (int i = 0; i < _randomColors_Exclude_Green_Yellow_Orange_Red.Length; i++)
                StandardColorsExcludedGreenYellowOrangeRed[i] = (Color)converter.ConvertFrom(_randomColors_Exclude_Green_Yellow_Orange_Red[i]);
        }

        /// <summary>
        /// Obtient des couleurs standards.
        /// </summary>
        public static Color[] StandardColors { get; private set; }

        /// <summary>
        /// Obtient des couleurs standards transparentes.
        /// </summary>
        public static SolidColorBrush[] StandardTransparentColors { get; private set; }

        /// <summary>
        /// Obtient les couleurs standard correspondant à la palette d'office.
        /// </summary>
        public static Color[] StandardOfficeColors { get; private set; }

        /// <summary>
        /// Obtient les couleurs standards avec les couleurs suivantes exclues : Vert - Jaune - Orange - Rouge.
        /// </summary>
        public static Color[] StandardColorsExcludedGreenYellowOrangeRed { get; private set; }

        /// <summary>
        /// Obtient une couleur aléatoire.
        /// </summary>
        /// <param name="excluded">Les couleurs à exclure.</param>
        /// <returns>Une couleur aléatoire.</returns>
        public static SolidColorBrush GetNextAvailableBrush(SolidColorBrush[] brushes, IEnumerable<SolidColorBrush> excluded)
        {
            foreach (SolidColorBrush referenceBrush in brushes)
            {
                Color referenceColor = referenceBrush.Color;

                if (excluded.Any(brush => brush.Color.Equals(referenceColor)))
                    continue;

                return referenceBrush;
            }

            return brushes[_random.Next(brushes.Length)];
        }

        /// <summary>
        /// Obtient une couleur aléatoire.
        /// </summary>
        /// <param name="colors">Les couleurs disponibles.</param>
        /// <param name="exclude">Les couleurs à exclure.</param>
        /// <returns>
        /// Une couleur aléatoire.
        /// </returns>
        public static Color GetRandomColor(Color[] colors, IEnumerable<string> exclude = null)
        {
            if (exclude == null)
                return colors[_random.Next(colors.Length)];
            Dictionary<string, Color> clrs = colors.ToDictionary(c => c.ToString());

            string[] canPick = colors
                .Select(c => c.ToString())
                .Except(exclude)
                .ToArray();

            if (canPick.Length > 0)
                return clrs[canPick.ElementAt(_random.Next(canPick.Length))];
            return colors[_random.Next(colors.Length)];
        }

        /// <summary>
        /// Le nombre de dégradés créés par la méthode GetDarkLight.
        /// </summary>
        public static int DarkLightColorsCount = 5;

        /// <summary>
        /// Obtient des dégradés plus clairs et foncés que la couleur d'origine.
        /// </summary>
        /// <param name="c">La couleur.</param>
        /// <returns>Les couleurs dégradées.</returns>
        public static Color[] GetDarkLight(Color c)
        {
            HsvColor hsv = ConvertRgbToHsv(c.R, c.B, c.G);

            double[] ratios = new double[5];

            if (hsv.V > .9 && hsv.S < .1)
            {
                // Couleur très claire
                ratios[0] = .95;
                ratios[1] = .85;
                ratios[2] = .75;
                ratios[3] = .65;
                ratios[4] = .5;
            }
            else if (hsv.V < .1)
            {
                // Couleur très sombre
                ratios[0] = 1.5;
                ratios[1] = 1.35;
                ratios[2] = 1.25;
                ratios[3] = 1.15;
                ratios[4] = 1.05;
            }
            else
            {
                ratios[0] = 1.8;
                ratios[1] = 1.6;
                ratios[2] = 1.4;
                ratios[3] = .75;
                ratios[4] = .5;
            }

            Color[] colors = new Color[ratios.Length];
            for (int i = 0; i < ratios.Length; i++)
                colors[i] = ChangeLightness(c, ratios[i]);

            return colors;
        }

        /// <summary>
        /// Modifie la luminosité d'une couleur.
        /// Ratio &gt; 1 = plus clair. Ratio &lt; 1 = plus sombre.
        /// </summary>
        /// <param name="c">La couleur d'origine.</param>
        /// <param name="ratio">Le ratio.</param>
        /// <returns>La couleur convertie</returns>
        public static Color ChangeLightness(Color c, double ratio)
        {
            double r, g, b;

            if (ratio > 1.0)
            {
                r = c.R + (256 - c.R) * (ratio - 1);
                g = c.G + (256 - c.G) * (ratio - 1);
                b = c.B + (256 - c.B) * (ratio - 1);
            }
            else
            {
                r = c.R * ratio;
                g = c.G * ratio;
                b = c.B * ratio;
            }

            return Color.FromRgb((byte)r, (byte)g, (byte)b);
        }

        /// <summary>
        /// Converts an RGB color to an HSV color.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="b"></param>
        /// <param name="g"></param>
        /// <returns></returns>
        static HsvColor ConvertRgbToHsv(int r, int b, int g)
        {
            double delta, min;
            double h = 0, s, v;

            min = Math.Min(Math.Min(r, g), b);
            v = Math.Max(Math.Max(r, g), b);
            delta = v - min;

            if (v == 0.0)
                s = 0;
            else
                s = delta / v;

            if (s == 0)
                h = 0.0;

            else
            {
                if (r == v)
                    h = (g - b) / delta;
                else if (g == v)
                    h = 2 + (b - r) / delta;
                else if (b == v)
                    h = 4 + (r - g) / delta;

                h *= 60;
                if (h < 0.0)
                    h = h + 360;

            }

            return new HsvColor { H = h, S = s, V = v / 255 };
        }

        /// <summary>
        ///  Converts an HSV color to an RGB color.
        /// </summary>
        /// <param name="h"></param>
        /// <param name="s"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        static Color ConvertHsvToRgb(double h, double s, double v)
        {
            double r = 0, g = 0, b = 0;

            if (s == 0)
            {
                r = v;
                g = v;
                b = v;
            }
            else
            {
                int i;
                double f, p, q, t;

                if (h == 360)
                    h = 0;
                else
                    h = h / 60;

                i = (int)Math.Truncate(h);
                f = h - i;

                p = v * (1.0 - s);
                q = v * (1.0 - (s * f));
                t = v * (1.0 - (s * (1.0 - f)));

                switch (i)
                {
                    case 0:
                        {
                            r = v;
                            g = t;
                            b = p;
                            break;
                        }
                    case 1:
                        {
                            r = q;
                            g = v;
                            b = p;
                            break;
                        }
                    case 2:
                        {
                            r = p;
                            g = v;
                            b = t;
                            break;
                        }
                    case 3:
                        {
                            r = p;
                            g = q;
                            b = v;
                            break;
                        }
                    case 4:
                        {
                            r = t;
                            g = p;
                            b = v;
                            break;
                        }
                    default:
                        {
                            r = v;
                            g = p;
                            b = q;
                            break;
                        }
                }

            }

            return Color.FromArgb(255, (byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
        }

        /// <summary>
        /// Représente une couleur au format HSV.
        /// </summary>
        struct HsvColor
        {
            public double H;
            public double S;
            public double V;

            public HsvColor(double h, double s, double v)
            {
                H = h;
                S = s;
                V = v;
            }
        }

    }
}
