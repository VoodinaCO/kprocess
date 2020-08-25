namespace DlhSoft.Windows.Controls
{
    using System.Collections.ObjectModel;
    using System.Windows;

    public class ScaleCollection : FreezableCollection<Scale>
    {

        /// <summary>
        /// Rend l'objet Freezable non modifiable ou vérifie si celui-ci peut être rendu non modifiable ou pas.
        /// </summary>
        /// <param name="isChecking">true pour retourner une indication de la possibilité ou non de figer l'objet (sans le figer réellement) ; false pour figer réellement l'objet.</param>
        /// <returns>Si isChecking est true, cette méthode retourne true si le Freezable peut être rendu non modifiable, ou false si cette opération est impossible.Si isChecking est false, cette méthode retourne true si le Freezable spécifié est désormais non modifiable, ou false si cette opération est impossible.</returns>
        protected override bool FreezeCore(bool isChecking)
        {
            return false;
        }

        protected override Freezable CreateInstanceCore()
        {
            return new ScaleCollection();
        }

    }
}

