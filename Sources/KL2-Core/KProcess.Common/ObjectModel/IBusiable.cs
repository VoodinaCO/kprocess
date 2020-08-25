//------------------------------------------------------------------------------
//*                                    Creation                            
//*************************************************************************
//* Fichier  : IBusiable.cs
//* Auteur   : Tekigo
//* Creation : 
//* Role     : Définit le comportement d'un objet gérant l'état "Occupé" par incréments.
//*************************************************************************
//*                                    Modifications              
//*************************************************************************
//*     Auteur            Date            Objet de la modification        
//*************************************************************************
//*   
//------------------------------------------------------------------------------
namespace KProcess
{
    /// <summary>
    /// Définit le comportement d'un objet gérant l'état "Occupé" par incréments.
    /// </summary>
    public interface IBusiable
    {
        /// <summary>
        /// Incrémente le compteur.
        /// </summary>
        void IncrementBusyCount();

        /// <summary>
        /// Décrémente le compteur.
        /// </summary>
        void DecrementBusyCount();

        /// <summary>
        /// Obtient une valeur indiquant si l'instance est occupée.
        /// </summary>
        bool IsBusy { get; }
    }
}
