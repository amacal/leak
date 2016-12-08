using Leak.Common;
using Leak.Core.Omnibus.Tasks;
using Leak.Events;
using Leak.Tasks;
using System;
using System.Collections.Generic;

namespace Leak.Core.Omnibus
{
    /// <summary>
    /// Manages the global bitfield for a given FileHash. It manages all peers
    /// which announced their bitfield, tracks all requested pieces/blocks and
    /// monitors completed pieces.
    /// </summary>
    public class OmnibusService : IDisposable
    {
        private readonly LeakPipeline pipeline;
        private readonly OmnibusContext context;

        public OmnibusService(Metainfo metainfo, Bitfield bitfield, LeakPipeline pipeline, OmnibusHooks hooks, OmnibusConfiguration configuration)
        {
            this.pipeline = pipeline;
            context = new OmnibusContext(metainfo, bitfield, hooks, configuration);
        }

        public void Start()
        {
            pipeline.Register(context.Queue);
        }

        public bool IsComplete()
        {
            return context.Pieces.IsComplete();
        }

        public bool IsComplete(int piece)
        {
            return context.Pieces.IsComplete(piece);
        }

        public IEnumerable<PeerHash> Find(int ranking, int count)
        {
            return context.States.Find(ranking, count);
        }

        public void Handle(PeerChanged data)
        {
            context.States.Handle(data);
            context.Bitfields.Add(data.Peer, data.Bitfield);
        }

        public void Complete(OmnibusBlock block)
        {
            context.Queue.Add(new CompleteBlockTask(block));
        }

        public void Complete(int piece)
        {
            context.Queue.Add(new CompletePieceTask(piece));
        }

        public void Invalidate(int piece)
        {
            context.Queue.Add(new InvalidatePieceTask(piece));
        }

        public void Schedule(OmnibusStrategy strategy, PeerHash peer, int count)
        {
            context.Queue.Add(new SchedulePeerTask(strategy, peer, count));
        }

        public void Dispose()
        {
        }
    }
}