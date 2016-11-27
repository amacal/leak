using Leak.Core.Common;
using Leak.Core.Core;
using Leak.Core.Metaget;
using Leak.Core.Repository;

namespace Leak.Core.Spartan
{
    public class SpartanScheduleNext : LeakTask<SpartanContext>
    {
        public void Execute(SpartanContext context)
        {
            if (context.Facts.CanStart(SpartanTasks.Discover))
            {
                MetagetHooks hooks = CreateMetagetHooks(context);
                MetagetConfiguration configuration = CreateMetagetConfiguration();

                context.Facts.MetaGet = new MetagetService(context.Glue, context.Destination, hooks, configuration);
                context.Facts.Start(SpartanTasks.Discover);

                context.Hooks.CallTaskStarted(context.Glue.Hash, SpartanTasks.Discover);
                context.Facts.MetaGet.Start(context.Pipeline);

                return;
            }

            if (context.Facts.CanStart(SpartanTasks.Verify))
            {
                RepositoryHooks hooks = CreateRepositoryHooks(context);
                RepositoryConfiguration configuration = CreateRepositoryConfiguration();

                context.Facts.Repository = new RepositoryService(context.Facts.Metainfo, context.Destination, context.Files, hooks, configuration);
                context.Facts.Start(SpartanTasks.Verify);

                context.Hooks.CallTaskStarted(context.Glue.Hash, SpartanTasks.Verify);
                context.Facts.Repository.Start();

                context.Facts.Repository.Verify(new Bitfield(context.Facts.Metainfo.Pieces.Length));
            }

            if (context.Facts.CanStart(SpartanTasks.Download))
            {
            }
        }

        private MetagetHooks CreateMetagetHooks(SpartanContext context)
        {
            return new MetagetHooks
            {
                OnMetadataDiscovered = data => context.Queue.Add(new SpartanCompleteDiscover(data)),
                OnMetafileMeasured = context.Hooks.OnMetafileMeasured,
                OnMetadataPieceReceived = context.Hooks.OnMetadataPieceReceived,
                OnMetadataPieceRequested = context.Hooks.OnMetadataPieceRequested
            };
        }

        private MetagetConfiguration CreateMetagetConfiguration()
        {
            return new MetagetConfiguration
            {
            };
        }

        private RepositoryHooks CreateRepositoryHooks(SpartanContext context)
        {
            return new RepositoryHooks
            {
                OnDataAllocated = context.Hooks.OnDataAllocated,
                OnDataVerified = data => context.Queue.Add(new SpartanCompleteVerify(data))
            };
        }

        private RepositoryConfiguration CreateRepositoryConfiguration()
        {
            return new RepositoryConfiguration
            {
            };
        }
    }
}