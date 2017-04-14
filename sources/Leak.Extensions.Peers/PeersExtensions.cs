using Leak.Bencoding;
using Leak.Common;
using System;
using Leak.Networking.Core;
using Leak.Peer.Coordinator;

namespace Leak.Extensions.Peers
{
    public static class PeersExtensions
    {
        public static void CallPeersDataReceived(this PeersHooks hooks, FileHash hash, PeerHash peer, NetworkAddress[] remotes)
        {
            hooks.OnPeersDataReceived?.Invoke(new PeersReceived
            {
                Hash = hash,
                Peer = peer,
                Remotes = remotes
            });
        }

        public static void CallPeersDataSent(this PeersHooks hooks, FileHash hash, PeerHash peer, NetworkAddress[] remotes)
        {
            hooks.OnPeersDataSent?.Invoke(new PeersReceived
            {
                Hash = hash,
                Peer = peer,
                Remotes = remotes
            });
        }

        public static void SendPeers(this CoordinatorService glue, PeerHash peer, params NetworkAddress[] remotes)
        {
            BencodedValue bencoded = new BencodedValue
            {
                Dictionary = new[]
                {
                    new BencodedEntry
                    {
                        Key = new BencodedValue { Text = new BencodedText("added") },
                        Value = new BencodedValue { Data = ToData(remotes) }
                    }
                }
            };

            glue.SendExtension(peer, PeersPlugin.Name, Bencoder.Encode(bencoded));
        }

        private static BencodedData ToData(NetworkAddress[] remotes)
        {
            byte[] data = new byte[remotes.Length * 6];

            for (int i = 0; i < remotes.Length; i++)
            {
                int port = remotes[i].Port;
                string[] parts = remotes[i].Host.Split('.');

                for (int j = 0; j < 4; j++)
                {
                    data[i * 6 + j] = Byte.Parse(parts[j]);
                }

                data[i * 6 + 4] = (byte)(port / 256);
                data[i * 6 + 5] = (byte)(port % 256);
            }

            return new BencodedData(data);
        }
    }
}