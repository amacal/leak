using Leak.Common;
using Leak.Data.Share;
using Leak.Data.Store;

namespace Leak.Client.Adapters
{
    public class DataShareToDataStoreAdapter : DataShareToDataStore
    {
        private readonly RepositoryService service;

        public DataShareToDataStoreAdapter(RepositoryService service)
        {
            this.service = service;
        }

        public void Read(BlockIndex index)
        {
            service.Read(index);
        }
    }
}