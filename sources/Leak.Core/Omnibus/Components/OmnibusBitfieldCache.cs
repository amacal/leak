using Leak.Core.Common;

namespace Leak.Core.Omnibus.Components
{
    public class OmnibusBitfieldCache
    {
        private readonly int size;
        private readonly Bitfield bitfield;

        private readonly int[] ranking;
        private readonly int[] included;
        private readonly int[] excluded;

        public OmnibusBitfieldCache(int size)
        {
            this.size = size;
            this.bitfield = new Bitfield(size);

            this.ranking = new int[size];
            this.included = new int[size];
            this.excluded = new int[size];
        }

        public int Size
        {
            get { return size; }
        }

        public Bitfield Bitfield
        {
            get { return bitfield; }
        }

        public int[] Ranking
        {
            get { return ranking; }
        }

        public int[] Included
        {
            get { return included; }
        }

        public int[] Excluded
        {
            get { return excluded; }
        }
    }
}