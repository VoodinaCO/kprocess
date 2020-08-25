//------------------------------------------------------------------------------
//*                                    Creation                            
//*************************************************************************
//* Fichier  : IoC.cs
//* Auteur   : Tekigo
//* Creation : 
//* Role     : Façade pour l'inversion de contrôle
//*************************************************************************
//*                                    Modifications              
//*************************************************************************
//*     Auteur            Date            Objet de la modification        
//*************************************************************************
//*   
//------------------------------------------------------------------------------

using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace KProcess
{
    /// <summary>
    /// Façade pour l'inversion de contrôle
    /// </summary>
    /// <remarks>un conteneur statique est créé automatiquement</remarks>
    public static class IoC
    {
        #region Attributs

        /// <summary>
        /// 
        /// </summary>
        public static IUnityContainer Container { get; private set; }

        #endregion

        #region Constructeur

        /// <summary>
        /// Constructeur statique
        /// </summary>
        static IoC()
        {
            try
            {
                // Création du conteneur Unity
                Container = new UnityContainer();

                // Ajoute l'extension permettant d'intercepter les IInterceptables
                Container.AddNewExtension<InterceptInterfacesExtension>();

                // Ajoute l'extension permettant de surcharger la création des validateurs VAB
                Container.AddNewExtension<Common.Validation.UnityCustomValidationFactoryExtension>();

                // Application de la configuration à partir du fichier de configuration
                if (ConfigurationManager.GetSection("unity") != null)
                    Container.LoadConfiguration();
            }
            catch (System.Threading.SynchronizationLockException e)
            {
                Console.WriteLine($"SynchronizationLockException : {e.Message}");
            }
        }

        #endregion

        #region Méthodes publiques

        /// <summary>
        /// Résoud un type
        /// </summary>
        /// <param name="pType">Type à résoudre</param>
        /// <returns>une instance du type <paramref name="pType"/> ou sa valeur par défaut</returns>
        public static object Resolve(Type pType)
        {
            return Container.Resolve(pType);
        }

        /// <summary>
        /// Résoud un type
        /// </summary>
        /// <typeparam name="T">type à résoudre</typeparam>
        /// <returns>une instance du type T ou sa valeur par défaut</returns>
        public static T Resolve<T>()
        {
            return Container.Resolve<T>();
        }

        /// <summary>
        /// Résoud un type à partir d'un nom
        /// </summary>
        /// <typeparam name="T">type à résoudre</typeparam>
        /// <param name="name">nom de l'instance</param>
        /// <returns>une instance du type T ou sa valeur par défaut</returns>
        public static T Resolve<T>(string name)
        {
            return Container.Resolve<T>(name);
        }

        /// <summary>
        /// Résoud all type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> ResolveAll<T>()
        {
            return Container.ResolveAll<T>();
        }

        /// <summary>
        /// Enregistre un type
        /// </summary>
        /// <typeparam name="TFrom">type source</typeparam>
        /// <typeparam name="TTo">type cible</typeparam>
        /// <param name="asSingleton">indique si lors de sa résolution, le type doit être conservé comme un singleton</param>
        public static void RegisterType<TFrom, TTo>(bool asSingleton = false)
            where TTo : TFrom
        {
            if (asSingleton)
                Container.RegisterType<TFrom, TTo>(new ContainerControlledLifetimeManager());
            else
                Container.RegisterType<TFrom, TTo>();
        }

        /// <summary>
        /// Enregistre un type
        /// </summary>
        /// <typeparam name="TFrom">type source</typeparam>
        /// <typeparam name="TTo">type cible</typeparam>
        /// <param name="name">nom de l'instance</param>
        /// <param name="asSingleton">indique si lors de sa résolution, le type doit être conservé comme un singleton</param>
        public static void RegisterType<TFrom, TTo>(string name, bool asSingleton = false)
            where TTo : TFrom
        {
            if (asSingleton)
                Container.RegisterType<TFrom, TTo>(name, new ContainerControlledLifetimeManager());
            else
                Container.RegisterType<TFrom, TTo>(name);
        }

        /// <summary>
        /// Enregistre un type à partir d'un délégué
        /// </summary>
        /// <typeparam name="T">type à enregistrer</typeparam>
        /// <param name="factory">délégué fabriquant une instance de T</param>
        public static void RegisterType<T>(Func<T> factory)
        {
            if (!Container.IsRegistered<T>())
                Container.RegisterType<T>(new InjectionFactory(c => factory()));
        }

        /// <summary>
        /// Enregistre une instance (en singleton)
        /// </summary>
        /// <typeparam name="T">type de l'instance</typeparam>
        /// <param name="instance">instance à enregistrer</param>
        public static void RegisterInstance<T>(T instance)
        {
            Container.RegisterInstance(instance);
        }

        /// <summary>
        /// Enregistre une instance avec son nom
        /// </summary>
        /// <typeparam name="T">type de l'instance</typeparam>
        /// <param name="name">nom de l'instance</param>
        /// <param name="instance">instance à enregistrer</param>
        public static void RegisterInstance<T>(string name, T instance)
        {
            Container.RegisterInstance(name, instance);
        }

        /// <summary>
        /// Retourne un conteneur enfant
        /// </summary>
        /// <returns>un nouveau conteneur enfant</returns>
        /// <remarks>à utiliser avec un bloc using pour disposer son contenu</remarks>
        public static object CreateChild()
        {
            return Container.CreateChildContainer();
        }

        /// <summary>
        /// Indique si un type est enregistré dans le conteneur
        /// </summary>
        /// <typeparam name="T">type à rechercher</typeparam>
        /// <returns>true s'il est enregistré, false sinon</returns>
        public static bool IsRegistered<T>()
        {
            return Container.IsRegistered<T>();
        }

        /// <summary>
        /// Indique si un type est enregistré dans le conteneur
        /// </summary>
        /// <typeparam name="T">type à rechercher</typeparam>
        /// <param name="name">nom de l'instance</param>
        /// <returns>true s'il est enregistré, false sinon</returns>
        public static bool IsRegistered<T>(string name)
        {
            return Container.IsRegistered<T>(name);
        }

        /// <summary>
        /// Nettoie une instance du conteneur
        /// </summary>
        /// <param name="instance">instance à nettoyer</param>
        public static void CleanUp(object instance)
        {
            Container.Teardown(instance);
        }

        #endregion
    }
}