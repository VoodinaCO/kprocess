using log4net.ObjectRenderer;
using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace KProcess.Supervision.Log4net
{
    /// <summary>
    /// Représente un renderer pour l'exception de type ReflectionTypeLoadException, dont le but est de logger les exceptions
    /// contenues dans la propriété LoaderExceptions.
    /// </summary>
    public class ReflectionTypeLoadExceptionRenderer : IObjectRenderer
    {

        /// <summary>
        /// Obtient le type rendu.
        /// </summary>
        internal static Type RendereredType
        {
            get { return typeof(ReflectionTypeLoadException); }
        }

        /// <summary>
        /// Effectue le rendu de l'objet.
        /// </summary>
        /// <param name="rendererMap">La map</param>
        /// <param name="obj">L'objet dont le rendu doit être effectué.</param>
        /// <param name="writer">Le writer dans lequel effectuer le rendu.</param>
        public void RenderObject(RendererMap rendererMap, object obj, TextWriter writer)
        {
            var ex = obj as ReflectionTypeLoadException;
            if (ex == null)
                throw new ArgumentException("The obj is not a ReflectionTypeLoadException");

            var sb = new StringBuilder();
            RenderReflectionTypeLoadException(ex, sb, 0);
            writer.WriteLine(sb.ToString());
        }

        /// <summary>
        /// Effectue le rendu de l'exception
        /// </summary>
        /// <param name="ex">L'exception.</param>
        /// <param name="sb">Le StringBuilder sur lequel effectuer le rendu.</param>
        /// <param name="indentation">L'indentation.</param>
        private void RenderReflectionTypeLoadException(ReflectionTypeLoadException ex, StringBuilder sb, int indentation)
        {
            sb.Append(' ', indentation * 2);
            sb.Append(ex.GetType().FullName);
            sb.Append(": ");
            sb.Append(ex.Message);
            sb.Append(Environment.NewLine);
            sb.Append(' ', indentation * 2);
            sb.Append(ex.StackTrace);
            sb.Append(Environment.NewLine);
            sb.Append("LoaderExceptions:");
            sb.Append(Environment.NewLine);

            foreach (var loaderEx in ex.LoaderExceptions)
            {
                var subLoaderEx = loaderEx as ReflectionTypeLoadException;
                if (subLoaderEx != null)
                    RenderReflectionTypeLoadException(subLoaderEx, sb, indentation + 1);
                else
                {
                    sb.Append(' ', (indentation + 1) * 2);
                    sb.Append(loaderEx.ToString());
                    sb.Append(Environment.NewLine);
                }
            }
            sb.Append(Environment.NewLine);

        }



    }
}
