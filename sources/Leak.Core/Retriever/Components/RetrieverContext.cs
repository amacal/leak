using Leak.Common;
using Leak.Core.Retriever.Tasks;
using Leak.Events;
using Leak.Files;
using Leak.Glue;
using Leak.Omnibus;
using Leak.Repository;
using Leak.Tasks;

namespace Leak.Core.Retriever.Components
{
    public class RetrieverContext
    {
        private readonly Metainfo metainfo;
        private readonly GlueService glue;
        private readonly LeakPipeline pipeline;
        private readonly RetrieverHooks hooks;
        private readonly RetrieverConfiguration configuration;
        private readonly RepositoryService repository;
        private readonly OmnibusService omnibus;
        private readonly LeakQueue<RetrieverContext> queue;

        public RetrieverContext(Metainfo metainfo, string destination, Bitfield bitfield, GlueService glue, FileFactory files, LeakPipeline pipeline, RetrieverHooks hooks, RetrieverConfiguration configuration)
        {
            this.metainfo = metainfo;
            this.glue = glue;
            this.pipeline = pipeline;
            this.hooks = hooks;
            this.configuration = configuration;

            repository = CreateRepositoryService(destination, files);
            omnibus = CreateOmnibusService(bitfield);

            queue = new LeakQueue<RetrieverContext>(this);
        }

        private RepositoryService CreateRepositoryService(string destination, FileFactory files)
        {
            RepositoryHooks hooks = CreateRepositoryHooks();
            RepositoryConfiguration configuration = new RepositoryConfiguration();

            return new RepositoryService(metainfo, destination, files, hooks, configuration);
        }

        private RepositoryHooks CreateRepositoryHooks()
        {
            return new RepositoryHooks
            {
                OnBlockWritten = OnBlockWritten,
                OnPieceAccepted = OnPieceAccepted,
                OnPieceRejected = OnPieceRejected
            };
        }

        private void OnBlockWritten(BlockWritten data)
        {
            omnibus.Complete(new OmnibusBlock(data.Piece, data.Block * 16384, data.Size));
        }

        private void OnPieceAccepted(PieceAccepted data)
        {
            omnibus.Complete(data.Piece);
            hooks.CallPieceAccepted(data);
        }

        private void OnPieceRejected(PieceRejected data)
        {
            omnibus.Invalidate(data.Piece);
            hooks.CallPieceRejected(data);
        }

        private OmnibusService CreateOmnibusService(Bitfield bitfield)
        {
            OmnibusHooks hooks = CreateOmnibusHooks();
            OmnibusConfiguration configuration = new OmnibusConfiguration();

            return new OmnibusService(metainfo, bitfield, pipeline, hooks, configuration);
        }

        private OmnibusHooks CreateOmnibusHooks()
        {
            return new OmnibusHooks
            {
                OnDataChanged = hooks.OnDataChanged,
                OnDataCompleted = hooks.OnDataCompleted,
                OnPieceReady = OnPieceReady,
                OnBlockReserved = OnBlockReserved
            };
        }

        private void OnPieceReady(PieceReady data)
        {
            repository.Verify(new PieceInfo(data.Piece));
        }

        private void OnBlockReserved(BlockReserved data)
        {
            queue.Add(new RequestBlockTask(data));
        }

        public RetrieverConfiguration Configuration
        {
            get { return configuration; }
        }

        public GlueService Glue
        {
            get { return glue; }
        }

        public Metainfo Metainfo
        {
            get { return metainfo; }
        }

        public RepositoryService Repository
        {
            get { return repository; }
        }

        public LeakQueue<RetrieverContext> Queue
        {
            get { return queue; }
        }

        public OmnibusService Omnibus
        {
            get { return omnibus; }
        }

        public LeakPipeline Pipeline
        {
            get { return pipeline; }
        }

        public RetrieverHooks Hooks
        {
            get { return hooks; }
        }
    }
}