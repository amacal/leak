using Leak.Common;
using Leak.Events;
using Leak.Metaget;
using Leak.Repository;
using Leak.Retriever;
using Leak.Tasks;

namespace Leak.Spartan
{
    public class SpartanScheduleNext : LeakTask<SpartanContext>
    {
        private readonly SpartanContext context;

        public SpartanScheduleNext(SpartanContext context)
        {
            this.context = context;
        }

        public void Execute(SpartanContext remove)
        {
            if (context.Facts.CanStart(SpartanTasks.Discover))
            {
                MetagetHooks hooks = CreateMetagetHooks();
                MetagetConfiguration configuration = CreateMetagetConfiguration();

                context.Facts.MetaGet = new MetagetService(context.Pipeline, context.Glue, context.Destination, hooks, configuration);
                context.Facts.Start(SpartanTasks.Discover);

                context.Hooks.CallTaskStarted(context.Glue.Hash, SpartanTasks.Discover);
                context.Facts.MetaGet.Start();

                return;
            }

            if (context.Facts.CanStart(SpartanTasks.Verify))
            {
                RepositoryHooks hooks = CreateRepositoryHooks();
                RepositoryConfiguration configuration = CreateRepositoryConfiguration();

                context.Facts.Repository = new RepositoryService(context.Facts.Metainfo, context.Destination, context.Files, hooks, configuration);
                context.Facts.Start(SpartanTasks.Verify);

                context.Hooks.CallTaskStarted(context.Glue.Hash, SpartanTasks.Verify);
                context.Facts.Repository.Start();

                context.Facts.Repository.Verify(new Bitfield(context.Facts.Metainfo.Pieces.Length));
            }

            if (context.Facts.CanStart(SpartanTasks.Download))
            {
                RetrieverHooks hooks = CreateRetrieverHooks();
                RetrieverConfiguration configuration = CreateRetrieverConfiguration();

                context.Facts.Retriever = new RetrieverService(context.Facts.Metainfo, context.Destination, context.Facts.Bitfield, context.Glue, context.Files, context.Pipeline, hooks, configuration);
                context.Facts.Start(SpartanTasks.Download);

                context.Hooks.CallTaskStarted(context.Glue.Hash, SpartanTasks.Download);
                context.Facts.Retriever.Start();
            }
        }

        private MetagetHooks CreateMetagetHooks()
        {
            return new MetagetHooks
            {
                OnMetadataDiscovered = data => context.Queue.Add(new SpartanCompleteDiscover(data)),
                OnMetafileMeasured = context.Hooks.OnMetafileMeasured,
            };
        }

        private MetagetConfiguration CreateMetagetConfiguration()
        {
            return new MetagetConfiguration
            {
            };
        }

        private RepositoryHooks CreateRepositoryHooks()
        {
            return new RepositoryHooks
            {
                OnDataAllocated = context.Hooks.OnDataAllocated,
                OnDataVerified = OnDataVerified
            };
        }

        private void OnDataVerified(DataVerified data)
        {
            context.Queue.Add(new SpartanCompleteVerify(data));
        }

        private RepositoryConfiguration CreateRepositoryConfiguration()
        {
            return new RepositoryConfiguration
            {
            };
        }

        private RetrieverHooks CreateRetrieverHooks()
        {
            return new RetrieverHooks
            {
                OnDataCompleted = OnDataCompleted,
                OnDataChanged = context.Hooks.OnDataChanged,
                OnPieceAccepted = context.Hooks.OnPieceAccepted,
                OnPieceRejected = context.Hooks.OnPieceRejected
            };
        }

        private void OnDataCompleted(DataCompleted data)
        {
            context.Queue.Add(new SpartanCompleteDownload(data));
        }

        private RetrieverConfiguration CreateRetrieverConfiguration()
        {
            return new RetrieverConfiguration
            {
            };
        }
    }
}