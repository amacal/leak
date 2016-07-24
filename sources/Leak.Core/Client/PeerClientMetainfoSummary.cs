using Leak.Core.Messages;

namespace Leak.Core.Client
{
    public class PeerClientMetainfoSummary
    {
        private readonly Bitfield bitfield;

        public PeerClientMetainfoSummary(Bitfield bitfield)
        {
            this.bitfield = bitfield;
        }

        public int Completed
        {
            get { return bitfield.Completed; }
        }

        public int Total
        {
            get { return bitfield.Length; }
        }
    }
}