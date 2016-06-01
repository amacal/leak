using Leak.Core.IO;
using Leak.Core.Net;
using Pargos;
using System;
using System.Linq;
using System.Threading;

namespace Leak.Commands
{
    public class GetMetadataCommand : Command
    {
        public override string Name
        {
            get { return "get-metadata"; }
        }

        public override void Execute(ArgumentCollection arguments)
        {
            foreach (CheckExtensionsTask task in CheckExtensionsTaskFactory.Find(arguments))
            {
                GetMetadata(task, arguments);
            }
        }

        private void GetMetadata(CheckExtensionsTask task, ArgumentCollection arguments)
        {
            PeerNegotiator negotiator = new PeerNegotiatorEncrypted(with =>
            {
            });

            PeerClientFactory factory = new PeerClientFactory(with =>
            {
                with.Hash = task.Hash;
                with.Callback = new NegotiatorCallback();
                with.Negotiator = negotiator;
                with.Options = PeerHandshakeOptions.Extended;
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
            public void OnConnect(PeerConnection connection)
            {
            }

            public void OnTerminate(PeerConnection connection)
            {
            }

            public void OnHandshake(PeerConnection connection, PeerHandshake handshake)
            {
                if (handshake.Options.HasFlag(PeerHandshakeOptions.Extended))
                {
                    handshake.Accept(new DownloadCallback());
                }
            }
        }

        private class DownloadCallback : PeerCallback
        {
            public override void OnAttached(PeerChannel channel)
            {
            }

            public override void OnBitfield(PeerChannel channel, PeerBitfield message)
            {
            }

            public override void OnHave(PeerChannel channel, PeerHave message)
            {
            }

            public override void OnInterested(PeerChannel channel, PeerInterested message)
            {
            }

            public override void OnKeepAlive(PeerChannel channel)
            {
            }

            public override void OnPiece(PeerChannel channel, PeerPiece message)
            {
            }

            public override void OnTerminate(PeerChannel channel)
            {
            }

            public override void OnUnchoke(PeerChannel channel, PeerUnchoke message)
            {
            }

            public override void OnExtended(PeerChannel channel, PeerExtended message)
            {
            }
        }
    }
}