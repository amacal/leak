using Leak.Common;
using System.Collections.Generic;

namespace Leak.Omnibus.Components
{
    public class OmnibusReservationComparer : IEqualityComparer<BlockIndex>
    {
        public int GetHashCode(BlockIndex obj)
        {
            return BlockIndex.GetHashCode(obj);
        }

        public bool Equals(BlockIndex x, BlockIndex y)
        {
            return BlockIndex.Equals(x, y);
        }
    }
}