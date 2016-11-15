using Leak.Core.Events;
using System;

namespace Leak.Core.Loop
{
    public class ConnectionLoopHooks
    {
        public Action<MessageReceived> OnMessageReceived;

        public Action<PeerKeepAliveMessageReceived> OnPeerKeepAliveMessageReceived;
        public Action<PeerChokeMessageReceived> OnPeerChokeMessageReceived;
        public Action<PeerUnchokeMessageReceived> OnPeerUnchokeMessageReceived;
        public Action<PeerInterestedMessageReceived> OnPeerInterestedMessageReceived;
        public Action<PeerHaveMessageReceived> OnPeerHaveMessageReceived;
        public Action<PeerBitfieldMessageReceived> OnPeerBitfieldMessageReceived;
        public Action<PeerPieceMessageReceived> OnPeerPieceMessageReceived;
        public Action<PeerExtendedMessageReceived> OnPeerExtendedMessageReceived;
    }
}