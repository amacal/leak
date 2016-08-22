using Leak.Core.Common;

namespace Leak.Core.Cando
{
    public class CandoEntry
    {
        private readonly PeerSession session;

        public CandoEntry(PeerSession session)
        {
            this.session = session;

            Local = new CandoMap();
            Remote = new CandoMap();
            Handlers = new CandoHandler[0];
        }

        public bool HasExtensions { get; set; }

        public bool KnowsRemoteExtensions { get; set; }

        public bool KnowsLocalExtensions { get; set; }

        public PeerDirection Direction { get; set; }

        public CandoMap Local { get; set; }

        public CandoMap Remote { get; set; }

        public CandoHandler[] Handlers { get; set; }

        public PeerSession Session
        {
            get { return session; }
        }
    }
}