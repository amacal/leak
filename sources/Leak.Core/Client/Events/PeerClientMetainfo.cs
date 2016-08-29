using Leak.Core.Messages;

namespace Leak.Core.Client.Events
{
    public class PeerClientMetainfo
    {
        private readonly Bitfield bitfield;

        public PeerClientMetainfo(Bitfield bitfield)
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