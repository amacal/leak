using Leak.Core.Common;
using Leak.Core.Messages;

namespace Leak.Core.Battlefield
{
    public class BattlefieldEntry
    {
        private readonly PeerHash peer;

        public BattlefieldEntry(PeerHash peer)
        {
            this.peer = peer;
        }

        public Bitfield Bitfield { get; set; }
    }
}