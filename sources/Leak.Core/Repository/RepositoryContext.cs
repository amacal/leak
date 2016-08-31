using Leak.Core.Core;
using Leak.Core.Metadata;
using System;

namespace Leak.Core.Repository
{
    public class RepositoryContext
    {
        private readonly RepositoryConfiguration configuration;
        private readonly LeakQueue<RepositoryContext> queue;
        private readonly LeakTimer timer;

        private readonly byte[] buffer;

        public RepositoryContext(Action<RepositoryConfiguration> configurer)
        {
            configuration = configurer.Configure(with =>
            {
                with.Callback = new RepositoryCallbackNothing();
            });

            queue = new LeakQueue<RepositoryContext>();
            timer = new LeakTimer(TimeSpan.FromSeconds(0.25));
            buffer = new byte[configuration.Metainfo.Properties.PieceSize];
        }

        public RepositoryConfiguration Configuration
        {
            get { return configuration; }
        }

        public RepositoryCallback Callback
        {
            get { return configuration.Callback; }
        }

        public Metainfo Metainfo
        {
            get { return configuration.Metainfo; }
        }

        public string Destination
        {
            get { return configuration.Destination; }
        }

        public LeakQueue<RepositoryContext> Queue
        {
            get { return queue; }
        }

        public LeakTimer Timer
        {
            get { return timer; }
        }

        public byte[] Buffer
        {
            get { return buffer; }
        }
    }
}