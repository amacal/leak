using System;

namespace Leak.Core.Communicator
{
    public class CommunicatorContext
    {
        private readonly object synchronized;
        private readonly CommunicatorConfiguration configuration;
        private readonly CommunicatorCollection collection;

        public CommunicatorContext(Action<CommunicatorConfiguration> configurer)
        {
            configuration = configurer.Configure(with =>
            {
            });

            synchronized = new object();
            collection = new CommunicatorCollection();
        }

        public object Synchronized
        {
            get { return synchronized; }
        }

        public CommunicatorConfiguration Configuration
        {
            get { return configuration; }
        }

        public CommunicatorCollection Collection
        {
            get { return collection; }
        }
    }
}