using Leak.Core.Common;

namespace Leak.Core.Battlefield
{
    public class BattlefieldEntry
    {
        private readonly PeerSession session;

        public BattlefieldEntry(PeerSession session)
        {
            this.session = session;
        }

        public PeerSession Session
        {
            get { return session; }
        }

        public Bitfield Bitfield { get; set; }
    }
}