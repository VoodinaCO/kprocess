using System;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

#pragma warning disable 1591

namespace KProcess.Presentation.Windows.Controls
{
    /// <summary>
    /// This is a WPF pixel shader effect that will scale 16-235 HD-TV pixel output to
    /// 0-255 pixel values for Deep color on video.
    /// </summary>
    public class DeepColorEffect : ShaderEffect
    {
        private static string _assemblyShortName;

        public static DependencyProperty InputProperty = RegisterPixelShaderSamplerProperty("Input", typeof(DeepColorEffect), 0);

        public DeepColorEffect()
        {
            var u = new Uri(@"pack://application:,,,/" + AssemblyShortName + ";component/PixelShaders/DeepColor/DeepColor.ps");            
            PixelShader = new PixelShader{ UriSource = u };
            UpdateShaderValue(InputProperty);
        }

        private static string AssemblyShortName
        {
            get
            {
                if (_assemblyShortName == null)
                {
                    Assembly a = typeof(DeepColorEffect).Assembly;
                    _assemblyShortName = a.ToString().Split(',')[0];
                }

                return _assemblyShortName;
            }
        }
 
        public Brush Input
        {
            get
            {
                return ((Brush)(GetValue(InputProperty)));
            }
            set
            {
                SetValue(InputProperty, value);
            }
        }
    }
}

#pragma warning restore 1591
