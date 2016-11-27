using Leak.Core.Common;
using Leak.Core.Core;
using Leak.Core.Events;
using Leak.Core.Glue;
using Leak.Core.Metadata;
using Leak.Core.Omnibus;
using Leak.Core.Repository;
using Leak.Core.Retriever.Callbacks;
using Leak.Files;

namespace Leak.Core.Retriever.Components
{
    public class RetrieverContext
    {
        private readonly Metainfo metainfo;
        private readonly LeakPipeline pipeline;
        private readonly RetrieverHooks hooks;
        private readonly RetrieverConfiguration configuration;
        private readonly RepositoryService repository;
        private readonly OmnibusService omnibus;
        private readonly LeakQueue<RetrieverContext> queue;

        public RetrieverContext(Metainfo metainfo, string destination, Bitfield bitfield, FileFactory files, LeakPipeline pipeline, RetrieverHooks hooks, RetrieverConfiguration configuration)
        {
            this.metainfo = metainfo;
            this.pipeline = pipeline;
            this.hooks = hooks;
            this.configuration = configuration;

            repository = CreateRepositoryService(destination, files);

            omnibus = new OmnibusService(with =>
            {
                with.Metainfo = metainfo;
                with.Bitfield = bitfield;
                with.Callback = new RetrieverToOmnibus(this);
            });

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
                OnDataWritten = OnDataWritten,
                OnDataAccepted = OnDataAccepted,
                OnDataRejected = OnDataRejected
            };
        }

        private void OnDataWritten(DataWritten data)
        {
            omnibus.Complete(new OmnibusBlock(data.Piece, data.Block * 16384, data.Size));
        }

        private void OnDataAccepted(DataAccepted data)
        {
            omnibus.Complete(data.Piece);
            //context.Callback.OnPieceVerified(hash, piece)
        }

        private void OnDataRejected(DataRejected data)
        {
            omnibus.Invalidate(data.Piece);
            //context.Callback.OnPieceRejected(hash, piece);
        }

        public RetrieverConfiguration Configuration
        {
            get { return configuration; }
        }

        public GlueService Glue
        {
            get { return null; }
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