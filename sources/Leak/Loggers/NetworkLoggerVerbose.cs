using Leak.Core.Client;
using Leak.Core.Common;
using System;

namespace Leak.Loggers
{
    public class NetworkLoggerVerbose : NetworkLoggerNormal
    {
        public override void OnPeerIncomingMessage(FileHash hash, PeerHash peer, PeerClientMessage message)
        {
            Console.WriteLine($"{peer}: incoming-message; type={message.Type}; size={message.Size}");
        }

        public override void OnPeerOutgoingMessage(FileHash hash, PeerHash peer, PeerClientMessage message)
        {
            Console.WriteLine($"{peer}: outgoing-message; type={message.Type}; size={message.Size}");
        }
    }
}