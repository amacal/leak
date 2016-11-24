using Leak.Core.Core;
using Leak.Core.Events;
using Leak.Core.Glue;
using Leak.Core.Glue.Extensions.Metadata;
using Leak.Core.Metaget;
using System;

namespace Leak.Core.Spartan
{
    public class SpartanService : IDisposable
    {
        private readonly LeakPipeline pipeline;
        private readonly string destination;
        private readonly GlueService glue;
        private readonly SpartanHooks hooks;
        private readonly SpartanFacts facts;

        public SpartanService(LeakPipeline pipeline, string destination, GlueService glue, SpartanHooks hooks, SpartanConfiguration configuration)
        {
            this.pipeline = pipeline;
            this.destination = destination;
            this.glue = glue;
            this.hooks = hooks;

            this.facts = new SpartanFacts(configuration);
        }

        public void Start()
        {
            ScheduleNextGoal();
        }

        public void HandleMetadataMeasured(MetadataMeasured data)
        {
            if (facts.IsOngoing(SpartanTasks.Discover))
            {
                facts.MetaGet.HandleMetadataMeasured(data);
            }
        }

        public void HandleMetadataReceived(MetadataReceived data)
        {
            if (facts.IsOngoing(SpartanTasks.Discover))
            {
                facts.MetaGet.HandleMetadataReceived(data);
            }
        }

        private void ScheduleNextGoal()
        {
            if (facts.CanStart(SpartanTasks.Discover))
            {
                MetagetHooks other = CreateMetagetHooks();
                MetagetConfiguration configuration = CreateMetagetConfiguration();

                facts.MetaGet = new MetagetService(glue, destination, other, configuration);
                facts.MetaGet.Start(pipeline);

                facts.Start(SpartanTasks.Discover);
                hooks.CallTaskStarted(glue.Hash, SpartanTasks.Discover);

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
                OnMetafileMeasured = hooks.OnMetafileMeasured,
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
            facts.MetaGet.Stop(pipeline);
            facts.MetaGet = null;

            facts.Complete(SpartanTasks.Discover);
            hooks.CallTaskCompleted(glue.Hash, SpartanTasks.Discover);

            hooks.CallMetadataDiscovered(data);
            glue.SetPieces(data.Metainfo.Pieces.Length);

            ScheduleNextGoal();
        }

        public void Dispose()
        {
            if (facts.IsOngoing(SpartanTasks.Discover))
            {
                facts.MetaGet.Stop(pipeline);
            }
        }
    }
}