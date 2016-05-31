using System;

namespace Leak.Core.Net
{
    public class PeerAnnounce
    {
        private readonly PeerAnnounceConfiguration configuration;

        public PeerAnnounce(Action<PeerAnnounceConfiguration> configurer)
        {
            configuration = new PeerAnnounceConfiguration
            {
                Port = 8080
            };

            configurer.Invoke(configuration);
        }

        public byte[] Hash
        {
            get { return configuration.Hash; }
        }

        public byte[] Peer
        {
            get { return configuration.Peer; }
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