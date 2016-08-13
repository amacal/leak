using Leak.Core.Common;

namespace Leak.Core.Cando
{
    public class CandoEntry
    {
        private readonly PeerHash peer;

        public CandoEntry(PeerHash peer)
        {
            this.peer = peer;

            Local = new CandoMap();
            Remote = new CandoMap();
            Handlers = new CandoHandler[0];
        }

        public PeerHash Peer
        {
            get { return peer; }
        }

        public bool HasExtensions { get; set; }

        public bool KnowsRemoteExtensions { get; set; }

        public bool KnowsLocalExtensions { get; set; }

        public PeerDirection Direction { get; set; }

        public CandoMap Local { get; set; }

        public CandoMap Remote { get; set; }

        public CandoHandler[] Handlers { get; set; }
    }
}