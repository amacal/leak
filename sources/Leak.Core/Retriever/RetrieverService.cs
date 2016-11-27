using Leak.Core.Common;
using Leak.Core.Core;
using Leak.Core.Events;
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

        public RetrieverService(Metainfo metainfo, string destination, Bitfield bitfield, FileFactory files, LeakPipeline pipeline, RetrieverHooks hooks, RetrieverConfiguration configuration)
        {
            context = new RetrieverContext(metainfo, destination, bitfield, files, pipeline, hooks, configuration);
        }

        public void Start()
        {
            context.Repository.Start();
            context.Omnibus.Start(context.Pipeline);

            context.Pipeline.Register(context.Queue);
            context.Pipeline.Register(TimeSpan.FromMilliseconds(250), OnTick);

            //context.Callback.OnFileStarted(context.Metainfo.Hash);

            context.Queue.Add(new VerifyPieceTask());
            context.Queue.Add(new FindBitfieldsTask());
        }

        public void OnBitfield(PeerHash peer, Bitfield bitfield)
        {
            context.Queue.Add(new HandleBitfieldTask(peer, bitfield));
        }

        public void HandleDataReceived(DataReceived data)
        {
            context.Queue.Add(new HandleDataReceived(data));
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