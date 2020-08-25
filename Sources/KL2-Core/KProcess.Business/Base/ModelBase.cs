//------------------------------------------------------------------------------
//*                                    Creation                            
//*************************************************************************
//* Fichier  : DtoBase.cs
//* Auteur   : Tekigo
//* Creation : 
//* Role     : Définit la classe de base des entités métier.
//*************************************************************************
//*                                    Modifications              
//*************************************************************************
//*     Auteur            Date            Objet de la modification        
//*************************************************************************
//*
//------------------------------------------------------------------------------

using System;
using System.IO;
using System.Runtime.Serialization;

namespace KProcess.Business
{
    /// <summary>
    /// Définit la classe de base des entités métier.
    /// </summary>
    [DataContract(IsReference = true)]
    [Serializable]
    public abstract class ModelBase : ValidatableObject, IModel
    {

    }
}