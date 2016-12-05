using System.Collections.Generic;
using Leak.Common;

namespace Leak.Core.Omnibus.Components
{
    public class OmnibusCache
    {
        private readonly int size;
        private readonly Bitfield bitfield;
        private readonly List<OmnibusBlock> blocks;

        private readonly int[] ranking;
        private readonly int[] included;
        private readonly int[] excluded;

        public OmnibusCache(int size)
        {
            this.size = size;
            this.bitfield = new Bitfield(size);
            this.blocks = new List<OmnibusBlock>(size);

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

        public List<OmnibusBlock> Blocks
        {
            get { return blocks; }
        }
    }
}