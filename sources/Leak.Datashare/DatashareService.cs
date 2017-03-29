using System;
using System.Collections.Generic;
using Leak.Common;
using Leak.Events;
using Leak.Tasks;

namespace Leak.Data.Share
{
    public class DatashareService : IDisposable
    {
        private readonly DatashareContext context;

        public DatashareService(DatashareParameters parameters, DatashareDependencies dependencies, DatashareConfiguration configuration, DatashareHooks hooks)
        {
            context = new DatashareContext(parameters, dependencies, configuration, hooks);
        }

        public FileHash Hash
        {
            get { return context.Parameters.Hash; }
        }

        public DatashareHooks Hooks
        {
            get { return context.Hooks; }
        }

        public DatashareParameters Parameters
        {
            get { return context.Parameters; }
        }

        public DatashareDependencies Dependencies
        {
            get { return context.Dependencies; }
        }

        public DatashareConfiguration Configuration
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

            context.Queue.Add(new DatashareTaskInterested());
        }

        public void Handle(BlockRequested data)
        {
            context.Queue.Add(() =>
            {
                context.Collection.Register(data.Peer, data.Block);
                context.Dependencies.DataStore.Read(data.Block);
            });
        }

        public void Handle(BlockRead data)
        {
            context.Queue.Add(() =>
            {
                IList<DatashareEntry> entries = context.Collection.RemoveAll(data.Block);

                for (int i = 1; i < entries.Count; i++)
                {
                    context.Dependencies.Glue.SendPiece(entries[i].Peer, data.Block, data.Payload);
                }

                if (entries.Count > 0)
                {
                    context.Dependencies.Glue.SendPiece(entries[0].Peer, data.Block, data.Payload);
                    context.Hooks.CallBlockSent(context.Parameters.Hash, entries[0].Peer, data.Block);
                }
            });
        }

        private void OnTick250()
        {
        }

        private void OnTick5000()
        {
            context.Queue.Add(new DatashareTaskInterested());
        }

        public void Dispose()
        {
            context.Dependencies.Pipeline.Remove(OnTick250);
            context.Dependencies.Pipeline.Remove(OnTick5000);
        }
    }
}