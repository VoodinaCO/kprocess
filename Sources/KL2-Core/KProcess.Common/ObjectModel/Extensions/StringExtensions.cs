//------------------------------------------------------------------------------
//*                                    Creation                            
//*************************************************************************
//* Fichier  : StringExtensions.cs
//* Auteur   : Tekigo
//* Creation : 
//* Role     : Regroupe les méthodes d'extension de la classe String.
//*************************************************************************
//*                                    Modifications              
//*************************************************************************
//*     Auteur            Date            Objet de la modification        
//*************************************************************************
//*   
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KProcess
{
    /// <summary>
    /// Regroupe les méthodes d'extension de la classe String.
    /// </summary>
    public static class StringExtensions
    {
        #region Méthodes publiques

        /// <summary>
        /// Indique si une chaîne n'est ni nulle ni vide
        /// </summary>
        /// <param name="value">chaîne à tester</param>
        /// <returns>true si la chaîne n'est ni nulle ni vide, false sinon</returns>
        public static bool IsNotNullNorEmpty(this string value)
        {
            return !string.IsNullOrEmpty(value);
        }

        /// <summary>
        /// Concatène une chaîne avec une autre
        /// </summary>
        /// <param name="value">chaîne à concaténer</param>
        /// <param name="str1">chaine à concaténer</param>
        /// <returns>chaîne concaténée</returns>
        public static string Concatenate(this string value, string str1)
        {
            return String.Concat(str1);
        }

        /// <summary>
        /// Concatène une chaîne avec d'autres
        /// </summary>
        /// <param name="value">chaîne à concaténer</param>
        /// <param name="values">chaines à concaténer</param>
        /// <returns>chaîne concaténée</returns>
        public static string Concatenate(this string value, params string[] values)
        {
            return String.Concat(values);
        }

        /// <summary>
        /// Concatène une chaîne avec des objets
        /// </summary>
        /// <param name="value">chaîne à concaténer</param>
        /// <param name="values">objets à concaténer</param>
        /// <returns>chaîne concaténée</returns>
        public static string Concatenate(this string value, params object[] values)
        {
            return String.Concat(values);
        }

        /// <summary>
        /// Concatène une chaîne avec un objet
        /// </summary>
        /// <param name="value">chaîne à concaténer</param>
        /// <param name="obj1">chaine à concaténer</param>
        /// <returns>chaîne concaténée</returns>
        public static string Concatenate(this string value, object obj1)
        {
            return String.Concat(obj1);
        }

        /// <summary>
        /// Formate la chaîne avec les éléments fournis
        /// </summary>
        /// <param name="value">chaîne de formatage</param>
        /// <param name="values">objets à insérer dans la chaîne de formatage</param>
        /// <returns>la chaîne formatée</returns>
        public static string Format(this string value, params object[] values)
        {
            return String.Format(value, values);
        }

        /// <summary>
        /// Convertit un packet d'octets représentés en chaîne hexa en un tableau d'octets.
        /// </summary>
        /// <param name="hex">L'héxa.</param>
        /// <returns>Le tableau d'octets</returns>
        public static byte[] ToByteArray(this string hex)
        {
            if (hex.StartsWith("0x", StringComparison.InvariantCultureIgnoreCase))
                hex = hex.Substring(2);

            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        /// <summary>
        /// Convertit une énumération d'octets en chaîne de caractères.
        /// </summary>
        /// <param name="byteArray">Les octets.</param>
        /// <returns>La chaîne.</returns>
        public static string BytesToString(IEnumerable<byte> byteArray)
        {
            StringBuilder hex = new StringBuilder(byteArray.Count() * 2);
            foreach (byte b in byteArray)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        /// <summary>
        /// Compte le nombre d'occurences d'une chaîne dans une autre.
        /// </summary>
        /// <param name="input">La chaîne source.</param>
        /// <param name="value">La chaîne à rechercher.</param>
        /// <returns>Le nombre d'occurences</returns>
        public static int CountOccurences(this string input, string value)
        {
            int occurences = 0;
            int index = 0;
            while ((index = input.IndexOf(value, index)) != -1)
            {
                index++;
                occurences++;
            }

            return occurences;
        }

        #endregion
    }
}
