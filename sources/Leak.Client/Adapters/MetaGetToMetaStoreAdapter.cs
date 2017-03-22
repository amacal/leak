using Leak.Meta.Get;
using Leak.Meta.Store;

namespace Leak.Client.Adapters
{
    internal class MetaGetToMetaStoreAdapter : MetagetMetafile
    {
        private readonly MetafileService service;

        public MetaGetToMetaStoreAdapter(MetafileService service)
        {
            this.service = service;
        }

        public bool IsCompleted()
        {
            return service.IsCompleted();
        }

        public void Write(int piece, byte[] data)
        {
            service.Write(piece, data);
        }

        public void Verify()
        {
            service.Verify();
        }
    }
}