using Leak.Core.Core;
using System;

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
                with.Callback = new TelegraphCallbackNothing();
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

        public TelegraphCallback Callback
        {
            get { return configuration.Callback; }
        }
    }
}