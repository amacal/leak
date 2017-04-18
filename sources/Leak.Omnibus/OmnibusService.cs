using System;
using System.Collections.Generic;
using Leak.Common;
using Leak.Data.Map.Components;
using Leak.Data.Map.Tasks;
using Leak.Events;
using Leak.Peer.Coordinator.Events;
using Leak.Tasks;

namespace Leak.Data.Map
{
    /// <summary>
    /// Manages the global bitfield for a given FileHash. It manages all peers
    /// which announced their bitfield, tracks all requested pieces/blocks and
    /// monitors completed pieces.
    /// </summary>
    public class OmnibusService : IDisposable
    {
        private readonly OmnibusContext context;

        public OmnibusService(OmnibusParameters parameters, OmnibusDependencies dependencies, OmnibusConfiguration configuration, OmnibusHooks hooks)
        {
            context = new OmnibusContext(parameters, dependencies, configuration, hooks);
        }

        public FileHash Hash
        {
            get { return context.Parameters.Hash; }
        }

        public OmnibusHooks Hooks
        {
            get { return context.Hooks; }
        }

        public OmnibusParameters Parameters
        {
            get { return context.Parameters; }
        }

        public OmnibusDependencies Dependencies
        {
            get { return context.Dependencies; }
        }

        public OmnibusConfiguration Configuration
        {
            get { return context.Configuration; }
        }

        public void Start()
        {
            context.Dependencies.Pipeline.Register(context.Queue);
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

        public void Handle(PeerConnected data)
        {
            context.Queue.Add(() =>
            {
                context.States.Handle(data);
            });
        }

        public void Handle(PeerDisconnected data)
        {
            context.Queue.Add(() =>
            {
                context.States.Handle(data);
                context.Bitfields.Handle(data);
            });
        }

        public void Handle(BitfieldChanged data)
        {
            context.Queue.Add(() =>
            {
                context.Bitfields.Handle(data);
            });
        }

        public void Handle(StatusChanged data)
        {
            context.Queue.Add(() =>
            {
                context.States.Handle(data);
            });
        }

        public void Handle(DataVerified data)
        {
            context.Queue.Add(() =>
            {
                context.Bitfield = data.Bitfield;
                context.Bitfields.Ranking.Complete(data.Bitfield);

                context.Cache = new OmnibusCache(data.Bitfield.Length);
                context.Pieces = new OmnibusPieceCollection(context);
            });
        }

        public void Handle(MetafileVerified data)
        {
            context.Queue.Add(() =>
            {
                context.Metainfo = data.Metainfo;
            });
        }

        public void Complete(BlockIndex block)
        {
            context.Queue.Add(new CompleteBlockTask(block));
        }

        public void Complete(PieceInfo piece)
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

        public void Query(Action<PeerHash, Bitfield, PeerState> callback)
        {
            foreach (PeerHash peer in context.States.All())
            {
                Bitfield bitfield = context.Bitfields?.ByPeer(peer);
                OmnibusStateEntry entry = context.States.ByPeer(peer);

                callback.Invoke(peer, bitfield, entry?.State);
            }
        }

        public void Dispose()
        {
        }
    }
}