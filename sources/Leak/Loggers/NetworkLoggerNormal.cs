using Leak.Core.Client;
using Leak.Core.Common;
using System;

namespace Leak.Loggers
{
    public class NetworkLoggerNormal : NetworkLogger
    {
        public override void OnPeerConnecting(FileHash hash, PeerAddress peer)
        {
            Console.WriteLine($"{hash}: connecting; endpoint={peer}");
        }

        public override void OnPeerConnected(FileHash hash, PeerClientConnected connected)
        {
            Console.WriteLine($"{hash}: connected; endpoint={connected.Peer}; total={connected.Total}");
        }

        public override void OnPeerDisconnected(FileHash hash, PeerHash peer)
        {
            Console.WriteLine($"{peer}: disconnected");
        }
    }
}