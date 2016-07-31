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
            Logging logging = GetLoggingValue(arguments.GetString("logging"));

            bool listen = arguments.GetString("listener") == "on";
            bool connect = arguments.GetString("connector") != "off";

            string portText = arguments.GetString("port");
            int? portValue = String.IsNullOrEmpty(portText) ? default(int?) : Int32.Parse(portText);

            PeerClient client = new PeerClient(with =>
            {
                with.Destination = destination;
                with.Callback = new Callback(handle, logging);
                with.Extensions.Metadata();

                if (listen)
                {
                    with.Listener.Enable(listener =>
                    {
                        if (portValue != null)
                        {
                            listener.Port = portValue.Value;
                        }
                    });
                }

                if (connect)
                {
                    with.Connector.Enable();
                }
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

        private static Logging GetLoggingValue(string value)
        {
            Logging found;
            Logging result = Logging.Default;

            if (Enum.TryParse(value, true, out found))
            {
                result = found;
            }

            return result;
        }

        [Flags]
        public enum Logging
        {
            None = 0,
            Default = 1,
            Blocks = 2
        }

        public class Callback : PeerClientCallbackBase
        {
            private readonly ManualResetEvent handle;
            private readonly Logging logging;

            public Callback(ManualResetEvent handle, Logging logging)
            {
                this.handle = handle;
                this.logging = logging;
            }

            public override void OnInitialized(FileHash hash, PeerClientMetainfo summary)
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

            public override void OnPeerConnected(FileHash hash, string endpoint)
            {
                Console.WriteLine($"{hash}: connected; endpoint={endpoint}");
            }

            public override void OnPeerRejected(FileHash hash, string endpoint)
            {
                Console.WriteLine($"{hash}: rejected; endpoint={endpoint}");
            }

            public override void OnPeerDisconnected(FileHash hash, PeerHash peer)
            {
                Console.WriteLine($"{peer}: disconnected");
            }

            public override void OnPeerHandshake(FileHash hash, PeerEndpoint endpoint)
            {
                string remote = endpoint.Remote;
                string direction = endpoint.Direction.ToString().ToLowerInvariant();

                Console.WriteLine($"{endpoint.Peer}: handshake; remote={remote}; direction={direction}");
            }

            public override void OnPeerIncomingMessage(FileHash hash, PeerHash peer, PeerClientMessage message)
            {
                if (logging.HasFlag(Logging.Blocks) && message.Type == "piece")
                {
                    Console.WriteLine($"{peer}: incoming-message; type={message.Type}; size={message.Size}");
                }
            }

            public override void OnPeerOutgoingMessage(FileHash hash, PeerHash peer, PeerClientMessage message)
            {
                if (logging.HasFlag(Logging.Blocks) && message.Type == "request")
                {
                    Console.WriteLine($"{peer}: outgoing-message; type={message.Type}; size={message.Size}");
                }
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
                if (logging.HasFlag(Logging.Blocks))
                {
                    Console.WriteLine($"{peer}: block; piece={piece.Index}; offset={piece.Offset}; size={piece.Size}");
                }
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