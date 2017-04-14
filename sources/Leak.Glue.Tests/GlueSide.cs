using System;
using Leak.Common;
using Leak.Networking.Core;

namespace Leak.Peer.Coordinator.Tests
{
    public class GlueSide : IDisposable
    {
        private readonly NetworkConnection connection;
        private readonly Handshake handshake;
        private readonly CoordinatorHooks hooks;

        public GlueSide(NetworkConnection connection, Handshake handshake)
        {
            this.connection = connection;
            this.handshake = handshake;
            this.hooks = new CoordinatorHooks();
        }

        public PeerHash Peer
        {
            get { return handshake.Remote; }
        }

        public Handshake Handshake
        {
            get { return handshake; }
        }

        public GlueInstance Build(params string[] plugins)
        {
            FileHash hash = FileHash.Random();
            CoordinatorBuilder builder = new CoordinatorBuilder();

            foreach (string plugin in plugins)
            {
                builder.WithPlugin(new GlueNamedPlugin(plugin));
            }

            CoordinatorService service =
                builder
                    .WithHash(hash)
                    .WithMemory(new GlueMemory())
                    .WithDefinition(new GlueMessages())
                    .Build(hooks);

            return new GlueInstance(service);
        }

        public CoordinatorHooks Hooks
        {
            get { return hooks; }
        }

        public NetworkConnection Connection
        {
            get { return connection; }
        }

        public void Dispose()
        {
        }
    }
}