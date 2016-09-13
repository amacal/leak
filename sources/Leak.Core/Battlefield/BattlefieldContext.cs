using System;

namespace Leak.Core.Battlefield
{
    public class BattlefieldContext
    {
        private readonly object synchronized;
        private readonly BattlefieldConfiguration configuration;
        private readonly BattlefieldCollection collection;

        public BattlefieldContext(Action<BattlefieldConfiguration> configurer)
        {
            configuration = configurer.Configure(with =>
            {
            });

            synchronized = new object();
            collection = new BattlefieldCollection();
        }

        public object Synchronized
        {
            get { return synchronized; }
        }

        public BattlefieldCallback Callback
        {
            get { return configuration.Callback; }
        }

        public BattlefieldCollection Collection
        {
            get { return collection; }
        }
    }
}