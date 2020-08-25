using KProcess.Ksmed.Business.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KProcess.Ksmed.Business
{
    /// <summary>
    /// Décrit le comportement d'un service fournissant des informatiosn sur le système.
    /// </summary>
    public interface ISystemInformationService : IService
    {
        /// <summary>
        /// Obtient des informations basiques sur le système.
        /// </summary>
        /// <returns>Les informations.</returns>
        SystemBasicInformation GetBasicInformation();
    }
}
