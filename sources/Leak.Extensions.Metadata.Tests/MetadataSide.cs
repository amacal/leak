using System;
using Leak.Common;
using Leak.Glue;
using Leak.Memory;

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
            FileHash hash = handshake.Hash;
            GlueConfiguration configuration = new GlueConfiguration();

            configuration.Plugins.Add(new MetadataPlugin(metadata));

            GlueFactory factory = new GlueFactory(new BufferedBlockFactory());
            GlueService service = factory.Create(hash, hooks, configuration);

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
