using Leak.Core.Cando.Metadata;
using Leak.Core.Client.Events;
using Leak.Core.Common;
using System;

namespace Leak.Loggers
{
    public class PeerLoggerNormal : PeerLogger
    {
        public override void OnPeerRejected(FileHash hash, PeerAddress peer)
        {
            Console.WriteLine($"{hash}: rejected; endpoint={peer}");
        }

        public override void OnPeerHandshake(FileHash hash, PeerEndpoint endpoint)
        {
            string remote = endpoint.Remote.ToString();
            string direction = endpoint.Direction.ToString().ToLowerInvariant();

            Console.WriteLine($"{endpoint.Session.Peer}: handshake; remote={remote}; direction={direction}");
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

        public override void OnPieceVerified(FileHash hash, PieceVerifiedEvent @event)
        {
            Console.WriteLine($"{hash}: verified; piece={@event.Piece.Index}");
        }

        public override void OnPieceRejected(FileHash hash, PieceRejectedEvent @event)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{hash}: rejected; piece={@event.Piece.Index}");
            Console.ResetColor();
        }

        public override void OnMetadataReceived(FileHash hash, PeerHash peer, MetadataData data)
        {
            Console.WriteLine($"{hash}: metadata; piece={data.Block}; total={data.Size}");
        }
    }
}