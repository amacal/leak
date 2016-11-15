using Leak.Core.Common;
using Leak.Core.Events;
using Leak.Core.Glue;
using Leak.Core.Metaget;

namespace Leak.Core.Spartan
{
    public class SpartanService
    {
        private readonly FileHash hash;
        private readonly string destination;
        private readonly GlueService glue;
        private readonly SpartanHooks hooks;
        private readonly SpartanFacts facts;

        public SpartanService(FileHash hash, string destination, GlueService glue, SpartanHooks hooks, SpartanConfiguration configuration)
        {
            this.hash = hash;
            this.destination = destination;
            this.glue = glue;
            this.hooks = hooks;

            this.facts = new SpartanFacts(configuration);
        }

        public void Start()
        {
            ScheduleNextGoal();
        }

        private void ScheduleNextGoal()
        {
            if (facts.CanStart(SpartanGoal.Discover))
            {
                MetagetHooks hooks = CreateMetagetHooks();
                MetagetConfiguration configuration = CreateMetagetConfiguration();

                facts.MetaGet = new MetagetService(hash, destination, hooks, configuration);
                facts.MetaGet.Start(null);

                facts.Start(SpartanGoal.Discover);
                return;
            }

            if (facts.CanStart(SpartanGoal.Verify))
            {
            }

            if (facts.CanStart(SpartanGoal.Download))
            {
            }
        }

        private MetagetHooks CreateMetagetHooks()
        {
            return new MetagetHooks
            {
                OnMetadataDiscovered = OnMetadataDiscovered,
                OnMetadataMeasured = hooks.OnMetadataMeasured,
                OnMetadataPieceReceived = hooks.OnMetadataPieceReceived,
                OnMetadataPieceRequested = hooks.OnMetadataPieceRequested
            };
        }

        private MetagetConfiguration CreateMetagetConfiguration()
        {
            return new MetagetConfiguration
            {
            };
        }

        private void OnMetadataDiscovered(MetadataDiscovered data)
        {
            facts.MetaGet.Stop(null);
            facts.MetaGet.Dispose();
            facts.MetaGet = null;

            facts.Complete(SpartanGoal.Discover);
            hooks.OnMetadataDiscovered?.Invoke(data);
            glue.AddMetainfo(data.Metainfo);

            ScheduleNextGoal();
        }
    }
}