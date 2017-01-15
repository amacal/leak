using System;
using Leak.Common;
using Leak.Events;
using Leak.Tasks;

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
            context.Dependencies.Pipeline.Register(250, OnTick250);
            context.Dependencies.Pipeline.Register(5000, OnTick5000);
        }

        public void Stop()
        {
            context.Dependencies.Pipeline.Remove(OnTick250);
            context.Dependencies.Pipeline.Remove(OnTick5000);
        }

        public void Handle(BlockReceived data)
        {
            context.Queue.Add(() =>
            {
                if (context.Dependencies.Omnibus.IsComplete(data.Block.Piece) == false)
                {
                    context.Dependencies.Repository.Write(data.Block, data.Payload);
                    context.Hooks.CallBlockHandled(data.Hash, data.Peer, data.Block);
                }
            });
        }

        public void Handle(BlockWritten data)
        {
            context.Queue.Add(() =>
            {
                context.Dependencies.Omnibus.Complete(data.Block);
            });
        }

        public void Handle(BlockReserved data)
        {
            context.Queue.Add(() =>
            {
                context.Dependencies.Glue.SendRequest(data.Peer, data.Block);
                context.Hooks.CallBlockRequested(data.Hash, data.Peer, data.Block);
            });
        }

        public void Handle(PieceAccepted data)
        {
            context.Queue.Add(() =>
            {
                context.Dependencies.Omnibus.Complete(data.Piece);
            });
        }

        public void Handle(PieceRejected data)
        {
            context.Queue.Add(() =>
            {
                context.Dependencies.Omnibus.Invalidate(data.Piece);
            });
        }

        public void Handle(PieceReady data)
        {
            context.Queue.Add(() =>
            {
                context.Dependencies.Repository.Verify(data.Piece);
            });
        }

        private void OnTick250()
        {
            context.Queue.Add(new RetrieverTaskScheduleAll());
        }

        private void OnTick5000()
        {
            context.Queue.Add(new RetrieverTaskInterested());
        }

        public void Dispose()
        {
            context.Dependencies.Pipeline.Remove(OnTick250);
            context.Dependencies.Pipeline.Remove(OnTick5000);
        }
    }
}