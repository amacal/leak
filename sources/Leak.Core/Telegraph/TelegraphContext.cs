using System;
using Leak.Tasks;

namespace Leak.Core.Telegraph
{
    public class TelegraphContext
    {
        private readonly object synchronized;
        private readonly TelegraphConfiguration configuration;

        private readonly TelegraphCollection collection;
        private readonly LeakQueue<TelegraphContext> queue;

        public TelegraphContext(Action<TelegraphConfiguration> configurer)
        {
            configuration = configurer.Configure(with =>
            {
            });

            synchronized = new object();
            collection = new TelegraphCollection();

            queue = new LeakQueue<TelegraphContext>(this);
        }

        public object Synchronized
        {
            get { return synchronized; }
        }

        public TelegraphCollection Collection
        {
            get { return collection; }
        }

        public LeakQueue<TelegraphContext> Queue
        {
            get { return queue; }
        }

        public TelegraphConfiguration Configuration
        {
            get { return configuration; }
        }

        public LeakBus Bus
        {
            get { return configuration.Bus; }
        }
    }
}