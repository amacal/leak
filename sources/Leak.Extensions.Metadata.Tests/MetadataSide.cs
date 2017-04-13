using Leak.Common;
using Leak.Memory;
using System;
using Leak.Networking.Core;
using Leak.Peer.Coordinator;

namespace Leak.Extensions.Metadata.Tests
{
    public class MetadataSide : IDisposable
    {
        private readonly NetworkConnection connection;
        private readonly Handshake handshake;
        private readonly MetadataHooks metadata;
        private readonly GlueHooks hooks;

        public MetadataSide(NetworkConnection connection, Handshake handshake)
        {
            this.connection = connection;
            this.handshake = handshake;

            this.metadata = new MetadataHooks();
            this.hooks = new GlueHooks();
        }

        public PeerHash Peer
        {
            get { return handshake.Remote; }
        }

        public Handshake Handshake
        {
            get { return handshake; }
        }

        public MetadataInstance Build()
        {
            GlueService service =
                new GlueBuilder()
                    .WithHash(handshake.Hash)
                    .WithMemory(new MemoryBuilder().Build())
                    .WithPlugin(new MetadataPlugin(metadata))
                    .WithDefinition(new MetadataMessages())
                    .Build(hooks);

            return new MetadataInstance(service);
        }

        public MetadataHooks Metadata
        {
            get { return metadata; }
        }

        public NetworkConnection Connection
        {
            get { return connection; }
        }

        public GlueHooks Hooks
        {
            get { return hooks; }
        }

        public void Dispose()
        {
        }
    }
}