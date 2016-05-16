using System;

namespace Leak.Core.Net
{
    public class PeerAnnounce
    {
        private readonly PeerHandshake handshake;
        private readonly PeerAnnounceConfiguration configuration;

        public PeerAnnounce(PeerHandshake handshake)
        {
            this.handshake = handshake;
            this.configuration = GetDefaultConfiguration();
        }

        public PeerAnnounce(PeerHandshake handshake, Action<PeerAnnounceConfigurator> with)
        {
            PeerAnnounceConfiguration configuration = GetDefaultConfiguration();
            PeerAnnounceConfigurator configurator = new PeerAnnounceConfigurator();

            with.Invoke(configurator);
            configurator.Apply(configuration);

            this.handshake = handshake;
            this.configuration = configuration;
        }

        private static PeerAnnounceConfiguration GetDefaultConfiguration()
        {
            return new PeerAnnounceConfiguration
            {
                Port = 8080
            };
        }

        public PeerHandshake Handshake
        {
            get { return handshake; }
        }

        public byte[] Address
        {
            get { return configuration.Address; }
        }

        public int? Port
        {
            get { return configuration.Port; }
        }
    }
}