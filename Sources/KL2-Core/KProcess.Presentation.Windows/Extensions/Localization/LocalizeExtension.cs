using KProcess.Globalization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace KProcess.Presentation.Windows.Localization
{
    /// <summary>
    /// Markup permettant de renvoyer une resource à partir d'une clé
    /// </summary>
    [MarkupExtensionReturnType(typeof(string)), Localizability(LocalizationCategory.NeverLocalize)]
    public class LocalizeExtension : MarkupExtension, IDependencyLocalizeExtension
    {
        /// <summary>
        /// Le format d'une valeur avec raccourci.
        /// </summary>
        public const string ShortcutStringFormat = "{0} ({1})";

        #region Attributs

        /// <summary>
        /// The property localized by the instance.
        /// </summary>
        object _targetProperty;

        /// <summary>
        /// The instance that owns the <see cref="DependencyProperty"/> localized by the instance.
        /// </summary>
        WeakReference _targetObject;

        /// <summary>
        /// The list of instances created by a template that own the <see cref="DependencyProperty"/>
        /// localized by the instance.
        /// </summary>
        List<WeakReference> _targetObjects;

        /// <summary>
        /// Gets value indicating if the instance localized by this instance is alive.
        /// </summary>
        internal bool IsAlive
        {
            get
            {
                // Verify if the extension is used in a template

                if (_targetObjects != null)
                {
                    foreach (var item in _targetObjects)
                    {
                        if (item.IsAlive)
                        {
                            return true;
                        }
                    }

                    return false;
                }

                return _targetObject.IsAlive;
            }
        }

        #endregion

        #region Constructeurs

        /// <summary>
        /// Constructeur par défaut
        /// </summary>
        public LocalizeExtension()
        {
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="key">clé de la resource</param>
        public LocalizeExtension(string key)
            : this()
        {
            Key = key;
        }

        #endregion

        #region Propriétés

        /// <summary>
        /// Obtient ou définit la clé de la resource
        /// </summary>
        [ConstructorArgument("Key")]
        public string Key { get; set; }

        /// <summary>
        /// Obtient ou définit la clé de la resource
        /// </summary>
        [ConstructorArgument("Shortcut")]
        public Shortcut? Shortcut { get; set; }

        /// <summary>
        /// Obtient ou définit la chaîne de formatage de la resource
        /// </summary>
        [ConstructorArgument("StringFormat")]
        public string StringFormat { get; set; }

        /// <summary>
        /// Obtient ou définit la valeur par défaut si la resource n'est pas trouvée
        /// </summary>
        [ConstructorArgument("DefaultValue")]
        public object DefaultValue { get; set; }

        /// <summary>
        /// Obtient ou définit le value converter à utiliser
        /// </summary>
        [ConstructorArgument("Converter")]
        public IValueConverter Converter { get; set; }

        /// <summary>
        /// Obtient ou définit le fournisseur à utiliser
        /// </summary>
        [ConstructorArgument("ProviderKey")]
        public string ProviderKey { get; set; }

        /// <summary>
        /// Obtient le convertisseur de type.
        /// </summary>
        public TypeConverter TypeConverter { get; private set; }

        #endregion

        #region Gestion des événements

        #endregion

        #region Surcharges

        /// <summary>
        /// Retourne la valeur localisée
        /// </summary>
        /// <param name="serviceProvider">objet fournissant des services au markup</param>
        /// <returns>
        /// la valeur localisée
        /// </returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (serviceProvider.GetService(typeof(IProvideValueTarget)) is IProvideValueTarget service)
            {
                if (service.TargetProperty is DependencyProperty)
                {
                    _targetProperty = service.TargetProperty;
                    if (service.TargetObject is DependencyObject)
                    {
                        var targetObject = new WeakReference(service.TargetObject);

                        // Verify if the extension is used in a template
                        // and has been already registered
                        if (_targetObjects != null)
                            _targetObjects.Add(targetObject);
                        else
                        {
                            _targetObject = targetObject;
                            LocalizationManagerExt.AddLocalization(this);
                        }
                    }
                    else
                    {
                        // The extension is used in a template
                        _targetObjects = new List<WeakReference>();
                        LocalizationManagerExt.AddLocalization(this);
                        return this;
                    }
                }
                else if (service.TargetProperty is PropertyInfo)
                {
                    _targetProperty = service.TargetProperty;
                    _targetObject = new WeakReference(service.TargetObject);
                    LocalizationManagerExt.AddLocalization(this);
                }
            }

            return Shortcut == null ? LocalizationManager.GetValue(Key, StringFormat)
                : string.Format(ShortcutStringFormat, LocalizationManager.GetValue(Key, StringFormat), ShortcutsManager.Manager[Shortcut.Value]);
        }

        /// <summary>
        /// Updates the value of the localized object.
        /// </summary>
        internal void UpdateTargetValue()
        {
            var targetProperty = _targetProperty;
            if (targetProperty == null)
                return;

            var value = Shortcut == null ? LocalizationManager.GetValue(Key, StringFormat)
                : string.Format(ShortcutStringFormat, LocalizationManager.GetValue(Key, StringFormat), ShortcutsManager.Manager[Shortcut.Value]);

            if (targetProperty is DependencyProperty)
            {
                if (_targetObject != null)
                {
                    var targetObject = _targetObject.Target as DependencyObject;
                    UpdateTargetObject(targetObject, targetProperty, value);
                }
                else if (_targetObjects != null)
                {
                    foreach (var item in _targetObjects)
                    {
                        var targetObject = item.Target as DependencyObject;
                        UpdateTargetObject(targetObject, targetProperty, value);
                    }
                }
            }
            else if (targetProperty is PropertyInfo)
            {
                var targetObject = _targetObject.Target;

                if(targetObject is Setter setter && setter.IsSealed)
                    return;
                if (targetObject is Binding)
                    return;

                if (targetObject != null)
                {
                    try
                    {
                        ((PropertyInfo)targetProperty).SetValue(targetObject, value, null);
                    }
                    catch (Exception ex)
                    {
                        TraceManager.TraceError(ex, ex.Message);
                    }
                }
            }
        }

        void UpdateTargetObject(DependencyObject targetObject, object targetProperty, object value)
        {
            if (targetObject == null)
                return;
            try
            {
                targetObject.SetValue((DependencyProperty)targetProperty, value);
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        #endregion
    }
}
