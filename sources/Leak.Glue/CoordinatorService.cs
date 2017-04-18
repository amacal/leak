using System;
using Leak.Common;
using Leak.Events;
using Leak.Extensions;
using Leak.Networking.Core;
using Leak.Peer.Coordinator.Core;
using Leak.Peer.Receiver.Events;

namespace Leak.Peer.Coordinator
{
    public class CoordinatorService
    {
        private readonly CoordinatorParameters parameters;
        private readonly CoordinatorDependencies dependencies;
        private readonly CoordinatorHooks hooks;
        private readonly CoordinatorConfiguration configuration;

        private readonly CoordinatorFacts facts;
        private readonly CoordinatorCollection collection;

        public CoordinatorService(CoordinatorParameters parameters, CoordinatorDependencies dependencies, CoordinatorHooks hooks, CoordinatorConfiguration configuration)
        {
            this.parameters = parameters;
            this.dependencies = dependencies;
            this.hooks = hooks;
            this.configuration = configuration;

            collection = new CoordinatorCollection();

            facts = new CoordinatorFacts();
            facts.Install(configuration.Plugins);
        }

        public FileHash Hash
        {
            get { return parameters.Hash; }
        }

        public CoordinatorParameters Parameters
        {
            get { return parameters; }
        }

        public CoordinatorDependencies Dependencies
        {
            get { return dependencies; }
        }

        public CoordinatorHooks Hooks
        {
            get { return hooks; }
        }

        public CoordinatorConfiguration Configuration
        {
            get { return configuration; }
        }

        public void Start()
        {
            dependencies.Pipeline.Register(TimeSpan.FromSeconds(15), OnTick);
        }

        private void OnTick()
        {
            foreach (CoordinatorEntry entry in collection.All())
            {
                hooks.SendKeepAlive(entry.Peer);
            }
        }

        public bool Connect(NetworkConnection connection, Handshake handshake)
        {
            CoordinatorEntry entry = collection.Add(connection, handshake);

            if (entry != null)
            {
                entry.More = new MoreContainer();
                entry.Extensions = handshake.Options.HasFlag(HandshakeOptions.Extended);

                hooks.CallPeerConnected(entry.Peer, connection);

                SendBitfieldIfNeeded(entry);
                SendActiveHandshakeWithExtensionsIfNeeded(entry);
            }

            return entry != null;
        }

        private void SendBitfieldIfNeeded(CoordinatorEntry entry)
        {
            if (configuration.AnnounceBitfield && facts.Bitfield != null)
            {
                hooks.SendBitfield(entry.Peer, facts.Bitfield);
            }
        }

        private void SendActiveHandshakeWithExtensionsIfNeeded(CoordinatorEntry entry)
        {
            bool supportExtensions = entry.Extensions;
            bool isOutgoing = entry.Direction == NetworkDirection.Outgoing;

            if (supportExtensions && isOutgoing)
            {
                hooks.SendExtended(entry.Peer, facts.GetHandshake());
                hooks.CallExtensionListSent(entry.Peer, facts.GetExtensions());
            }
        }

        public bool Disconnect(NetworkConnection connection)
        {
            CoordinatorEntry entry = collection.Remove(connection);

            if (entry != null)
            {
                hooks.CallPeerDisconnected(entry.Peer, entry.Remote);
            }

            return entry != null;
        }

        public void Handle(MetafileVerified data)
        {
            facts.Handle(data);
        }

        public void Handle(DataVerified data)
        {
            facts.Handle(data);
        }

        public void SendChoke(PeerHash peer)
        {
            CoordinatorEntry entry = collection.Find(peer);

            if (entry != null)
            {
                hooks.SendChoke(entry.Peer);
                entry.State.IsLocalChokingRemote = true;
                hooks.CallStatusChanged(entry.Peer, entry.State);
            }
        }

        public void Unchoke(PeerHash peer)
        {
            CoordinatorEntry entry = collection.Find(peer);

            if (entry != null)
            {
                hooks.SendUnchoke(entry.Peer);
                entry.State.IsLocalChokingRemote = false;
                hooks.CallStatusChanged(entry.Peer, entry.State);
            }
        }

        public void Interested(PeerHash peer)
        {
            CoordinatorEntry entry = collection.Find(peer);

            if (entry != null)
            {
                hooks.SendInterested(entry.Peer);
                entry.State.IsLocalInterestedInRemote = true;
                hooks.CallStatusChanged(entry.Peer, entry.State);
            }
        }

        public void SendBitfield(PeerHash peer, Bitfield bitfield)
        {
            CoordinatorEntry entry = collection.Find(peer);

            if (entry != null)
            {
                hooks.SendBitfield(entry.Peer, bitfield);
            }
        }

        public void SendRequest(PeerHash peer, BlockIndex block)
        {
            CoordinatorEntry entry = collection.Find(peer);
            Request request = new Request(block);

            if (entry != null)
            {
                hooks.SendRequest(entry.Peer, request);
            }
        }

        public void SendPiece(PeerHash peer, BlockIndex block, DataBlock payload)
        {
            CoordinatorEntry entry = collection.Find(peer);
            Piece piece = new Piece(block, payload);

            if (entry != null)
            {
                hooks.SendPiece(entry.Peer, piece);
            }
        }

        public void SendHave(PeerHash peer, int piece)
        {
            CoordinatorEntry entry = collection.Find(peer);

            if (entry != null)
            {
                hooks.SendHave(entry.Peer, new PieceInfo(piece));
            }
        }

        public void SendExtension(PeerHash peer, string extension, byte[] payload)
        {
            CoordinatorEntry entry = collection.Find(peer);

            if (entry != null)
            {
                byte identifier = entry.More.Translate(extension);
                Extended extended = new Extended(identifier, payload);
                MoreHandler handler = facts.GetHandler(extension);

                hooks.SendExtended(entry.Peer, extended);
                hooks.CallExtensionDataSent(entry.Peer, extension, payload.Length);
                handler.OnMessageSent(parameters.Hash, entry.Peer, payload);
            }
        }

        public bool IsSupported(PeerHash peer, string extension)
        {
            return collection.Find(peer)?.More.Supports(extension) == true;
        }

        public void ForEachPeer(Action<PeerHash> callback)
        {
            foreach (CoordinatorEntry entry in collection.All())
            {
                callback.Invoke(entry.Peer);
            }
        }

        public void ForEachPeer(Action<PeerHash, NetworkAddress> callback)
        {
            foreach (CoordinatorEntry entry in collection.All())
            {
                callback.Invoke(entry.Peer, entry.Remote);
            }
        }

        public void ForEachPeer(Action<PeerHash, NetworkAddress, NetworkDirection> callback)
        {
            foreach (CoordinatorEntry entry in collection.All())
            {
                callback.Invoke(entry.Peer, entry.Remote, entry.Direction);
            }
        }

        public void Handle(MessageReceived data)
        {
            CoordinatorEntry entry = collection.Find(data.Peer);

            if (entry != null)
            {
                switch (data.Type)
                {
                    case "choke":
                        entry.State.IsRemoteChokingLocal = true;
                        hooks.CallStatusChanged(entry.Peer, entry.State);
                        break;

                    case "unchoke":
                        entry.State.IsRemoteChokingLocal = false;
                        hooks.CallStatusChanged(entry.Peer, entry.State);
                        break;

                    case "interested":
                        entry.State.IsRemoteInterestedInLocal = true;
                        hooks.CallStatusChanged(entry.Peer, entry.State);
                        break;

                    case "have":
                        entry.Bitfield = facts.ApplyHave(entry.Bitfield, data.Payload.GetInt32(0));
                        hooks.CallBitfieldChanged(entry.Peer, entry.Bitfield, new PieceInfo(data.Payload.GetInt32(0)));
                        break;

                    case "bitfield":
                        entry.Bitfield = facts.ApplyBitfield(entry.Bitfield, data.Payload.GetBitfield());
                        hooks.CallBitfieldChanged(entry.Peer, entry.Bitfield);
                        break;

                    case "request":
                        hooks.CallBlockRequested(parameters.Hash, entry.Peer, data.Payload.GetRequest());
                        break;

                    case "piece":
                        hooks.CallBlockReceived(parameters.Hash, entry.Peer, data.Payload.GetPiece(dependencies.Blocks));
                        break;

                    case "extended":
                        HandleHandshakeWithExtensionsIfNeeded(entry, data);
                        SendPassiveHandshakeWithExtensionsIfNeeded(entry, data);
                        HandleExtensionIfNeeded(entry, data);
                        break;
                }
            }
        }

        private void HandleHandshakeWithExtensionsIfNeeded(CoordinatorEntry entry, MessageReceived data)
        {
            if (data.Payload.IsExtensionHandshake())
            {
                entry.More = new MoreContainer(data.Payload.GetBencoded());
                hooks.CallExtensionListReceived(entry.Peer, entry.More.ToArray());
            }
        }

        private void SendPassiveHandshakeWithExtensionsIfNeeded(CoordinatorEntry entry, MessageReceived data)
        {
            if (data.Payload.IsExtensionHandshake())
            {
                bool supportExtensions = entry.Extensions;
                bool isIncoming = entry.Direction == NetworkDirection.Incoming;

                if (supportExtensions && isIncoming)
                {
                    hooks.SendExtended(entry.Peer, facts.GetHandshake());
                    hooks.CallExtensionListSent(entry.Peer, facts.GetExtensions());
                }
            }
        }

        private void HandleExtensionIfNeeded(CoordinatorEntry entry, MessageReceived data)
        {
            if (data.Payload.IsExtensionHandshake())
            {
                foreach (MoreHandler handler in facts.AllHandlers())
                {
                    handler.OnHandshake(parameters.Hash, entry.Peer, data.Payload.GetExtensionData());
                }
            }

            if (data.Payload.IsExtensionHandshake() == false)
            {
                MoreHandler handler;
                byte id = data.Payload.GetExtensionIdentifier();
                string code = facts.Translate(id, out handler);

                hooks.CallExtensionDataReceived(entry.Peer, code, data.Payload.GetExtensionSize());
                handler.OnMessageReceived(parameters.Hash, entry.Peer, data.Payload.GetExtensionData());
            }
        }
    }
}