using Leak.Core.Listener;
using System;

namespace Leak.Core.Client
{
    public class PeerClientListenerBuilder
    {
        private PeerClientListenerConfiguration configuration;

        public PeerClientListenerStatus Status
        {
            get
            {
                if (configuration == null)
                    return PeerClientListenerStatus.Off;

                return PeerClientListenerStatus.On;
            }
        }

        public void Disable()
        {
        }

        public void Enable(Action<PeerClientListenerConfiguration> configurer)
        {
            configuration = configurer.Configure(with =>
            {
                with.Port = 8080;
            });
        }

        public PeerListener Build(Action<PeerListenerConfiguration> configurer)
        {
            return new PeerListener(with =>
            {
                with.Port = configuration.Port;
                configurer.Invoke(with);
            });
        }
    }
}