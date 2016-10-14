using System;

namespace Leak.Core.Responder
{
    public class ResponderContext
    {
        private readonly object synchronized;
        private readonly ResponderConfiguration configuration;
        private readonly ResponderCollection collection;

        public ResponderContext(Action<ResponderConfiguration> configurer)
        {
            configuration = configurer.Configure(with =>
            {
            });

            synchronized = new object();
            collection = new ResponderCollection();
        }

        public object Synchronized
        {
            get { return synchronized; }
        }

        public ResponderConfiguration Configuration
        {
            get { return configuration; }
        }

        public ResponderCollection Collection
        {
            get { return collection; }
        }
    }
}