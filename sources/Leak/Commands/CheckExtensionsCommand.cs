using Leak.Core;
using Leak.Core.IO;
using Leak.Core.Net;
using Pargos;
using System;
using System.Linq;
using System.Threading;
using Leak.Core.Network;

namespace Leak.Commands
{
    public class CheckExtensionsCommand : Command
    {
        public override string Name
        {
            get { return "check-extensions"; }
        }

        public override void Execute(ArgumentCollection arguments)
        {
            foreach (CheckExtensionsTask task in CheckExtensionsTaskFactory.Find(arguments))
            {
                Check(task, arguments);
            }
        }

        private void Check(CheckExtensionsTask task, ArgumentCollection arguments)
        {
            PeerNegotiator negotiator = new PeerNegotiatorEncrypted(with =>
            {
            });

            PeerClientFactory factory = new PeerClientFactory(with =>
            {
                with.Hash = task.Hash;
                with.Callback = new NegotiatorCallback();
                with.Negotiator = negotiator;
            });

            PeerAnnounce announce = new PeerAnnounce(with =>
            {
                with.Hash = task.Hash;
                with.Peer = task.Hash;

                if (arguments.Has("ip-address"))
                {
                    with.SetAddress(arguments.GetString("ip-address"));
                }
            });

            foreach (MetainfoTracker tracker in task.Trackers.Where(TrackerClientFactory.IsSupported))
            {
                try
                {
                    TrackerClient client = TrackerClientFactory.Create(tracker);
                    TrackerResonse response = client.Announce(announce);

                    foreach (TrackerResponsePeer peer in response.Peers)
                    {
                        try
                        {
                            lock (typeof(Console))
                            {
                                Console.WriteLine($"Connecting to {peer.Host}:{peer.Port}");
                                factory.Connect(peer.Host, peer.Port);
                            }

                            Thread.Sleep(TimeSpan.FromSeconds(1));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"{peer.Host}:{peer.Port} {ex.Message}");
                        }
                    }

                    Thread.Sleep(TimeSpan.FromMinutes(1));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            Thread.Sleep(TimeSpan.FromMinutes(5));
        }

        private class NegotiatorCallback : PeerNegotiatorCallback
        {
            public void OnConnect(NetworkConnection connection)
            {
            }

            public void OnTerminate(NetworkConnection connection)
            {
            }

            public void OnHandshake(NetworkConnection connection, PeerHandshake handshake)
            {
                string peer = Bytes.ToString(handshake.Peer);
                string options = String.Join(", ", Enum.GetValues(typeof(PeerHandshakeOptions)).Cast<PeerHandshakeOptions>().Where(x => x != PeerHandshakeOptions.None).Where(x => handshake.Options.HasFlag(x)).Select(x => x.ToString()));

                lock (typeof(Console))
                {
                    Console.WriteLine($"peer='{peer}'; options={options}");
                }

                handshake.Reject();
            }
        }
    }
}