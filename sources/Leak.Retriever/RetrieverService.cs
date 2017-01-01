using System;
using Leak.Common;
using Leak.Events;
using Leak.Omnibus;
using Leak.Retriever.Components;
using Leak.Retriever.Tasks;

namespace Leak.Retriever
{
    public class RetrieverService : IDisposable
    {
        private readonly RetrieverContext context;

        public RetrieverService(RetrieverParameters parameters, RetrieverDependencies dependencies, RetrieverConfiguration configuration, RetrieverHooks hooks)
        {
            context = new RetrieverContext(parameters, dependencies, configuration, hooks);
        }

        public FileHash Hash
        {
            get { return context.Parameters.Hash; }
        }

        public RetrieverHooks Hooks
        {
            get { return context.Hooks; }
        }

        public RetrieverParameters Parameters
        {
            get { return context.Parameters; }
        }

        public RetrieverDependencies Dependencies
        {
            get { return context.Dependencies; }
        }

        public RetrieverConfiguration Configuration
        {
            get { return context.Configuration; }
        }

        public void Start()
        {
            context.Dependencies.Pipeline.Register(context.Queue);
            context.Dependencies.Pipeline.Register(TimeSpan.FromMilliseconds(250), OnTick);

            context.Queue.Add(new VerifyPieceTask());
            context.Queue.Add(new FindBitfieldsTask());
        }

        public void Stop()
        {
            context.Dependencies.Pipeline.Remove(OnTick);
        }

        public void Handle(PeerChanged data)
        {
            context.Queue.Add(new HandleBitfieldTask(data));
        }

        public void Handle(BlockReceived data)
        {
            context.Queue.Add(new HandleBlockReceived(data));
        }

        public void Handle(BlockWritten data)
        {
            context.Dependencies.Omnibus.Complete(data.Block);
        }

        public void Handle(BlockReserved data)
        {
            context.Queue.Add(new RequestBlockTask(data));
        }

        public void Handle(PieceAccepted data)
        {
            context.Dependencies.Omnibus.Complete(data.Piece);
        }

        public void Handle(PieceRejected data)
        {
            context.Dependencies.Omnibus.Invalidate(data.Piece);
        }

        public void Handle(PieceReady data)
        {
            context.Dependencies.Repository.Verify(new PieceInfo(data.Piece));
        }

        private void OnTick()
        {
            context.Queue.Add(new ScheduleAllTask());
        }

        public void Dispose()
        {
            context.Dependencies.Pipeline.Remove(OnTick);
        }
    }
}