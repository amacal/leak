using Leak.Core.Events;
using System;

namespace Leak.Core.Glue
{
    public class GlueHooks
    {
        public Action<PeerConnected> OnPeerConnected;
        public Action<PeerDisconnected> OnPeerDisconnected;

        public Action<PeerStateChanged> OnPeerStateChanged;
        public Action<PeerBitfieldChanged> OnPeerBitfieldChanged;

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