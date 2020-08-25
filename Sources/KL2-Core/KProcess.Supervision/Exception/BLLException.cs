using System;
using System.Runtime.Serialization;

namespace KProcess
{
    /// <summary>
    /// Classe d'exception pour la couche business
    /// </summary>
    [Serializable]
    public abstract class BLLException : ExceptionBase
    {
        #region Constructeurs

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="message">Message de l'exception.</param>
        public BLLException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="format">Chaîne de formatage.</param>
        /// <param name="args">Arguments de formatage.</param>
        public BLLException(string format, params object[] args)
            : base(String.Format(format, args))
        {
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="message">Message de l'exception.</param>
        /// <param name="innerException">Exception d'origine.</param>
        public BLLException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="innerException">Exception d'origine.</param>
        /// <param name="format">Chaîne de formatage.</param>
        /// <param name="args">Arguments de formatage.</param>
        public BLLException(Exception innerException, string format, params object[] args)
            : base(String.Format(format, args), innerException)
        {
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="info">La <see cref="T:System.Runtime.Serialization.SerializationInfo"/> quit contient les données de l'objet sérialisé concernant l'exception levée.</param>
        /// <param name="context">Le <see cref="T:System.Runtime.Serialization.StreamingContext"/> contenant des informations contextuelles à propos de la source ou de la destionation.</param>
        /// <exception cref="T:System.ArgumentNullException">Le paramètre <paramref name="info"/> est null.</exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">Le nom de la classe est null ou <see cref="P:System.Exception.HResult"/> vaut zéro (0). </exception>
        public BLLException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            ErrorCode = info.GetInt32(nameof(ErrorCode));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));
            info.AddValue(nameof(ErrorCode), ErrorCode);
            base.GetObjectData(info, context);
        }

        #endregion

        /// <summary>
        /// Obtient ou définit le code de l'erreur.
        /// </summary>
        public int ErrorCode { get; set; }
    }
}
