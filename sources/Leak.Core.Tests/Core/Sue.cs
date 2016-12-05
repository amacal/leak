using F2F.Sandbox;
using Leak.Common;
using Leak.Core.Leakage;
using System;

namespace Leak.Core.Tests.Core
{
    public class Sue : IDisposable
    {
        private readonly LeakClient client;
        private readonly LeakConfiguration configuration;
        private readonly LeakRegistrant registrant;
        private readonly FileSandbox sandbox;

        public Sue()
        {
            LeakHooks hooks = new LeakHooks
            {
            };

            configuration = new LeakConfiguration
            {
                Port = 8091,
                Peer = PeerHash.Random()
            };

            sandbox = new FileSandbox(new EmptyFileLocator());

            registrant = new LeakRegistrant
            {
                Hash = FileHash.Random(),
                Destination = sandbox.Directory
            };

            client = new LeakClient(hooks, configuration);
            client.Register(registrant);
        }

        public FileHash Hash
        {
            get { return registrant.Hash; }
        }

        public PeerHash Peer
        {
            get { return configuration.Peer; }
        }

        public PeerAddress Endpoint
        {
            get { return new PeerAddress("127.0.0.1", configuration.Port.Value); }
        }

        public void Dispose()
        {
            client.Dispose();
            sandbox.Dispose();
        }
    }
}