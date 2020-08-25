//------------------------------------------------------------------------------
//*                                    Creation                            
//*************************************************************************
//* Fichier  : ReflectionHelper.cs
//* Auteur   : Tekigo
//* Creation : 
//* Role     : Fournit des méthodes permettant de récupérer des informations sur l'appelant d'une méthode.
//*************************************************************************
//*                                    Modifications              
//*************************************************************************
//*     Auteur            Date            Objet de la modification        
//*************************************************************************
//*
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace KProcess
{
    /// <summary>
    /// Fournit des méthodes permettant de récupérer des informations sur l'appelant d'une méthode.
    /// </summary>
    public static class ReflectionHelper
    {
        /// <summary>
        /// Retourne le nom complet qualifié du type à l'origine de l'appel.
        /// </summary>
        /// <param name="typesToIgnore">Ensemble des types à ignorer.</param>
        /// <returns>Nom complet qualifié du type à l'origine de l'appel.</returns>
        public static string GetCallingFullTypeName(params Type[] typesToIgnore)
        {
            return GetCallingMethod(typesToIgnore).ReflectedType.FullName;
        }

        /// <summary>
        /// Retourne la méthode à l'origine de l'appel.
        /// </summary>
        /// <param name="typesToIgnore">Ensemble des types à ignorer.</param>
        /// <returns>Méthode à l'origine de l'appel.</returns>
        public static MethodBase GetCallingMethod(params Type[] typesToIgnore)
        {
            StackFrame stackFrame = null;

            List<string> ignoreNames = new List<string>(typesToIgnore.Length);

            foreach (Type type in typesToIgnore)
                ignoreNames.Add(type.Name);

            StackTrace stackTrace = new StackTrace(true);
            MethodBase method = null;

            for (int i = 0; i < stackTrace.FrameCount; i++)
            {
                stackFrame = stackTrace.GetFrame(i);
                method = stackFrame.GetMethod();
                string typeName = method.ReflectedType.Name;

                if (String.Compare(typeName, "ReflectionHelper") != 0 && (ignoreNames.Count == 0 || !ignoreNames.Contains(typeName)))
                    break;
            }

            return method;
        }

        /// <summary>
        /// Obtient la première propriété qui correspond au nom et aux flags fournis en remontant étape par étape sur la classe parente.
        /// </summary>
        /// <param name="type">Le type.</param>
        /// <param name="propertyName">Le nomd e la propriété.</param>
        /// <param name="flags">Les binding flags.</param>
        /// <returns>La première propriété trouvée ou null.</returns>
        public static PropertyInfo GetPropertyFirstInHierarchy(this Type type, string propertyName, BindingFlags flags)
        {
            while (type != null)
            {
                var property = type.GetProperty(propertyName, flags | BindingFlags.DeclaredOnly);
                if (property != null)
                    return property;
                else
                    type = type.BaseType;
            }

            return null;
        }

        /// <summary>
        /// Vérifie si l'instance fournie est du type générique fourni et si les types arguments du type générique correspondent.
        /// </summary>
        /// <typeparam name="TActual">Le type réel de l'instance.</typeparam>
        /// <param name="actualInstance">L'instance.</param>
        /// <param name="expectedGenericType">Le type générique recherché. Utiliser par exemple <![CDATA[IDictionary<,>]]></param>
        /// <param name="expectedGenericTypeArguments">Les typse arguments recherchés. 
        /// Chaque argument spécifié sera testé dans l'ordre spécifié.
        /// Un type argument qui se trouve dans le type réel de l'instance et qui n'est pas spécifié dans les <paramref name="expectedGenericTypeArguments"/>
        /// sera marqué valide par défaut.</param>
        /// <returns><c>true</c> si l'instance fournie est du type générique fourni et si les types arguments du type générique correspondent.</returns>
        public static bool CheckGenericType<TActual>(TActual actualInstance, Type expectedGenericType, params Type[] expectedGenericTypeArguments)
        {
            if (actualInstance == null)
                throw new ArgumentNullException("actualInstance");

            return CheckGenericType(actualInstance.GetType(), expectedGenericType, expectedGenericTypeArguments);
        }

        /// <summary>
        /// Vérifie si le type fournie est du type générique fourni et si les types arguments du type générique correspondent.
        /// </summary>
        /// <param name="actualType">Le type à tester.</param>
        /// <param name="expectedGenericType">Le type générique recherché. Utiliser par exemple <![CDATA[IDictionary<,>]]></param>
        /// <param name="expectedGenericTypeArguments">Les typse arguments recherchés.
        /// Chaque argument spécifié sera testé dans l'ordre spécifié.
        /// Un type argument qui se trouve dans le type réel de l'instance et qui n'est pas spécifié dans les <paramref name="expectedGenericTypeArguments"/>
        /// sera marqué valide par défaut.</param>
        /// <returns>
        ///   <c>true</c> si l'instance fournie est du type générique fourni et si les types arguments du type générique correspondent.
        /// </returns>
        public static bool CheckGenericType(Type actualType, Type expectedGenericType, params Type[] expectedGenericTypeArguments)
        {
            Assertion.NotNull(actualType, "actualType");
            Assertion.NotNull(expectedGenericType, "expectedGenericType");

            bool isValueCorrect = true;

            var actualGenericType = actualType.GetInterface(expectedGenericType.Name);
            if (actualGenericType != null)
            {
                if (expectedGenericTypeArguments != null)
                {
                    var actualGenericTypeArguments = actualGenericType.GetGenericArguments();

                    if (actualGenericTypeArguments.Length < expectedGenericTypeArguments.Length)
                        isValueCorrect = false;
                    else
                    {
                        for (int i = 0; i < expectedGenericTypeArguments.Length; i++)
                        {
                            var currentExpectedType = expectedGenericTypeArguments[i];
                            var currentActualType = actualGenericTypeArguments[i];

                            if (currentExpectedType.IsInterface)
                            {
                                isValueCorrect &= currentActualType.GetInterface(currentExpectedType.Name) != null;
                            }
                            else
                                isValueCorrect &= currentExpectedType.IsAssignableFrom(currentActualType);
                        }
                    }
                }
            }
            else
                isValueCorrect = false;

            return isValueCorrect;
        }

        /// <summary>
        /// Obtient le nom de la propriété retournée par la lambda.
        /// </summary>
        /// <param name="expression">L'expression lambda.</param>
        /// <returns>le nom de la propriété retournée par la lambda</returns>
        public static string GetExpressionPropertyName(LambdaExpression expression)
        {
            var memExp = expression.Body as MemberExpression;
            if (memExp != null)
                return memExp.Member.Name;
            else
                throw new InvalidOperationException("L'expression ne doit contenir qu'un accesseur à une propriété");
        }

        /// <summary>
        /// Obtient la valeur de la propriete
        /// </summary>
        /// <param name="src"></param>
        /// <param name="propName"></param>
        /// <returns></returns>
        public static object GetPropertyValue(object src, string propName)
        {
            var property = src.GetType().GetProperty(propName);
            if (property == null)
                return null;
            return property.GetValue(src, null);
        }
    }
}
