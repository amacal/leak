using Leak.Core.Core;
using System;

namespace Leak.Core.Telegraph
{
    public class TelegraphContext
    {
        private readonly object synchronized;
        private readonly TelegraphConfiguration configuration;

        private readonly TelegraphCollection collection;
        private readonly LeakTimer timer;
        private readonly LeakQueue<TelegraphContext> queue;

        public TelegraphContext(Action<TelegraphConfiguration> configurer)
        {
            configuration = configurer.Configure(with =>
            {
                with.Callback = new TelegraphCallbackNothing();
            });

            synchronized = new object();
            collection = new TelegraphCollection();

            timer = new LeakTimer(TimeSpan.FromSeconds(5));
            queue = new LeakQueue<TelegraphContext>();
        }

        public object Synchronized
        {
            get { return synchronized; }
        }

        public TelegraphCollection Collection
        {
            get { return collection; }
        }

        public LeakTimer Timer
        {
            get { return timer; }
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