using Leak.Core.Client;
using Leak.Core.Common;
using System;

namespace Leak.Loggers
{
    public class NetworkLoggerNormal : NetworkLogger
    {
        public override void OnPeerConnectingTo(FileHash hash, PeerAddress peer)
        {
            Console.WriteLine($"{hash}: connecting; endpoint={peer}; type=outgoing");
        }

        public override void OnPeerConnectingFrom(PeerHash local, PeerAddress peer)
        {
            Console.WriteLine($"{local}: connecting; endpoint={peer}; type=incoming");
        }

        public override void OnPeerConnectedTo(FileHash hash, PeerClientConnected connected)
        {
            Console.WriteLine($"{hash}: connected; endpoint={connected.Peer}; type=outgoing; total={connected.Total}");
        }

        public override void OnPeerConnectedFrom(PeerHash local, PeerClientConnected connected)
        {
            Console.WriteLine($"{local}: connected; endpoint={connected.Peer}; type=incoming; total={connected.Total}");
        }

        public override void OnPeerDisconnected(FileHash hash, PeerHash peer)
        {
            Console.WriteLine($"{peer}: disconnected");
        }
    }
}