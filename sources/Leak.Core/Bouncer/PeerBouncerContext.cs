using System;

namespace Leak.Core.Bouncer
{
    public class PeerBouncerContext
    {
        private readonly object synchronized;
        private readonly PeerBouncerConfiguration configuration;
        private readonly PeerBouncerCollection collection;

        public PeerBouncerContext(Action<PeerBouncerConfiguration> configurer)
        {
            configuration = configurer.Configure(with =>
            {
                with.Callback = new PeerBouncerCallbackNothing();
                with.Connections = 128;
            });

            synchronized = new object();
            collection = new PeerBouncerCollection();
        }

        public object Synchronized
        {
            get { return synchronized; }
        }

        public PeerBouncerConfiguration Configuration
        {
            get { return configuration; }
        }

        public PeerBouncerCallback Callback
        {
            get { return configuration.Callback; }
        }

        public PeerBouncerCollection Collection
        {
            get { return collection; }
        }
    }
}