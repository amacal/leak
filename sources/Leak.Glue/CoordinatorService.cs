using System;
using Leak.Common;
using Leak.Events;
using Leak.Networking.Core;

namespace Leak.Peer.Coordinator
{
    public interface CoordinatorService
    {
        FileHash Hash { get; }

        CoordinatorHooks Hooks { get; }

        CoordinatorParameters Parameters { get; }

        CoordinatorDependencies Dependencies { get; }

        CoordinatorConfiguration Configuration { get; }

        void Start();

        bool Connect(NetworkConnection connection, Handshake handshake);

        bool Disconnect(NetworkConnection connection);

        void Handle(MetafileVerified data);

        void Handle(DataVerified data);

        void SendChoke(PeerHash peer);

        void SendUnchoke(PeerHash peer);

        void SendInterested(PeerHash peer);

        void SendBitfield(PeerHash peer, Bitfield bitfield);

        void SendRequest(PeerHash peer, BlockIndex block);

        void SendPiece(PeerHash peer, BlockIndex block, DataBlock payload);

        void SendHave(PeerHash peer, int piece);

        void SendExtension(PeerHash peer, string extension, byte[] payload);

        bool IsSupported(PeerHash peer, string extension);

        void ForEachPeer(Action<PeerHash> callback);

        void ForEachPeer(Action<PeerHash, NetworkAddress> callback);

        void ForEachPeer(Action<PeerHash, NetworkAddress, NetworkDirection> callback);
    }
}