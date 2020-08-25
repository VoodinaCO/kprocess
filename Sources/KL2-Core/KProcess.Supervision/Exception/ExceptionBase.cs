﻿using System;

namespace KProcess
{
    /// <summary>
    /// Classe de base des exceptions
    /// </summary>
    [Serializable]
    public abstract class ExceptionBase : Exception
    {
        #region Constructeurs

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="message">Message de l'exception.</param>
        public ExceptionBase(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="format">Chaîne de formatage.</param>
        /// <param name="args">Arguments de formatage.</param>
        public ExceptionBase(string format, params object[] args)
            : base(String.Format(format, args))
        {
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="message">Message de l'exception.</param>
        /// <param name="innerException">Exception d'origine.</param>
        public ExceptionBase(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="innerException">Exception d'origine.</param>
        /// <param name="format">Chaîne de formatage.</param>
        /// <param name="args">Arguments de formatage.</param>
        public ExceptionBase(Exception innerException, string format, params object[] args)
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
        public ExceptionBase(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }

        #endregion

    }
}