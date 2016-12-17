using System;
using Leak.Events;

namespace Leak.Glue
{
    public class GlueHooks
    {
        public Action<PeerConnected> OnPeerConnected;
        public Action<PeerDisconnected> OnPeerDisconnected;

        public Action<PeerChanged> OnPeerChanged;
        public Action<BlockReceived> OnBlockReceived;

        public Action<ExtensionListReceived> OnExtensionListReceived;
        public Action<ExtensionListSent> OnExtensionListSent;

        public Action<ExtensionDataReceived> OnExtensionDataReceived;
        public Action<ExtensionDataSent> OnExtensionDataSent;
    }
}