using Leak.Bencoding;
using Leak.Common;
using System;
using System.Collections.Generic;
using System.Text;
using Leak.Networking.Core;

namespace Leak.Extensions.Peers
{
    public class PeersHandler : MoreHandler
    {
        private readonly PeersHooks hooks;

        public PeersHandler(PeersHooks hooks)
        {
            this.hooks = hooks;
        }

        public void OnHandshake(FileHash hash, PeerHash peer, byte[] payload)
        {
        }

        public void OnMessageReceived(FileHash hash, PeerHash peer, byte[] payload)
        {
            Handle(hash, peer, payload, hooks.CallPeersDataReceived);
        }

        public void OnMessageSent(FileHash hash, PeerHash peer, byte[] payload)
        {
            Handle(hash, peer, payload, hooks.CallPeersDataSent);
        }

        private static void Handle(FileHash hash, PeerHash peer, byte[] payload, Action<FileHash, PeerHash, NetworkAddress[]> callback)
        {
            BencodedValue value = Bencoder.Decode(payload);
            byte[] added = value.Find("added", x => x?.Data?.GetBytes());
            List<NetworkAddress> peers = new List<NetworkAddress>();

            if (added != null)
            {
                for (int i = 0; i < added.Length; i += 6)
                {
                    string host = GetHost(added, i);
                    int port = GetPort(added, i);

                    if (port > 0)
                    {
                        peers.Add(new NetworkAddress(host, port));
                    }
                }
            }

            if (added?.Length > 0)
            {
                callback(hash, peer, peers.ToArray());
            }
        }

        private static string GetHost(byte[] data, int offset)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append(data[0 + offset]);
            builder.Append('.');
            builder.Append(data[1 + offset]);
            builder.Append('.');
            builder.Append(data[2 + offset]);
            builder.Append('.');
            builder.Append(data[3 + offset]);

            return builder.ToString();
        }

        private static int GetPort(byte[] data, int offset)
        {
            return data[4 + offset] * 256 + data[5 + offset];
        }
    }
}