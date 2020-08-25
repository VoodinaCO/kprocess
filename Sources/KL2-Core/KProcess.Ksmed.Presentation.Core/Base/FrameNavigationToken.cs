using KProcess.Presentation.Windows;
using System;
using System.Windows;

namespace KProcess.Ksmed.Presentation.Core
{
    /// <summary>
    /// Contient l'état de la dernière demande de navigation.
    /// </summary>
    /// <remarks>
    /// Cette classe introduit un effet de bord important dans le processus de navigation qui a néanmoins permis de réaliser la "User Story 3436" dans les temps.
    /// Si l'occasion se présente, il faudrait donc faire en sorte de supprimer cette classe au profit d'une autre mécanique sans effet de bord.
    /// TODO: Supprimer l'effet de bord du processus de navigation asynchrone
    /// </remarks>
    public static class FrameNavigationTokenState
    {
        /// <summary>
        /// Le dernier code du menu selectionné
        /// </summary>
        public static string LastMenuCodeAttempt { get; set; }

        /// <summary>
        /// Le dernier code du sous menu selectionné
        /// </summary>
        public static string LastSubMenuCodeAttempt { get; set; }
    }


    /// <summary>
    /// Implémente la logique de navigation du jeton de navigation
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    public class FrameNavigationToken<TViewModel> : IFrameNavigationToken, IDisposable
        where TViewModel: IFrameContentViewModel
    {
        //private readonly Action<TViewModel> _initialization = null;
        private bool _disposed = false;
        private bool _isEnable = false;

        /// <summary>
        /// Détermine si le jeton est valide.
        /// La valeur est fausse par défaut, car le jeton n'est valable que lorsque la validation synchrone (OnNavigatingAway) a été déclinée
        /// </summary>
        public bool IsValid { get { return !_disposed && _isEnable; } }

        /// <summary>
        /// Initialize le jeton de navigation
        /// </summary>
        /// <param name="initialization"></param>
        public FrameNavigationToken(Action<TViewModel> initialization = null)
        {
            //_initialization = null;
        }

        /// <summary>
        /// Execute l'opération de navigation du jeton
        /// </summary>
        public void Navigate()
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (_disposed)
                {
                    throw new ObjectDisposedException("The navigation token cannot be used one disposed. This may occur because you try to use the token twice.");
                }

                if (!_disposed && this.IsValid)
                {
                    this.Dispose();
                    //IoC.Resolve<IServiceBus>().Get<INavigationService>().TryShow<TViewModel>();
                    IoC.Resolve<IEventBus>().Publish(new NavigationRequestedEvent(this, FrameNavigationTokenState.LastMenuCodeAttempt, FrameNavigationTokenState.LastSubMenuCodeAttempt));
                }
            }));
        }

        /// <summary>
        /// Passe l'état du jeton à validé
        /// </summary>
        public void Activate()
        {
            _isEnable = true;
        }

        /// <summary>
        /// Invalide l'état du jeton
        /// </summary>
        public void Deactivate()
        {
            _isEnable = false;
        }

        /// <summary>
        /// Dispose l'état du jeton de façon à ce qu'il ne puisse plus être utilisé définitivement
        /// </summary>
        public void Dispose()
        {
            _disposed = true;
        }
    }
}
