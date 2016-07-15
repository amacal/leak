using Leak.Core.Network;
using System;

namespace Leak.Core.Net
{
    public class PeerExtended : PeerMessageFactory
    {
        private readonly PeerExtendedConfiguration configuration;

        public PeerExtended(Action<PeerExtendedConfiguration> configurer)
        {
            configuration = new PeerExtendedConfiguration();
            configurer.Invoke(configuration);
        }

        public PeerExtended(NetworkIncomingMessage message)
        {
            configuration = new PeerExtendedConfiguration
            {
                Id = message[5],
                Content = message.ToBytes(6)
            };
        }

        public byte Id
        {
            get { return configuration.Id; }
        }

        public byte[] Content
        {
            get { return configuration.Content; }
        }

        public int Length
        {
            get { return configuration.Content.Length; }
        }

        public override NetworkOutgoingMessageBytes GetMessage()
        {
            int length = configuration.Content.Length + 2;
            byte[] data = new byte[4 + length];

            data[0] = (byte)((length / 256 / 256 / 256) % 256);
            data[1] = (byte)((length / 256 / 256) % 256);
            data[2] = (byte)((length / 256) % 256);
            data[3] = (byte)(length % 256);
            data[4] = 20;
            data[5] = configuration.Id;

            Array.Copy(configuration.Content, 0, data, 6, configuration.Content.Length);

            return new NetworkOutgoingMessageBytes(data);
        }
    }
}