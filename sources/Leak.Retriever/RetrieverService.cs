using System;
using Leak.Common;
using Leak.Events;
using Leak.Files;
using Leak.Glue;
using Leak.Retriever.Components;
using Leak.Retriever.Tasks;
using Leak.Tasks;

namespace Leak.Retriever
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
            context.Omnibus.Start();

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