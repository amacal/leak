using System;

namespace Leak.Core.Infantry
{
    public class InfantryContext
    {
        private readonly object synchronized;
        private readonly InfantryCollection collection;
        private readonly InfantryConfiguration configuration;

        public InfantryContext(Action<InfantryConfiguration> configurer)
        {
            configuration = configurer.Configure(with =>
            {
                with.Callback = new InfantryCallbackNothing();
            });

            synchronized = new object();
            collection = new InfantryCollection();
        }

        public object Synchronized
        {
            get { return synchronized; }
        }

        public InfantryCollection Collection
        {
            get { return collection; }
        }

        public InfantryConfiguration Configuration
        {
            get { return configuration; }
        }

        public InfantryCallback Callback
        {
            get { return configuration.Callback; }
        }
    }
}