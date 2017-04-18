using System;
using Leak.Peer.Coordinator.Events;

namespace Leak.Peer.Coordinator
{
    public class CoordinatorHooks
    {
        public Action<PeerConnected> OnPeerConnected;
        public Action<PeerDisconnected> OnPeerDisconnected;

        public Action<BitfieldChanged> OnBitfieldChanged;
        public Action<StatusChanged> OnStatusChanged;

        public Action<BlockReceived> OnBlockReceived;
        public Action<BlockRequested> OnBlockRequested;

        public Action<KeepAliveRequested> OnKeepAliveRequested;
        public Action<MessageRequested> OnMessageRequested;

        public Action<ExtensionListReceived> OnExtensionListReceived;
        public Action<ExtensionListSent> OnExtensionListSent;

        public Action<ExtensionDataReceived> OnExtensionDataReceived;
        public Action<ExtensionDataSent> OnExtensionDataSent;
    }
}