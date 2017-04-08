using Leak.Common;
using Leak.Data.Get;
using Leak.Data.Store;
using Leak.Networking.Core;

namespace Leak.Client.Adapters
{
    internal class DataGetToDataStoreAdapter : DataGetToDataStore
    {
        private readonly RepositoryService service;

        public DataGetToDataStoreAdapter(RepositoryService service)
        {
            this.service = service;
        }

        public void Verify(PieceInfo piece)
        {
            service.Verify(piece);
        }

        public void Write(BlockIndex block, DataBlock data)
        {
            service.Write(block, data);
        }
    }
}