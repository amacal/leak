using F2F.Sandbox;
using Leak.Common;
using Leak.Completion;
using Leak.Core.Core;
using Leak.Files;
using System;
using System.IO;

namespace Leak.Core.Tests.Core
{
    public class Environment : IDisposable
    {
        private readonly MetadataFixture fixture;

        public readonly FileSandbox Sandbox;
        public readonly CompletionThread Worker;
        public readonly LeakPipeline Pipeline;
        public readonly FileFactory Files;
        public readonly SwarmEnvironment Peers;
        public readonly string Destination;

        public Environment(MetadataFixture fixture)
        {
            this.fixture = fixture;

            Sandbox = new FileSandbox(new EmptyFileLocator());
            Worker = new CompletionThread();
            Pipeline = new LeakPipeline();
            Files = new FileFactory(Worker);
            Destination = Path.Combine(Sandbox.Directory, fixture.Hash.ToString());
            Peers = new SwarmEnvironment(this);

            Worker.Start();
            Pipeline.Start();
        }

        public void Dispose()
        {
            Worker.Dispose();
            Pipeline.Stop();
            Peers.Dispose();
            Sandbox.Dispose();
        }

        public void CompleteMetainfoFile()
        {
            Sandbox.CreateFile($"{fixture.Hash}.metainfo", fixture.Content);
        }

        public class SwarmEnvironment : IDisposable
        {
            public readonly Swarm Swarm;

            public readonly PeerEnvironment Bob;
            public readonly PeerEnvironment Sue;

            public SwarmEnvironment(Environment environment)
            {
                Swarm = new Swarm(environment.fixture.Hash);
                Swarm.Start();

                Swarm.Listen("bob");
                Swarm.Connect("sue", "bob");

                Swarm["bob"].Exchanged.Wait();
                Swarm["sue"].Exchanged.Wait();

                Bob = new PeerEnvironment
                {
                    Entry = Swarm["bob"],
                    Hash = Swarm["bob"].Peer
                };

                Sue = new PeerEnvironment
                {
                    Entry = Swarm["sue"],
                    Hash = Swarm["sue"].Peer
                };
            }

            public void Dispose()
            {
                Swarm.Dispose();
            }

            public class PeerEnvironment
            {
                public PeerHash Hash;
                public SwarmEntry Entry;
            }
        }
    }
}