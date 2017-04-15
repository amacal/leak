using System;
using System.Collections.Generic;
using Leak.Common;
using Leak.Events;
using Leak.Networking.Core;
using Leak.Peer.Coordinator.Events;
using Leak.Tasks;

namespace Leak.Data.Share
{
    public class DataShareService : IDisposable
    {
        private readonly DataShareContext context;

        public DataShareService(DataShareParameters parameters, DataShareDependencies dependencies, DataShareConfiguration configuration, DataShareHooks hooks)
        {
            context = new DataShareContext(parameters, dependencies, configuration, hooks);
        }

        public FileHash Hash
        {
            get { return context.Parameters.Hash; }
        }

        public DataShareHooks Hooks
        {
            get { return context.Hooks; }
        }

        public DataShareParameters Parameters
        {
            get { return context.Parameters; }
        }

        public DataShareDependencies Dependencies
        {
            get { return context.Dependencies; }
        }

        public DataShareConfiguration Configuration
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

            context.Queue.Add(new DataShareTaskInterested());
        }

        public void Handle(BlockRequested data)
        {
            context.Queue.Add(() =>
            {
                if (context.Collection.Register(data.Peer, data.Block))
                {
                    context.Dependencies.DataStore.Read(data.Block);
                }
            });
        }

        public void Handle(BlockRead data)
        {
            context.Queue.Add(() =>
            {
                IList<DataShareEntry> entries = context.Collection.RemoveAll(data.Block);
                DataBlock payload = data.Payload.Shared(entries.Count);

                for (int i = 1; i < entries.Count; i++)
                {
                    context.Dependencies.Glue.SendPiece(entries[i].Peer, data.Block, payload);
                    context.Hooks.CallBlockSent(context.Parameters.Hash, entries[i].Peer, data.Block);
                }

                if (entries.Count > 0)
                {
                    context.Dependencies.Glue.SendPiece(entries[0].Peer, data.Block, payload);
                    context.Hooks.CallBlockSent(context.Parameters.Hash, entries[0].Peer, data.Block);
                }
                else
                {
                    data.Payload.Release();
                }
            });
        }

        private void OnTick250()
        {
        }

        private void OnTick5000()
        {
            context.Queue.Add(new DataShareTaskInterested());
        }

        public void Dispose()
        {
            context.Dependencies.Pipeline.Remove(OnTick250);
            context.Dependencies.Pipeline.Remove(OnTick5000);
        }
    }
}