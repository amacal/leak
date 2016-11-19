using Leak.Core.Events;
using Leak.Core.Glue;
using Leak.Core.Metaget;

namespace Leak.Core.Spartan
{
    public class SpartanService
    {
        private readonly string destination;
        private readonly GlueService glue;
        private readonly SpartanHooks hooks;
        private readonly SpartanFacts facts;

        public SpartanService(string destination, GlueService glue, SpartanHooks hooks, SpartanConfiguration configuration)
        {
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
            if (facts.CanStart(SpartanTasks.Discover))
            {
                MetagetHooks hooks = CreateMetagetHooks();
                MetagetConfiguration configuration = CreateMetagetConfiguration();

                facts.MetaGet = new MetagetService(glue, destination, hooks, configuration);
                facts.MetaGet.Start(null);

                facts.Start(SpartanTasks.Discover);
                return;
            }

            if (facts.CanStart(SpartanTasks.Verify))
            {
            }

            if (facts.CanStart(SpartanTasks.Download))
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
            facts.MetaGet = null;

            facts.Complete(SpartanTasks.Discover);
            hooks.OnMetadataDiscovered?.Invoke(data);
            glue.SetPieces(data.Metainfo.Pieces.Length);

            ScheduleNextGoal();
        }
    }
}