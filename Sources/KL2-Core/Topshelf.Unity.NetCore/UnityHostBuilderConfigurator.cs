using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using Topshelf.Builders;
using Topshelf.Configurators;
using Topshelf.HostConfigurators;

namespace Topshelf.Unity.NetCore
{
    public class UnityHostBuilderConfigurator : HostBuilderConfigurator
    {
        public UnityHostBuilderConfigurator(IUnityContainer unityContainer)
        {
            UnityContainer = unityContainer ?? throw new ArgumentNullException("unityContainer");
        }

        public static IUnityContainer UnityContainer { get; private set; }

        public HostBuilder Configure(HostBuilder builder)
        {
            return builder;
        }

        public IEnumerable<ValidateResult> Validate()
        {
            yield break;
        }
    }
}
