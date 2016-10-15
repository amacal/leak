using Leak.Core.Common;
using Leak.Core.Core;

namespace Leak.Core.Omnibus.Tasks
{
    public class AddBitfieldTask : LeakTask<OmnibusContext>
    {
        private readonly PeerHash peer;
        private readonly Bitfield bitfield;

        public AddBitfieldTask(PeerHash peer, Bitfield bitfield)
        {
            this.peer = peer;
            this.bitfield = bitfield;
        }

        public void Execute(OmnibusContext context)
        {
            context.Bitfields.Add(peer, bitfield);
        }
    }
}