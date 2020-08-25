using System.Collections.Generic;
using System.Windows.Input;

namespace KProcess.Presentation.Windows.Localization
{
    public static class Shortcuts
    {
        /// <summary>
        /// Sauvegarder
        /// </summary>
        public const Key Save = Key.F5;

        /// <summary>
        /// Ajouter
        /// </summary>
        public const Key Add = Key.F8;

        /// <summary>
        /// Supprimer
        /// </summary>
        public const Key Delete = Key.Delete;

        /// <summary>
        /// Annuler
        /// </summary>
        public const Key Cancel = Key.F9;

        /// <summary>
        /// Play/Pause sur le player
        /// </summary>
        public const Key PlayerPlayPlause = Key.F3;

        /// <summary>
        /// Pas en arrière sur le player
        /// </summary>
        public const Key PlayerStepBackward = Key.F2;

        /// <summary>
        /// Pas en avant sur le player
        /// </summary>
        public const Key PlayerStepForward = Key.F4;

        /// <summary>
        /// Bascule du mode de visionnage sur le player
        /// </summary>
        public const Key PlayerToggleScreenMode = Key.F11;
    }

    public class ShortcutsManager
    {
        static readonly Dictionary<Shortcut, Key> shorcutsDictionary = new Dictionary<Shortcut, Key>()
        {
            [Shortcut.Save] = Shortcuts.Save,
            [Shortcut.Add] = Shortcuts.Add,
            [Shortcut.Delete] = Shortcuts.Delete,
            [Shortcut.Cancel] = Shortcuts.Cancel,
            [Shortcut.PlayerPlayPlause] = Shortcuts.PlayerPlayPlause,
            [Shortcut.PlayerStepBackward] = Shortcuts.PlayerStepBackward,
            [Shortcut.PlayerStepForward] = Shortcuts.PlayerStepForward,
            [Shortcut.PlayerToggleScreenMode] = Shortcuts.PlayerToggleScreenMode
        };

        static ShortcutsManager _manager;
        public static ShortcutsManager Manager
        {
            get
            {
                if (_manager == null)
                    _manager = new ShortcutsManager();
                return _manager;
            }
        }

        public Key this[Shortcut value] =>
            shorcutsDictionary[value];
    }

    public enum Shortcut
    {
        /// <summary>
        /// Sauvegarder
        /// </summary>
        Save,

        /// <summary>
        /// Ajouter
        /// </summary>
        Add,

        /// <summary>
        /// Supprimer
        /// </summary>
        Delete,

        /// <summary>
        /// Annuler
        /// </summary>
        Cancel,

        /// <summary>
        /// Play/Pause sur le player
        /// </summary>
        PlayerPlayPlause,

        /// <summary>
        /// Pas en arrière sur le player
        /// </summary>
        PlayerStepBackward,

        /// <summary>
        /// Pas en avant sur le player
        /// </summary>
        PlayerStepForward,

        /// <summary>
        /// Bascule du mode de visionnage sur le player
        /// </summary>
        PlayerToggleScreenMode
    }
}
