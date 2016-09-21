using Leak.Core.Connector;
using System;

namespace Leak.Core.Client.Configuration
{
    public class PeerClientConnectorBuilder
    {
        private PeerClientConnectorStatus status;

        public PeerClientConnectorBuilder()
        {
            status = PeerClientConnectorStatus.Off;
        }

        public PeerClientConnectorStatus Status
        {
            get { return status; }
        }

        public void Disable()
        {
            status = PeerClientConnectorStatus.Off;
        }

        public void Enable()
        {
            status = PeerClientConnectorStatus.On;
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