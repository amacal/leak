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
                    context.Destination.Validate();
                }
            }
        }
    }
}