using Leak.Core.Common;
using Leak.Core.Core;
using Leak.Core.Omnibus.Tasks;
using System;

namespace Leak.Core.Omnibus
{
    /// <summary>
    /// Manages the global bitfield for a given FileHash. It manages all peers
    /// which announced their bitfield, tracks all requested pieces/blocks and
    /// monitors completed pieces.
    /// </summary>
    public class OmnibusService
    {
        private readonly OmnibusContext context;

        public OmnibusService(Action<OmnibusConfiguration> configurer)
        {
            context = new OmnibusContext(configurer);
        }

        public void Start(LeakPipeline pipeline)
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

        /// <summary>
        /// Registers a bitfield belonging to the peer.
        /// </summary>
        /// <param name="peer">The peer affected by a bitfield.</param>
        /// <param name="bitfield">The bitfield of requested hash.</param>
        public void Add(PeerHash peer, Bitfield bitfield)
        {
            context.Queue.Add(new AddBitfieldTask(peer, bitfield));
        }

        /// <summary>
        /// Reports completeness of the received block.
        /// </summary>
        /// <param name="block">The received block structure.</param>
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
    }
}