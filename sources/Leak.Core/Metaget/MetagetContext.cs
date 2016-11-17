using Leak.Core.Common;
using Leak.Core.Core;
using Leak.Core.Metafile;
using Leak.Core.Metamine;

namespace Leak.Core.Metaget
{
    public class MetagetContext
    {
        private readonly FileHash hash;
        private readonly string destination;
        private readonly MetagetHooks hooks;
        private readonly MetagetConfiguration configuration;
        private readonly LeakQueue<MetagetContext> queue;
        private readonly MetafileService metafile;

        private MetamineBitfield metamine;

        public MetagetContext(FileHash hash, string destination, MetagetHooks hooks, MetagetConfiguration configuration)
        {
            this.hash = hash;
            this.destination = destination;
            this.hooks = hooks;
            this.configuration = configuration;

            metafile = CreateMetafile();
            queue = new LeakQueue<MetagetContext>(this);
        }

        private MetafileService CreateMetafile()
        {
            string path = destination + ".metainfo";
            MetafileHooks hooks = new MetafileHooks
            {
                OnMetafileVerified = data =>
                {
                    this.hooks.CallMetadataDiscovered(data.Hash, data.Metainfo);
                }
            };

            return new MetafileService(hash, path, hooks);
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

        public LeakQueue<MetagetContext> Queue
        {
            get { return queue; }
        }

        public MetafileService Metafile
        {
            get { return metafile; }
        }

        public MetagetHooks Hooks
        {
            get { return hooks; }
        }

        public FileHash Hash
        {
            get { return hash; }
        }
    }
}