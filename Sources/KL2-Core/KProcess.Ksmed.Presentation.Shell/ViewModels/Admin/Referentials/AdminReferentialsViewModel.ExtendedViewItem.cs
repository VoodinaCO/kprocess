namespace KProcess.Ksmed.Presentation.ViewModels
{
    partial class AdminReferentialsViewModel
    {
        /// <summary>
        /// Définit une vue encapsulée
        /// </summary>
        public class ExtendedViewItem : ValidatableObject
        {
            private string _key;
            /// <summary>
            /// La clef correspondant à la vue
            /// </summary>
            public string Key {
                get
                {
                    return _key;
                }

                set
                {
                    if (_key != value)
                    {
                        _key = value;
                        OnPropertyChanged("Key");
                    }
                }
            }

            /// <summary>
            /// Détermine s'il s'agit d'une référence de type ressource
            /// </summary>
            public bool IsResource { get; set; }
        }

    }
}
