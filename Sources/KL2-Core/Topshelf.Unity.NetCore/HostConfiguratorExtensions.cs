﻿using Microsoft.Practices.Unity;
using Topshelf.HostConfigurators;

namespace Topshelf.Unity.NetCore
{
    public static class HostConfiguratorExtensions
    {
        public static HostConfigurator UseUnityContainer(
            this HostConfigurator configurator,
            IUnityContainer unityContainer)
        {
            configurator.AddConfigurator(new UnityHostBuilderConfigurator(unityContainer));
            return configurator;
        }
    }
}
