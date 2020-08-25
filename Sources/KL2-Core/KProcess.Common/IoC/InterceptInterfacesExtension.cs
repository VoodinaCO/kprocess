//------------------------------------------------------------------------------
//*                                    Creation                            
//*************************************************************************
//* Fichier  : InterceptInterfacesExtension.cs
//* Auteur   : Tekigo
//* Creation : 
//* Role     : Extension pour Unity permettant d'intercepter toutes les méthodes des interfaces IInterceptable.
//*************************************************************************
//*                                    Modifications              
//*************************************************************************
//*     Auteur            Date            Objet de la modification        
//*************************************************************************
//*   
//------------------------------------------------------------------------------

using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;
using System;

namespace KProcess
{
    /// <summary>
    /// Extension pour Unity permettant d'intercepter toutes les méthodes des interfaces IInterceptable.
    /// </summary>
    public class InterceptInterfacesExtension : UnityContainerExtension
    {
        #region Surcharges

        /// <summary>
        /// Initial the container with this extension's functionality.
        /// </summary>
        /// <remarks>
        /// When overridden in a derived class, this method will modify the given
        /// <see cref="T:Microsoft.Practices.Unity.ExtensionContext"/> by adding strategies, policies, etc. to
        /// install it's functions into the container.</remarks>
        protected override void Initialize()
        {
            // Ajoute l'extension
            Container.AddNewExtension<Interception>();

            // S'abonne aux événements d'enregistrement pour intercepter chaque type / instance
            Context.Registering += new EventHandler<RegisterEventArgs>(Context_Registering);
            Context.RegisteringInstance += new EventHandler<RegisterInstanceEventArgs>(Context_RegisteringInstance);
        }

        /// <summary>
        /// Removes the extension's functions from the container.
        /// </summary>
        /// <remarks>
        /// 	<para>
        /// This method is called when extensions are being removed from the container. It can be
        /// used to do things like disconnect event handlers or clean up member state. You do not
        /// need to remove strategies or policies here; the container will do that automatically.
        /// </para>
        /// 	<para>
        /// The default implementation of this method does nothing.</para>
        /// </remarks>
        public override void Remove()
        {
            // Désabonnement
            Context.Registering -= new EventHandler<RegisterEventArgs>(Context_Registering);
            Context.RegisteringInstance -= new EventHandler<RegisterInstanceEventArgs>(Context_RegisteringInstance);

            base.Remove();
        }

        #endregion

        #region Gestionnaire d'événements

        private void Context_RegisteringInstance(object sender, RegisterInstanceEventArgs e)
        {
            SetInterceptorFor(e.RegisteredType, e.Instance.GetType());
        }

        private void Context_Registering(object sender, RegisterEventArgs e)
        {
            SetInterceptorFor(e.TypeFrom, e.TypeTo);
        }

        #endregion

        #region Méthodes privées

        private void SetInterceptorFor(Type typeToIntercept, Type typeIntercepted)
        {
            if (typeToIntercept != null && typeToIntercept.GetInterface(typeof(IInterceptable).Name) != null)
            {
                // Configure le type intercepté pour être intercepté grâce aux méthodes virtuelles
                Container
                    .Configure<Interception>()
                    .SetDefaultInterceptorFor(typeIntercepted, new VirtualMethodInterceptor());
            }
        }

        #endregion
    }
}