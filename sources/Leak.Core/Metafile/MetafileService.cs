using System;

namespace Leak.Core.Metafile
{
    public class MetafileService
    {
        private readonly MetafileContext context;

        public MetafileService(Action<MetafileConfiguration> configurer)
        {
            context = new MetafileContext(configurer);
        }

        public void Write(int block, byte[] data)
        {
            lock (context.Synchronized)
            {
                if (context.IsCompleted == false)
                {
                    context.Destination.Write(block, data);
                    context.Destination.Verify();
                }
            }
        }

        public void Verify()
        {
            lock (context.Synchronized)
            {
                if (context.IsCompleted == false)
                {
                    context.Destination.Verify();
                }
            }
        }

        public bool IsCompleted()
        {
            lock (context.Synchronized)
            {
                return context.IsCompleted;
            }
        }
    }
}