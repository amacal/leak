using System;
using System.Collections.Generic;
using Leak.Completion;
using Leak.Core.Common;
using Leak.Core.Connector;
using Leak.Core.Core;
using Leak.Core.Glue;
using Leak.Core.Glue.Extensions.Metadata;
using Leak.Core.Listener;
using Leak.Core.Messages;
using Leak.Core.Network;

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

            pool = new NetworkPool(worker, new NetworkPoolHooks());
            entries = new Dictionary<string, SwarmEntry>();
            disposables = new List<IDisposable>();
        }

        public SwarmEntry this[string name]
        {
            get { return entries[name]; }
        }

        public void Listen(string name, int port)
        {
            SwarmEntry entry = new SwarmEntry
            {
                Hooks = new GlueHooks(),
                Metadata = new MetadataHooks()
            };

            GlueConfiguration configuration = new GlueConfiguration
            {
                Plugins = new List<GluePlugin>
                {
                    new MetadataPlugin(entry.Metadata)
                }
            };

            DataBlockFactory blocks = new BufferedBlockFactory();
            GlueFactory factory = new GlueFactory(blocks, configuration);

            PeerListenerHooks listenerHooks = new PeerListenerHooks
            {
                OnHandshakeCompleted = data =>
                {
                    entry.Glue = factory.Create(hash, entry.Hooks);
                    entry.Glue.Connect(data.Connection, data.Handshake);
                }
            };

            PeerListenerConfiguration listenerConfiguration = new PeerListenerConfiguration
            {
                Port = port,
                Peer = PeerHash.Random(),
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

        public void Connect(string name, int port)
        {
            SwarmEntry entry = new SwarmEntry
            {
                Hooks = new GlueHooks(),
                Metadata = new MetadataHooks()
            };

            GlueConfiguration configuration = new GlueConfiguration
            {
                Plugins = new List<GluePlugin>
                {
                    new MetadataPlugin(entry.Metadata)
                }
            };

            DataBlockFactory blocks = new BufferedBlockFactory();
            GlueFactory factory = new GlueFactory(blocks, configuration);

            PeerConnectorHooks connectorHooks = new PeerConnectorHooks
            {
                OnHandshakeCompleted = data =>
                {
                    entry.Glue = factory.Create(hash, entry.Hooks);
                    entry.Glue.Connect(data.Connection, data.Handshake);
                }
            };

            PeerConnectorConfiguration connectorConfiguration = new PeerConnectorConfiguration
            {
                Peer = PeerHash.Random()
            };

            PeerConnector connector = new PeerConnector(pool, connectorHooks, connectorConfiguration);

            entry.Exchanged = entry.Hooks.OnExtensionListReceived.Trigger();
            entry.Hooks.OnExtensionListReceived = entry.Exchanged;

            entry.Connected = entry.Hooks.OnPeerConnected.Trigger();
            entry.Hooks.OnPeerConnected = entry.Connected;

            entries.Add(name, entry);
            connector.Start(pipeline);
            connector.ConnectTo(hash, new PeerAddress("127.0.0.1", port));
        }

        public void Start()
        {
            pipeline.Start();
            worker.Start();
            pool.Start(pipeline);
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