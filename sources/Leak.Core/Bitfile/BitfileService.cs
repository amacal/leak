using Leak.Core.Common;
using System;

namespace Leak.Core.Bitfile
{
    public class BitfileService
    {
        private readonly BitfileContext context;

        public BitfileService(Action<BitfileConfiguration> configurer)
        {
            context = new BitfileContext(configurer);
        }

        public Bitfield Read()
        {
            lock (context.Synchronized)
            {
                return context.Destination.Read();
            }
        }

        public void Write(Bitfield bitfield)
        {
            lock (context.Synchronized)
            {
                context.Destination.Write(bitfield);
            }
        }
    }
}