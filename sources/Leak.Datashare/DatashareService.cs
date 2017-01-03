using System;
using System.Collections.Generic;
using Leak.Common;
using Leak.Events;

namespace Leak.Datashare
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
        }

        public void Stop()
        {
        }

        public void Handle(BlockRequested data)
        {
            context.Collection.Register(data.Peer, data.Block);
            context.Dependencies.Repository.Read(data.Block);
        }

        public void Handle(BlockRead data)
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
        }

        public void Dispose()
        {
        }
    }
}
