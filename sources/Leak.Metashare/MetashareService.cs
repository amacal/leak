using System;
using Leak.Common;
using Leak.Events;
using Leak.Extensions.Metadata;

namespace Leak.Meta.Share
{
    public class MetashareService : IDisposable
    {
        private readonly MetashareContext context;

        public MetashareService(MetashareParameters parameters, MetashareDependencies dependencies, MetashareConfiguration configuration, MetashareHooks hooks)
        {
            context = new MetashareContext(parameters, dependencies, configuration, hooks);
        }

        public FileHash Hash
        {
            get { return context.Parameters.Hash; }
        }

        public MetashareHooks Hooks
        {
            get { return context.Hooks; }
        }

        public MetashareParameters Parameters
        {
            get { return context.Parameters; }
        }

        public MetashareDependencies Dependencies
        {
            get { return context.Dependencies; }
        }

        public MetashareConfiguration Configuration
        {
            get { return context.Configuration; }
        }

        public void Start()
        {
            context.Dependencies.Pipeline.Register(context.Queue);
        }

        public void Stop()
        {
        }

        public void Handle(MetadataRequested data)
        {
            context.Collection.Register(data.Peer, data.Piece);
            context.Dependencies.Metafile.Read(data.Piece);
        }

        public void Handle(MetafileRead data)
        {
            foreach (MetashareEntry entry in context.Collection.Remove(data.Piece))
            {
                context.Dependencies.Glue.SendMetadataPiece(entry.Peer, entry.Piece, data.Total, data.Payload);
                context.Hooks.CallMetadataShared(context.Parameters.Hash, entry.Peer, entry.Piece);
            }
        }

        public void Dispose()
        {
        }
    }
}