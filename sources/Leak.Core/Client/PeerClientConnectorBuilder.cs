using Leak.Core.Connector;
using System;

namespace Leak.Core.Client
{
    public class PeerClientConnectorBuilder
    {
        private PeerClientListenerStatus status;

        public PeerClientConnectorBuilder()
        {
            status = PeerClientListenerStatus.Off;
        }

        public PeerClientListenerStatus Status
        {
            get { return status; }
        }

        public void Disable()
        {
            status = PeerClientListenerStatus.Off;
        }

        public void Enable()
        {
            status = PeerClientListenerStatus.On;
        }

        public PeerConnector Build(Action<PeerConnectorConfiguration> configurer)
        {
            return new PeerConnector(with =>
            {
                configurer.Invoke(with);
            });
        }
    }
}