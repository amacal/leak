using Leak.Tasks;
using System;

namespace Leak.Core.Bitfile
{
    public class BitfileContext
    {
        private readonly object synchronized;
        private readonly BitfileConfiguration configuration;
        private readonly BitfileDestination destination;

        public BitfileContext(Action<BitfileConfiguration> configurer)
        {
            configuration = configurer.Configure(with =>
            {
                with.Callback = new BitfileCallbackNothing();
            });

            destination = new BitfileDestination(this);
            synchronized = new object();
        }

        public object Synchronized
        {
            get { return synchronized; }
        }

        public BitfileDestination Destination
        {
            get { return destination; }
        }

        public BitfileConfiguration Configuration
        {
            get { return configuration; }
        }
    }
}