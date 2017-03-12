using Leak.Common;

namespace Leak.Meta.Store
{
    public class MetafileImplementation : MetafileService
    {
        private readonly MetafileContext context;

        public MetafileImplementation(MetafileParameters parameters, MetafileDependencies dependencies, MetafileHooks hooks)
        {
            context = new MetafileContext(parameters, dependencies, hooks);
        }

        public FileHash Hash
        {
            get { return context.Parameters.Hash; }
        }

        public MetafileHooks Hooks
        {
            get { return context.Hooks; }
        }

        public MetafileParameters Parameters
        {
            get { return context.Parameters; }
        }

        public MetafileDependencies Dependencies
        {
            get { return context.Dependencies; }
        }

        public void Start()
        {
            context.Dependencies.Pipeline.Register(context.Queue);
        }

        public void Read(int piece)
        {
            if (context.IsCompleted)
            {
                context.Destination.Read(piece);
            }
        }

        public void Write(int piece, byte[] data)
        {
            if (context.IsCompleted == false)
            {
                context.Destination.Write(piece, data);
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

        public void Dispose()
        {
            context.Destination.Dispose();
        }
    }
}