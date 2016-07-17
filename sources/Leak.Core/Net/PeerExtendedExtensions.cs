using System;
using Leak.Core.Bencoding;

namespace Leak.Core.Net
{
    public static class PeerExtendedExtensions
    {
        public static void Bencoded(this PeerExtendedConfiguration configuration, BencodedValue value)
        {
            configuration.Content = Bencoder.Encode(value);
        }

        public static void Bencoded(this PeerExtendedConfiguration configuration, BencodedValue value, byte[] data)
        {
            byte[] bencoded = Bencoder.Encode(value);
            int length = bencoded.Length;

            Array.Resize(ref bencoded, length + data.Length);
            Array.Copy(data, 0, bencoded, length, data.Length);

            configuration.Content = bencoded;
        }

        public static void Handshake(this PeerExtendedConfiguration configuration, PeerExtendedMapping mapping)
        {
            configuration.Id = 0;
            configuration.Bencoded(mapping.Encode());
        }

        public static BencodedValue Decode(this PeerExtended message)
        {
            return Bencoder.Decode(message.Content);
        }

        public static void Handle(this PeerExtended message, PeerChannel channel, Action<PeerExtendedCallback> configurer)
        {
            PeerExtendedCallback callback = new PeerExtendedCallback();
            configurer.Invoke(callback);

            if (message.Id == 0)
            {
                callback.OnHandshake.Invoke(channel, new PeerExtendedMapping(with => with.Decode(message.Decode())));
            }

            if (message.Id > 0)
            {
                string name = callback.Mapping.FindName(message.Id);
                Action<PeerChannel, PeerExtended> handler;

                callback.OnMessage.TryGetValue(name, out handler);
                handler?.Invoke(channel, message);
            }
        }
    }
}