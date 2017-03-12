using System.Collections.Generic;
using Leak.Common;

namespace Leak.Data.Store
{
    public class RepositoryAllocation
    {
        private readonly int size;
        private readonly List<RepositoryAllocationRange> items;

        public RepositoryAllocation(int size)
        {
            this.size = size;
            items = new List<RepositoryAllocationRange>();
        }

        public void Add(MetainfoEntry entry, RepositoryAllocationRange range)
        {
            items.Add(range);
        }

        public Bitfield ToBitfield()
        {
            Bitfield bitfield = new Bitfield(size);

            foreach (RepositoryAllocationRange range in items)
            {
                for (int i = range.From; i <= range.To; i++)
                {
                    bitfield[i] = true;
                }
            }

            return bitfield;
        }
    }
}