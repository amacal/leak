using System;
using Leak.Common;

namespace Leak.Metafile
{
    public class MetafileService : IDisposable
    {
        private readonly MetafileContext context;

        public MetafileService(MetafileParameters parameters, MetafileDependencies dependencies, MetafileHooks hooks)
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

        public void Dispose()
        {
            context.Destination.Dispose();
        }
    }
}