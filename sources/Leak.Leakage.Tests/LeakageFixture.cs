using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using F2F.Sandbox;
using Leak.Common;
using Leak.Metadata;

namespace Leak.Leakage.Tests
{
    public class LeakageFixture : IDisposable
    {
        public LeakageSwarm Start()
        {
            byte[] meta;
            Metainfo metainfo;
            byte[] data = Bytes.Random(20000);

            using (FileSandbox temp = new FileSandbox(new EmptyFileLocator()))
            {
                MetainfoBuilder builder = new MetainfoBuilder(temp.Directory);
                string path = temp.CreateFile("debian-8.5.0-amd64-CD-1.iso");

                File.WriteAllBytes(path, data);
                builder.AddFile(path);

                metainfo = builder.ToMetainfo(out meta);
            }

            LeakageNode sue = CreateNode(metainfo.Hash, meta);
            LeakageNode bob = CreateNode(metainfo.Hash, meta);
            LeakageNode joe = CreateNode(metainfo.Hash);

            return new LeakageSwarm(metainfo.Hash, sue, bob, joe);
        }

        private LeakageNode CreateNode(FileHash hash, byte[] metadata = null)
        {
            FileSandbox sandbox = new FileSandbox(new EmptyFileLocator());
            LeakRegistrant registrant = new LeakRegistrant
            {
                Destination = sandbox.Directory,
                Hash = hash,
                Peers = new List<PeerAddress>(),
                Trackers = new List<string>()
            };

            TaskCompletionSource<bool> onListening = new TaskCompletionSource<bool>();
            TaskCompletionSource<bool> onConnected = new TaskCompletionSource<bool>();
            LeakageEvents events = new LeakageEvents
            {
                Listening = onListening.Task,
                Connected = onConnected.Task
            };

            LeakHooks hooks = new LeakHooks();
            LeakConfiguration configuration = new LeakConfiguration
            {
                Port = LeakPort.Random
            };

            LeakClient client = new LeakClient(hooks, configuration);
            LeakageNode node = new LeakageNode(hooks, client, registrant, sandbox, events);

            hooks.OnListenerStarted += data =>
            {
                onListening.SetResult(true);
                node.Address = PeerAddress.Parse(IPAddress.Loopback, data.Port);
            };

            hooks.OnPeerConnected += data =>
            {
                onConnected?.SetResult(true);
                onConnected = null;
            };

            if (metadata != null)
            {
                File.WriteAllBytes(Path.Combine(sandbox.Directory, hash.ToString()) + ".metainfo", metadata);
            }

            return node;
        }

        public void Dispose()
        {
        }
    }
}
