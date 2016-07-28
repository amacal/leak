using Leak.Core.Client;
using Leak.Core.Common;
using Leak.Core.Extensions.Metadata;
using Leak.Core.Messages;
using Leak.Core.Metadata;
using Pargos;
using System;
using System.Threading;

namespace Leak.Commands
{
    public class DownloadCommand
    {
        private readonly ArgumentCollection arguments;

        public DownloadCommand(ArgumentCollection arguments)
        {
            this.arguments = arguments;
        }

        public void Execute()
        {
            string destination = arguments.GetString("destination");
            string torrent = arguments.GetString("torrent");

            string hash = arguments.GetString("hash");
            int trackers = arguments.Count("tracker");

            ManualResetEvent handle = new ManualResetEvent(false);
            PeerClient client = new PeerClient(with =>
            {
                with.Destination = destination;
                with.Callback = new Callback(handle);
                with.Extensions.Metadata();
            });

            if (torrent != null)
            {
                client.Start(MetainfoFactory.FromFile(torrent));
            }
            else if (hash != null)
            {
                client.Start(with =>
                {
                    with.Hash = FileHash.Parse(hash);

                    for (int i = 0; i < trackers; i++)
                    {
                        with.Trackers.Add(arguments.GetString("tracker", i));
                    }
                });
            }

            handle.WaitOne();
        }

        public class Callback : PeerClientCallbackBase
        {
            private readonly ManualResetEvent handle;

            public Callback(ManualResetEvent handle)
            {
                this.handle = handle;
            }

            public override void OnInitialized(FileHash hash, PeerClientMetainfoSummary summary)
            {
                Console.WriteLine($"{hash}: initialized; completed={summary.Completed}; total={summary.Total}");
            }

            public override void OnCompleted(FileHash hash)
            {
                Console.WriteLine($"{hash}: completed");
                handle.Set();
            }

            public override void OnPeerConnecting(FileHash hash, string endpoint)
            {
                Console.WriteLine($"{hash}: connecting; endpoint={endpoint}");
            }

            public override void OnPeerConnected(FileHash hash, PeerEndpoint endpoint)
            {
                Console.WriteLine($"{endpoint.Peer}: connected; remote={endpoint.Remote}");
            }

            public override void OnPeerDisconnected(FileHash hash, PeerHash peer)
            {
                Console.WriteLine($"{peer}: disconnected");
            }

            public override void OnPeerBitfield(FileHash hash, PeerHash peer, Bitfield bitfield)
            {
                Console.WriteLine($"{peer}: bitfield; total={bitfield.Length}; completed={bitfield.Completed}");
            }

            public override void OnPeerChoked(FileHash hash, PeerHash peer)
            {
                Console.WriteLine($"{peer}: choke");
            }

            public override void OnPeerUnchoked(FileHash hash, PeerHash peer)
            {
                Console.WriteLine($"{peer}: unchoke");
            }

            public override void OnBlockReceived(FileHash hash, PeerHash peer, Piece piece)
            {
                Console.WriteLine($"{peer}: block; piece={piece.Index}; offset={piece.Offset}; size={piece.Size}");
            }

            public override void OnPieceVerified(FileHash hash, PeerClientPieceVerification verification)
            {
                Console.WriteLine($"{hash}: verified; piece={verification.Piece}");
            }

            public override void OnMetadataReceived(FileHash hash, PeerHash peer, MetadataData data)
            {
                Console.WriteLine($"{hash}: metadata; piece={data.Piece}; total={data.Size}");
            }
        }
    }
}