using KProcess.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KProcess.Ksmed.Presentation.Core
{
    /// <summary>
    /// Représente une ViewModel de base pouvant être affiché dans la frame principale de l'application.
    /// </summary>
    public class FrameContentViewModelBase : KsmedViewModelBase, IFrameContentViewModel
    {
        private Notification _validationNotification;
        private bool _isShowingValidationErrors;

        /// <summary>
        /// Appelé lorsque la navigation souhaite quitter ce VM.
        /// </summary>
        /// <param name="token">Représente le jeton de navigation permettant d'effectuer une navigation asynchrone</param>
        /// <returns><c>true</c> si la navigation est acceptée.</returns>
        public virtual Task<bool> OnNavigatingAway(IFrameNavigationToken token)
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// Rafraichît l'affichage des erreurs de validation pour l'objet spécifié.
        /// </summary>
        /// <param name="model">Le modèle.</param>
        protected void RefreshValidationErrors(ValidatableObject model)
        {
            if (!model.IsValid.GetValueOrDefault())
                ShowValidationErrors(GetErrorMessage(model));
            else
                HideValidationErrors();
        }

        /// <summary>
        /// Rafraichît l'affichage des erreurs de validation pour les objets spécifiés.
        /// </summary>
        /// <param name="models">Les modèles.</param>
        protected void RefreshValidationErrors(IEnumerable<ValidatableObject> models)
        {
            string[] messages = models
                .Where(m => !m.IsValid.GetValueOrDefault())
                .Select(m => GetErrorMessage(m))
                .ToArray();

            if (messages.Any())
                ShowValidationErrors(string.Join("\n", messages.Distinct()));
            else
                HideValidationErrors();
        }

        /// <summary>
        /// Appelé lorsque la refraichissement des erreurs de validation est demandé.
        /// Dans une méthode dérivée, appeler RefreshValidationErrors.
        /// </summary>
        protected virtual void OnRefreshValidationErrorsRequested()
        {
        }

        /// <summary>
        /// Affiche une erreur de validation.
        /// </summary>
        /// <param name="message">Les message à afficher.</param>
        protected void ShowValidationErrors(string message)
        {
            if (_validationNotification == null)
            {
                _validationNotification = new Notification()
                {
                    Title = LocalizationManager.GetString("Common_ValidationErrors"),
                };
                _validationNotification.UpdateRequested += new EventHandler(ValidationNotification_UpdateRequested);
            }

            _validationNotification.Content = message;

            _isShowingValidationErrors = true;
            base.ServiceBus.Get<INotificationService>().Notify(_validationNotification);
            _isShowingValidationErrors = false;
        }

        /// <summary>
        /// Gère l'évènement UpdateRequested de la notification de validation.
        /// </summary>
        /// <param name="sender">La source de l'évènement.</param>
        /// <param name="e">Les <see cref="System.EventArgs"/> contenant les données de l'évènement.</param>
        private void ValidationNotification_UpdateRequested(object sender, EventArgs e)
        {
            if (!_isShowingValidationErrors)
                this.OnRefreshValidationErrorsRequested();
        }

        /// <summary>
        /// Cache une éventuelle erreur de validation précédente.
        /// </summary>
        protected void HideValidationErrors()
        {
            if (_validationNotification != null)
            {
                _validationNotification.UpdateRequested -= new EventHandler(ValidationNotification_UpdateRequested);
                base.ServiceBus.Get<INotificationService>().DeleteNotification(_validationNotification);
                _validationNotification = null;
            }
        }

        /// <summary>
        /// Obtient le message d'erreur de validation à partir d'un modèle.
        /// </summary>
        /// <param name="model">Le modèle.</param>
        /// <returns>Le message d'erreur.</returns>
        private string GetErrorMessage(ValidatableObject model)
        {
            return string.Join("\n", model.Errors.Select(e => e.Message).Distinct());
        }

        /// <summary>
        /// Appelé afin de nettoyer les ressources.
        /// </summary>
        protected override void OnCleanup()
        {
            base.OnCleanup();
            HideValidationErrors();
        }

        /// <summary>
        /// Obtient une valeur indiquant si le sélectionneur de scénario est à afficher.
        /// </summary>
        public virtual bool ShowScenarioPicker { get { return false; } }

        /// <summary>
        /// Obtient le filtre des natures de scénarios à activer.
        /// </summary>
        public virtual string[] ScenarioNaturesFilter { get { return null; } }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si sélectionneur de scénario est activé.
        /// </summary>
        public bool IsScenarioPickerEnabled
        {
            get { return base.ServiceBus.Get<IProjectManagerService>().IsScenarioPickerEnabled; }
            set { base.ServiceBus.Get<IProjectManagerService>().IsScenarioPickerEnabled = value; }
        }

    }
}
