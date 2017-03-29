using System;
using Leak.Common;

namespace Leak.Data.Share
{
    public interface DataShareToDataMap
    {
        void Query(Action<PeerHash, Bitfield, PeerState> callback);
    }
}