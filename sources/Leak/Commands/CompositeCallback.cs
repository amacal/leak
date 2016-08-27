using Leak.Core.Cando.Metadata;
using Leak.Core.Client;
using Leak.Core.Common;
using Leak.Core.Messages;
using System.Collections.Generic;

namespace Leak.Commands
{
    public class CompositeCallback : PeerClientCallbackBase
    {
        private List<PeerClientCallback> items;

        public CompositeCallback()
        {
            items = new List<PeerClientCallback>();
        }

        public void Add(PeerClientCallback item)
        {
            items.Add(item);
        }

        public override void OnFileScheduled(FileHash hash)
        {
            foreach (PeerClientCallback item in items)
            {
                item.OnFileScheduled(hash);
            }
        }

        public override void OnFileDiscovered(FileHash hash)
        {
            foreach (PeerClientCallback item in items)
            {
                item.OnFileDiscovered(hash);
            }
        }

        public override void OnFileInitialized(FileHash hash, PeerClientMetainfo summary)
        {
            foreach (PeerClientCallback item in items)
            {
                item.OnFileInitialized(hash, summary);
            }
        }

        public override void OnFileStarted(FileHash hash)
        {
            foreach (PeerClientCallback item in items)
            {
                item.OnFileStarted(hash);
            }
        }

        public override void OnFileCompleted(FileHash hash)
        {
            foreach (PeerClientCallback item in items)
            {
                item.OnFileCompleted(hash);
            }
        }

        public override void OnPeerConnecting(FileHash hash, PeerAddress peer)
        {
            foreach (PeerClientCallback item in items)
            {
                item.OnPeerConnecting(hash, peer);
            }
        }

        public override void OnPeerConnected(FileHash hash, PeerClientConnected connected)
        {
            foreach (PeerClientCallback item in items)
            {
                item.OnPeerConnected(hash, connected);
            }
        }

        public override void OnPeerRejected(FileHash hash, PeerAddress peer)
        {
            foreach (PeerClientCallback item in items)
            {
                item.OnPeerRejected(hash, peer);
            }
        }

        public override void OnPeerDisconnected(FileHash hash, PeerHash peer)
        {
            foreach (PeerClientCallback item in items)
            {
                item.OnPeerDisconnected(hash, peer);
            }
        }

        public override void OnPeerHandshake(FileHash hash, PeerEndpoint endpoint)
        {
            foreach (PeerClientCallback item in items)
            {
                item.OnPeerHandshake(hash, endpoint);
            }
        }

        public override void OnPeerIncomingMessage(FileHash hash, PeerHash peer, PeerClientMessage message)
        {
            foreach (PeerClientCallback item in items)
            {
                item.OnPeerIncomingMessage(hash, peer, message);
            }
        }

        public override void OnPeerOutgoingMessage(FileHash hash, PeerHash peer, PeerClientMessage message)
        {
            foreach (PeerClientCallback item in items)
            {
                item.OnPeerOutgoingMessage(hash, peer, message);
            }
        }

        public override void OnPeerBitfield(FileHash hash, PeerHash peer, Bitfield bitfield)
        {
            foreach (PeerClientCallback item in items)
            {
                item.OnPeerBitfield(hash, peer, bitfield);
            }
        }

        public override void OnPeerChoked(FileHash hash, PeerHash peer)
        {
            foreach (PeerClientCallback item in items)
            {
                item.OnPeerChoked(hash, peer);
            }
        }

        public override void OnPeerUnchoked(FileHash hash, PeerHash peer)
        {
            foreach (PeerClientCallback item in items)
            {
                item.OnPeerUnchoked(hash, peer);
            }
        }

        public override void OnBlockReceived(FileHash hash, PeerHash peer, Piece piece)
        {
            foreach (PeerClientCallback item in items)
            {
                item.OnBlockReceived(hash, peer, piece);
            }
        }

        public override void OnPieceVerified(FileHash hash, PeerClientPieceVerification verification)
        {
            foreach (PeerClientCallback item in items)
            {
                item.OnPieceVerified(hash, verification);
            }
        }

        public override void OnMetadataReceived(FileHash hash, PeerHash peer, MetadataData data)
        {
            foreach (PeerClientCallback item in items)
            {
                item.OnMetadataReceived(hash, peer, data);
            }
        }
    }
}