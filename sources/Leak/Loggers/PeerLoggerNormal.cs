using Leak.Core.Client;
using Leak.Core.Common;
using Leak.Core.Extensions.Metadata;
using Leak.Core.Messages;
using System;

namespace Leak.Loggers
{
    public class PeerLoggerNormal : PeerLogger
    {
        public override void OnPeerRejected(FileHash hash, string endpoint)
        {
            Console.WriteLine($"{hash}: rejected; endpoint={endpoint}");
        }

        public override void OnPeerHandshake(FileHash hash, PeerEndpoint endpoint)
        {
            string remote = endpoint.Remote;
            string direction = endpoint.Direction.ToString().ToLowerInvariant();

            Console.WriteLine($"{endpoint.Peer}: handshake; remote={remote}; direction={direction}");
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