using Leak.Common;
using Leak.Completion;
using Leak.Core.Glue;
using Leak.Core.Glue.Extensions.Metadata;
using Leak.Networking;
using Leak.Tasks;
using System;
using System.Collections.Generic;
using Leak.Connector;
using Leak.Listener;
using Leak.Testing;

namespace Leak.Core.Tests.Core
{
    public class Swarm : IDisposable
    {
        private readonly FileHash hash;

        private readonly NetworkPool pool;
        private readonly CompletionThread worker;
        private readonly LeakPipeline pipeline;

        private readonly Dictionary<string, SwarmEntry> entries;
        private readonly List<IDisposable> disposables;

        public Swarm(FileHash hash)
        {
            this.hash = hash;

            worker = new CompletionThread();
            pipeline = new LeakPipeline();

            pool = new NetworkPoolFactory(pipeline, worker).CreateInstance(new NetworkPoolHooks());
            entries = new Dictionary<string, SwarmEntry>();
            disposables = new List<IDisposable>();
        }

        public SwarmEntry this[string name]
        {
            get { return entries[name]; }
        }

        public void Listen(string name)
        {
            SwarmEntry entry = new SwarmEntry
            {
                Hooks = new GlueHooks(),
                Metadata = new MetadataHooks(),
                Peer = PeerHash.Random()
            };

            GlueConfiguration configuration = new GlueConfiguration
            {
                Plugins = new List<GluePlugin>
                {
                    new MetadataPlugin(entry.Metadata)
                }
            };

            DataBlockFactory blocks = new BufferedBlockFactory();
            GlueFactory factory = new GlueFactory(blocks);

            PeerListenerHooks listenerHooks = new PeerListenerHooks
            {
                //OnHandshakeCompleted = data =>
                //{
                //    entry.Glue = factory.Create(hash, entry.Hooks, configuration);
                //    entry.Glue.Connect(data.Connection, data.Handshake);
                //},
                OnListenerStarted = data =>
                {
                    entry.Port = data.Port;
                }
            };

            PeerListenerConfiguration listenerConfiguration = new PeerListenerConfiguration
            {
                Peer = entry.Peer,
                Port = new PeerListenerPortRandom()
            };

            PeerListener listener = new PeerListener(pool, listenerHooks, listenerConfiguration);

            entry.Exchanged = entry.Hooks.OnExtensionListReceived.Trigger();
            entry.Hooks.OnExtensionListReceived = entry.Exchanged;

            entry.Connected = entry.Hooks.OnPeerConnected.Trigger();
            entry.Hooks.OnPeerConnected = entry.Connected;

            entries.Add(name, entry);

            disposables.Add(listener);
            listener.Enable(hash);
            listener.Start();
        }

        public void Connect(string name, string other)
        {
            SwarmEntry entry = new SwarmEntry
            {
                Hooks = new GlueHooks(),
                Metadata = new MetadataHooks(),
                Peer = PeerHash.Random()
            };

            GlueConfiguration configuration = new GlueConfiguration
            {
                Plugins = new List<GluePlugin>
                {
                    new MetadataPlugin(entry.Metadata)
                }
            };

            DataBlockFactory blocks = new BufferedBlockFactory();
            GlueFactory factory = new GlueFactory(blocks);

            PeerConnectorHooks connectorHooks = new PeerConnectorHooks
            {
                //OnHandshakeCompleted = data =>
                //{
                //    entry.Glue = factory.Create(hash, entry.Hooks, configuration);
                //    entry.Glue.Connect(data.Connection, data.Handshake);
                //}
            };

            PeerConnectorConfiguration connectorConfiguration = new PeerConnectorConfiguration
            {
                Peer = entry.Peer
            };

            PeerConnector connector = new PeerConnector(pool, connectorHooks, connectorConfiguration);

            entry.Exchanged = entry.Hooks.OnExtensionListReceived.Trigger();
            entry.Hooks.OnExtensionListReceived = entry.Exchanged;

            entry.Connected = entry.Hooks.OnPeerConnected.Trigger();
            entry.Hooks.OnPeerConnected = entry.Connected;

            entries.Add(name, entry);
            connector.Start(pipeline);
            connector.ConnectTo(hash, new PeerAddress("127.0.0.1", entries[other].Port.Value));
        }

        public void Start()
        {
            pipeline.Start();
            worker.Start();
            pool.Start();
        }

        public void Dispose()
        {
            worker.Dispose();
            pipeline.Stop();

            foreach (IDisposable disposable in disposables)
            {
                disposable.Dispose();
            }
        }
    }
}