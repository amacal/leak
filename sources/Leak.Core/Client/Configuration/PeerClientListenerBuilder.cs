using Leak.Core.Listener;
using System;

namespace Leak.Core.Client.Configuration
{
    public class PeerClientListenerBuilder
    {
        private PeerClientListenerConfiguration configuration;

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

        public PeerClientListenerStatus Status
        {
            get
            {
                if (configuration == null)
                    return PeerClientListenerStatus.Off;

                return PeerClientListenerStatus.On;
            }
        }

        public int Port
        {
            get { return configuration?.Port ?? 0; }
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