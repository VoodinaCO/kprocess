﻿//------------------------------------------------------------------------------
//*                                    Creation                            
//*************************************************************************
//* Fichier  : BLLTechException.cs
//* Auteur   : Luc Chany
//* Creation : 
//* Role     : Classe des exceptions techniques pour la couche Business
//*************************************************************************
//*                                    Modifications              
//*************************************************************************
//*     Auteur            Date            Objet de la modification        
//*************************************************************************
//*
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KProcess
{
  /// <summary>
  /// Classe des exceptions techniques pour la couche Business
  /// </summary>
  [Serializable]
  public class BLLTechException : BLLException
  {
    #region Constructeurs

    /// <summary>
    /// Constructeur permettant d'assigner un message
    /// </summary>
    /// <param name="pMessage">Message de l'exception</param>
    public BLLTechException(string pMessage)
      : base(pMessage)
    {
    }

    /// <summary>
    /// Constructeur permettant d'assigner un message avec un string.Format
    /// </summary>
    /// <param name="pFormat">Format du message</param>
    /// <param name="pArgs">Argument de formattage du message</param>
    public BLLTechException(string pFormat, params object[] pArgs)
      : base(String.Format(pFormat, pArgs))
    {
    }

    /// <summary>
    /// Constructeur permettant d'assigner un message et d'emballer une exception
    /// </summary>
    /// <param name="pMessage">Message de l'exception</param>
    /// <param name="pExceptionInterne">Exception emballée</param>
    public BLLTechException(string pMessage, Exception pExceptionInterne)
      : base(pMessage, pExceptionInterne)
    {
    }

    /// <summary>
    /// Constructeur permettant d'assigner un message avec un string.Format et d'emballer une exception
    /// </summary>
    /// <param name="pExceptionInterne">Exception à emballer</param>
    /// <param name="pFormat">Format du message</param>
    /// <param name="pArgs">Argument de formattage du message</param>
    public BLLTechException(Exception pExceptionInterne, string pFormat, params object[] pArgs)
      : base(String.Format(pFormat, pArgs), pExceptionInterne)
    {
    }

    /// <summary>
    /// Constructeur de désérialisation
    /// </summary>
    /// <param name="pInfo">Information de sérialisation</param>
    /// <param name="pContexte">Contexte de sérialisation</param>
    public BLLTechException(System.Runtime.Serialization.SerializationInfo pInfo, System.Runtime.Serialization.StreamingContext pContexte)
      : base(pInfo, pContexte)
    {
    }

    #endregion
  }
}
