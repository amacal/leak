using System;
using Leak.Common;
using Leak.Data.Map;
using Leak.Events;
using Leak.Peer.Coordinator.Events;
using Leak.Tasks;

namespace Leak.Data.Get
{
    public class DataGetService : IDisposable
    {
        private readonly DataGetContext context;

        public DataGetService(DataGetParameters parameters, DataGetDependencies dependencies, DataGetConfiguration configuration, DataGetHooks hooks)
        {
            context = new DataGetContext(parameters, dependencies, configuration, hooks);
        }

        public FileHash Hash
        {
            get { return context.Parameters.Hash; }
        }

        public DataGetHooks Hooks
        {
            get { return context.Hooks; }
        }

        public DataGetParameters Parameters
        {
            get { return context.Parameters; }
        }

        public DataGetDependencies Dependencies
        {
            get { return context.Dependencies; }
        }

        public DataGetConfiguration Configuration
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

        public void Handle(DataVerified data)
        {
            context.Queue.Add(() =>
            {
                context.Verified = true;
            });

            context.Queue.Add(new DataGetTaskInterested());
        }

        public void Handle(BlockReceived data)
        {
            context.Queue.Add(() =>
            {
                if (context.Dependencies.DataMap.IsComplete(data.Block.Piece) == false)
                {
                    context.Dependencies.DataStore.Write(data.Block, data.Payload);
                    context.Hooks.CallBlockHandled(data.Hash, data.Peer, data.Block);
                }
                else
                {
                    data.Payload.Release();
                }
            });
        }

        public void Handle(BlockWritten data)
        {
            context.Queue.Add(() =>
            {
                context.Dependencies.DataMap.Complete(data.Block);
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
                context.Dependencies.DataMap.Complete(data.Piece);
            });
        }

        public void Handle(PieceRejected data)
        {
            context.Queue.Add(() =>
            {
                context.Dependencies.DataMap.Invalidate(data.Piece);
            });
        }

        public void Handle(PieceReady data)
        {
            context.Queue.Add(() =>
            {
                context.Dependencies.DataStore.Verify(data.Piece);
            });
        }

        public void Handle(ThresholdReached data)
        {
            context.Queue.Add(new DataGetTaskSchedule(data.Peer));
        }

        private void OnTick250()
        {
            context.Queue.Add(new DataGetTaskScheduleAll());
        }

        private void OnTick5000()
        {
            context.Queue.Add(new DataGetTaskInterested());
        }

        public void Dispose()
        {
            context.Dependencies.Pipeline.Remove(OnTick250);
            context.Dependencies.Pipeline.Remove(OnTick5000);
        }
    }
}