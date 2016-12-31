using Leak.Common;

namespace Leak.Metafile
{
    public class MetafileService
    {
        private readonly MetafileContext context;

        public MetafileService(MetafileParameters parameters, MetafileHooks hooks)
        {
            context = new MetafileContext(parameters, hooks);
        }

        public FileHash Hash
        {
            get { return context.Parameters.Hash; }
        }

        public MetafileParameters Parameters
        {
            get { return context.Parameters; }
        }

        public MetafileHooks Hooks
        {
            get { return context.Hooks; }
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