using Leak.Core.Cando.Metadata;
using Leak.Core.Client;
using Leak.Core.Client.Events;
using Leak.Core.Common;
using Leak.Core.Messages;
using System.Collections.Generic;

namespace Leak.Commands
{
    public class CompositeCallback : PeerClientCallbackBase
    {
        private readonly List<PeerClientCallback> items;

        public CompositeCallback()
        {
            items = new List<PeerClientCallback>();
        }

        public void Add(PeerClientCallback item)
        {
            items.Add(item);
        }

        public override void OnPeerConnectingTo(FileHash hash, PeerAddress peer)
        {
            foreach (PeerClientCallback item in items)
            {
                item.OnPeerConnectingTo(hash, peer);
            }
        }

        public override void OnPeerConnectingFrom(PeerHash local, PeerAddress peer)
        {
            foreach (PeerClientCallback item in items)
            {
                item.OnPeerConnectingFrom(local, peer);
            }
        }

        public override void OnPeerConnectedTo(FileHash hash, PeerClientConnected connected)
        {
            foreach (PeerClientCallback item in items)
            {
                item.OnPeerConnectedTo(hash, connected);
            }
        }

        public override void OnPeerConnectedFrom(PeerHash local, PeerClientConnected connected)
        {
            foreach (PeerClientCallback item in items)
            {
                item.OnPeerConnectedFrom(local, connected);
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

        public override void OnPieceVerified(FileHash hash, PieceVerifiedEvent @event)
        {
            foreach (PeerClientCallback item in items)
            {
                item.OnPieceVerified(hash, @event);
            }
        }

        public override void OnPieceRejected(FileHash hash, PieceRejectedEvent @event)
        {
            foreach (PeerClientCallback item in items)
            {
                item.OnPieceRejected(hash, @event);
            }
        }

        public override void OnMetadataReceived(FileHash hash, PeerHash peer, MetadataData data)
        {
            foreach (PeerClientCallback item in items)
            {
                item.OnMetadataReceived(hash, peer, data);
            }
        }

        public override void OnAnnounceStarted(FileHash hash)
        {
            foreach (PeerClientCallback item in items)
            {
                item.OnAnnounceStarted(hash);
            }
        }

        public override void OnAnnounceFailed(FileHash hash)
        {
            foreach (PeerClientCallback item in items)
            {
                item.OnAnnounceFailed(hash);
            }
        }
    }
}