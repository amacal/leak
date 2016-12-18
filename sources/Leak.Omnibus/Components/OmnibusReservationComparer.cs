using System.Collections.Generic;

namespace Leak.Omnibus.Components
{
    public class OmnibusReservationComparer : IEqualityComparer<OmnibusBlock>
    {
        public int GetHashCode(OmnibusBlock obj)
        {
            return OmnibusBlock.GetHashCode(obj);
        }

        public bool Equals(OmnibusBlock x, OmnibusBlock y)
        {
            return OmnibusBlock.Equals(x, y);
        }
    }
}