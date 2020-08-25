using Quartz;
using System.Collections.Generic;

namespace Topshelf.Quartz
{
    public class QuartzTriggerListenerConfig
    {
        public ITriggerListener Listener { get; set; }
        public IList<IMatcher<TriggerKey>> Matchers { get; set; }
        public QuartzTriggerListenerConfig(ITriggerListener listener, params IMatcher<TriggerKey>[] matchers)
        {
            Listener = listener;
            Matchers = matchers;
        }
    }
}
