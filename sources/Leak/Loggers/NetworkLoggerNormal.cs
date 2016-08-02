using Leak.Core.Common;
using System;

namespace Leak.Loggers
{
    public class NetworkLoggerNormal : NetworkLogger
    {
        public override void OnPeerConnecting(FileHash hash, string endpoint)
        {
            Console.WriteLine($"{hash}: connecting; endpoint={endpoint}");
        }

        public override void OnPeerConnected(FileHash hash, string endpoint)
        {
            Console.WriteLine($"{hash}: connected; endpoint={endpoint}");
        }

        public override void OnPeerDisconnected(FileHash hash, PeerHash peer)
        {
            Console.WriteLine($"{peer}: disconnected");
        }
    }
}