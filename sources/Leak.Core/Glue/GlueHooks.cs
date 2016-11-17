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

        public Action<ExtensionListReceived> OnExtensionListReceived;
        public Action<ExtensionListSent> OnExtensionListSent;
    }
}