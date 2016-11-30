using Leak.Core.Common;
using Leak.Core.Core;
using Leak.Core.Events;
using Leak.Core.Glue;
using Leak.Core.Metadata;
using Leak.Core.Retriever.Components;
using Leak.Core.Retriever.Tasks;
using Leak.Files;
using System;

namespace Leak.Core.Retriever
{
    public class RetrieverService : IDisposable
    {
        private readonly RetrieverContext context;

        public RetrieverService(Metainfo metainfo, string destination, Bitfield bitfield, GlueService glue, FileFactory files, LeakPipeline pipeline, RetrieverHooks hooks, RetrieverConfiguration configuration)
        {
            context = new RetrieverContext(metainfo, destination, bitfield, glue, files, pipeline, hooks, configuration);
        }

        public void Start()
        {
            context.Repository.Start();
            context.Omnibus.Start(context.Pipeline);

            context.Pipeline.Register(context.Queue);
            context.Pipeline.Register(TimeSpan.FromMilliseconds(250), OnTick);

            context.Queue.Add(new VerifyPieceTask());
            context.Queue.Add(new FindBitfieldsTask());
        }

        public void HandlePeerChanged(PeerChanged data)
        {
            context.Queue.Add(new HandleBitfieldTask(data));
        }

        public void HandleBlockReceived(BlockReceived data)
        {
            context.Queue.Add(new HandleBlockReceived(data));
        }

        private void OnTick()
        {
            context.Queue.Add(new ScheduleAllTask());
        }

        public void Dispose()
        {
            context.Repository.Dispose();
            context.Pipeline.Remove(OnTick);
        }
    }
}