using System;
using Leak.Common;
using Leak.Data.Map;
using Leak.Data.Share;

namespace Leak.Client.Adapters
{
    public class DataShareToDataMapAdapter : DataShareToDataMap
    {
        private readonly OmnibusService service;

        public DataShareToDataMapAdapter(OmnibusService service)
        {
            this.service = service;
        }

        public void Query(Action<PeerHash, Bitfield, PeerState> callback)
        {
            service.Query(callback);
        }
    }
}