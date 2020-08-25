using System;

namespace KProcess
{
    /// <summary>
    /// Classe des exceptions techniques pour la couche Business
    /// </summary>
    [Serializable]
    public class ServerNotReacheableException : ExceptionBase
    {
        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="message">Message de l'exception.</param>
        public ServerNotReacheableException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="format">Chaîne de formatage.</param>
        /// <param name="args">Arguments de formatage.</param>
        public ServerNotReacheableException(string format, params object[] args)
            : base(string.Format(format, args))
        {
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="message">Message de l'exception.</param>
        /// <param name="innerException">Exception d'origine.</param>
        public ServerNotReacheableException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="innerException">Exception d'origine.</param>
        /// <param name="format">Chaîne de formatage.</param>
        /// <param name="args">Arguments de formatage.</param>
        public ServerNotReacheableException(Exception innerException, string format, params object[] args)
            : base(string.Format(format, args), innerException)
        {
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="info">La <see cref="T:System.Runtime.Serialization.SerializationInfo"/> quit contient les données de l'objet sérialisé concernant l'exception levée.</param>
        /// <param name="context">Le <see cref="T:System.Runtime.Serialization.StreamingContext"/> contenant des informations contextuelles à propos de la source ou de la destionation.</param>
        /// <exception cref="T:System.ArgumentNullException">Le paramètre <paramref name="info"/> est null.</exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">Le nom de la classe est null ou <see cref="P:System.Exception.HResult"/> vaut zéro (0). </exception>
        public ServerNotReacheableException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
    }
}
