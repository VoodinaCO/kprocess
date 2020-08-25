using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KProcess.Globalization;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Presentation.Core;

namespace KProcess.Ksmed.Presentation.ViewModels
{
    public static class ResourcesHelper
    {
        /// <summary>
        /// Crée les types de ressources.
        /// </summary>
        /// <param name="onChecked">Délégué appelé lorsque la ressource a été cochée.</param>
        /// <returns></returns>
        public static ResourceType[] CreateResourceTypes(Action<ResourceType> onChecked = null)
        {
            return new ResourceType[]
            {
                new ResourceType(onChecked) 
                {
                    Label = ReferentialsUse.Operator, 
                    Type = typeof(Operator),
                    ProjectType = typeof(Operator),
                },
                new ResourceType(onChecked) 
                {
                    Label = ReferentialsUse.Equipment, 
                    Type = typeof(Equipment),
                    ProjectType = typeof(Equipment),
                },
            };
        }
    }
}
