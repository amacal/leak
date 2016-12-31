using Leak.Common;
using Leak.Events;
using Leak.Metafile;
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
            if (context.Facts.CanStart(Goal.Discover))
            {
                context.Facts.Start(Goal.Discover);
                context.Hooks.CallTaskStarted(context.Parameters.Hash, Goal.Discover);
                context.Dependencies.Metaget.Start();
                context.Dependencies.Metashare.Start();

                return;
            }

            if (context.Facts.CanStart(Goal.Verify))
            {
                RepositoryHooks hooks = CreateRepositoryHooks();
                RepositoryConfiguration configuration = CreateRepositoryConfiguration();

                context.Facts.Repository = new RepositoryService(context.Facts.Metainfo, context.Parameters.Destination, context.Dependencies.Files, hooks, configuration);
                context.Facts.Start(Goal.Verify);

                context.Hooks.CallTaskStarted(context.Parameters.Hash, Goal.Verify);
                context.Facts.Repository.Start();

                context.Facts.Repository.Verify(new Bitfield(context.Facts.Metainfo.Pieces.Length));
            }

            if (context.Facts.CanStart(Goal.Download))
            {
                RetrieverHooks hooks = CreateRetrieverHooks();
                RetrieverConfiguration configuration = CreateRetrieverConfiguration();

                context.Facts.Retriever = new RetrieverService(context.Facts.Metainfo, context.Parameters.Destination, context.Facts.Bitfield, context.Dependencies.Glue, context.Dependencies.Files, context.Dependencies.Pipeline, hooks, configuration);
                context.Facts.Start(Goal.Download);

                context.Hooks.CallTaskStarted(context.Parameters.Hash, Goal.Download);
                context.Facts.Retriever.Start();
            }
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