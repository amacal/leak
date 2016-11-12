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

        public override void OnPeerDisconnected(FileHash hash, PeerHash peer)
        {
            Console.WriteLine($"{peer}: disconnected");
        }
    }
}