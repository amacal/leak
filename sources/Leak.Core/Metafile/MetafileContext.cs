using System;

namespace Leak.Core.Metafile
{
    public class MetafileContext
    {
        private readonly object synchronized;
        private readonly MetafileConfiguration configuration;
        private readonly MetafileDestination destination;

        private bool isCompleted;

        public MetafileContext(Action<MetafileConfiguration> configurer)
        {
            configuration = configurer.Configure(with =>
            {
                with.Callback = new MetafileCallbackNothing();
            });

            synchronized = new object();
            destination = new MetafileDestination(this);
        }

        public bool IsCompleted
        {
            get { return isCompleted; }
            set { isCompleted = value; }
        }

        public object Synchronized
        {
            get { return synchronized; }
        }

        public MetafileDestination Destination
        {
            get { return destination; }
        }

        public MetafileCallback Callback
        {
            get { return configuration.Callback; }
        }

        public MetafileConfiguration Configuration
        {
            get { return configuration; }
        }
    }
}