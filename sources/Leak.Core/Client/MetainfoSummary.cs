using Leak.Core.Messages;

namespace Leak.Core.Client
{
    public class MetainfoSummary
    {
        private readonly Bitfield bitfield;

        public MetainfoSummary(Bitfield bitfield)
        {
            this.bitfield = bitfield;
        }

        public int Completed
        {
            get { return bitfield.Completed; }
        }
    }
}