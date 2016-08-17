using Leak.Core.Collector;
using Leak.Core.Metafile;
using Leak.Core.Metamine;
using System;

namespace Leak.Core.Metaget
{
    public class MetagetContext
    {
        private readonly MetagetConfiguration configuration;
        private readonly MetagetQueue queue;
        private readonly MetagetTimer timer;
        private readonly MetafileService metafile;

        private MetamineBitfield metamine;

        public MetagetContext(Action<MetagetConfiguration> configurer)
        {
            configuration = configurer.Configure(with =>
            {
                with.Callback = new MetagetCallbackNothing();
            });

            metafile = new MetafileService(with =>
            {
                with.Hash = configuration.Hash;
                with.Destination = configuration.Destination;
                with.Callback = new MetagetMetafile(this);
            });

            queue = new MetagetQueue();
            timer = new MetagetTimer(TimeSpan.FromSeconds(1));
        }

        public MetamineBitfield Metamine
        {
            get { return metamine; }
            set { metamine = value; }
        }

        public MetagetConfiguration Configuration
        {
            get { return configuration; }
        }

        public PeerCollectorView View
        {
            get { return configuration.View; }
        }

        public MetagetCallback Callback
        {
            get { return configuration.Callback; }
        }

        public MetagetQueue Queue
        {
            get { return queue; }
        }

        public MetagetTimer Timer
        {
            get { return timer; }
        }

        public MetafileService Metafile
        {
            get { return metafile; }
        }
    }
}