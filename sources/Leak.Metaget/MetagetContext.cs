using Leak.Events;
using Leak.Glue;
using Leak.Metafile;
using Leak.Tasks;

namespace Leak.Metaget
{
    public class MetagetContext
    {
        private readonly GlueService glue;
        private readonly string destination;
        private readonly MetagetHooks hooks;
        private readonly MetagetConfiguration configuration;
        private readonly LeakQueue<MetagetContext> queue;
        private readonly MetafileService metafile;

        private MetamineBitfield metamine;

        public MetagetContext(GlueService glue, string destination, MetagetHooks hooks, MetagetConfiguration configuration)
        {
            this.glue = glue;
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
                OnMetafileVerified = OnMetafileVerified
            };

            return new MetafileService(glue.Hash, path, hooks);
        }

        private void OnMetafileVerified(MetafileVerified data)
        {
            this.hooks.CallMetadataDiscovered(data.Hash, data.Metainfo);
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

        public GlueService Glue
        {
            get { return glue; }
        }
    }
}