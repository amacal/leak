using Leak.Core.Common;
using System;

namespace Leak.Core.Responder
{
    public class ResponderEntry
    {
        private readonly PeerHash peer;

        public ResponderEntry(PeerHash peer)
        {
            this.peer = peer;
        }

        public PeerHash Peer
        {
            get { return peer; }
        }

        public ResponderChannel Channel { get; set; }

        public DateTime LastKeepAlive { get; set; }

        public DateTime NextKeepAlive { get; set; }
    }
}