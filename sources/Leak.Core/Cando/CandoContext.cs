using System;
using Leak.Tasks;

namespace Leak.Core.Cando
{
    public class CandoContext
    {
        private readonly object synchronized;
        private readonly CandoConfiguration configuration;
        private readonly CandoCollection collection;

        public CandoContext(Action<CandoConfiguration> configurer)
        {
            configuration = configurer.Configure(with =>
            {
                with.Extensions = new CandoBuilder();
                with.Callback = new CandoCallbackNothing();
            });

            synchronized = new object();
            collection = new CandoCollection();
        }

        public object Synchronized
        {
            get { return synchronized; }
        }

        public CandoConfiguration Configuration
        {
            get { return configuration; }
        }

        public CandoCallback Callback
        {
            get { return configuration.Callback; }
        }

        public LeakBus Bus
        {
            get { return configuration.Bus; }
        }

        public CandoCollection Collection
        {
            get { return collection; }
        }
    }
}