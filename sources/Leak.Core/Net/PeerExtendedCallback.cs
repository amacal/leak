using System;
using System.Collections.Generic;

namespace Leak.Core.Net
{
    public class PeerExtendedCallback
    {
        public PeerExtendedCallback()
        {
            OnMessage = new Dictionary<string, Action<PeerChannel, PeerExtended>>();
        }

        public PeerExtendedMapping Mapping { get; set; }

        public Action<PeerChannel, PeerExtendedMapping> OnHandshake { get; set; }

        public Dictionary<string, Action<PeerChannel, PeerExtended>> OnMessage { get; set; }
    }
}