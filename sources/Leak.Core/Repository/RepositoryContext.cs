using Leak.Core.Core;
using Leak.Core.Metadata;
using System;

namespace Leak.Core.Repository
{
    public class RepositoryContext
    {
        private readonly RepositoryConfiguration configuration;
        private readonly RepositoryTaskQueue queue;
        private readonly LeakTimer timer;

        private readonly byte[] buffer;

        public RepositoryContext(Action<RepositoryConfiguration> configurer)
        {
            configuration = configurer.Configure(with =>
            {
                with.Callback = new RepositoryCallbackNothing();
            });

            queue = new RepositoryTaskQueue();
            timer = new LeakTimer(TimeSpan.FromMilliseconds(50));
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

        public RepositoryTaskQueue Queue
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