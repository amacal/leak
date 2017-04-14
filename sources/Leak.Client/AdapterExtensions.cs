using Leak.Client.Adapters;
using Leak.Data.Get;
using Leak.Data.Map;
using Leak.Data.Share;
using Leak.Data.Store;
using Leak.Memory;
using Leak.Meta.Get;
using Leak.Meta.Store;
using Leak.Networking;
using Leak.Peer.Coordinator;

namespace Leak.Client
{
    public static class AdapterExtensions
    {
        public static MetagetGlue AsMetaGet(this CoordinatorService service)
        {
            return new MetaGetToGlueAdapter(service);
        }

        public static MetagetMetafile AsMetaGet(this MetafileService service)
        {
            return new MetaGetToMetaStoreAdapter(service);
        }

        public static DataGetToGlue AsDataGet(this CoordinatorService service)
        {
            return new DataGetToGlueAdapter(service);
        }

        public static DataGetToDataStore AsDataGet(this RepositoryService service)
        {
            return new DataGetToDataStoreAdapter(service);
        }

        public static DataGetToDataMap AsDataGet(this OmnibusService service)
        {
            return new DataGetToDataMapAdapter(service);
        }

        public static DataShareToDataMap AsDataShare(this OmnibusService service)
        {
            return new DataShareToDataMapAdapter(service);
        }

        public static DataShareToDataStore AsDataShare(this RepositoryService service)
        {
            return new DataShareToDataStoreAdapter(service);
        }

        public static DataShareToGlue AsDataShare(this CoordinatorService service)
        {
            return new DataShareToGlueAdapter(service);
        }

        public static NetworkPoolMemory AsNetwork(this MemoryService service)
        {
            return new MemoryToNetworkAdapter(service);
        }

        public static RepositoryMemory AsDataStore(this MemoryService service)
        {
            return new MemoryToDataStoreAdapter(service);
        }
    }
}