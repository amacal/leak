using Leak.Common;

namespace Leak.Core.Metafile
{
    public class MetafileService
    {
        private readonly MetafileContext context;

        public MetafileService(FileHash hash, string destination, MetafileHooks hooks)
        {
            context = new MetafileContext(hash, destination, hooks);
        }

        public void Write(int piece, byte[] data)
        {
            if (context.IsCompleted == false)
            {
                context.Destination.Write(piece, data);
                context.Destination.Verify();
            }
        }

        public void Verify()
        {
            if (context.IsCompleted == false)
            {
                context.Destination.Verify();
            }
        }

        public bool IsCompleted()
        {
            return context.IsCompleted;
        }
    }
}