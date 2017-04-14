using System;
using Leak.Common;
using Leak.Events;
using Leak.Extensions;
using Leak.Networking.Core;
using Leak.Peer.Coordinator.Core;
using Leak.Peer.Receiver;
using Leak.Peer.Receiver.Events;
using Leak.Peer.Sender;

namespace Leak.Peer.Coordinator
{
    public class CoordinatorImplementation : CoordinatorService
    {
        private readonly CoordinatorParameters parameters;
        private readonly CoordinatorDependencies dependencies;
        private readonly CoordinatorHooks hooks;
        private readonly CoordinatorConfiguration configuration;

        private readonly CoordinatorFacts facts;
        private readonly CoordinatorCollection collection;

        public CoordinatorImplementation(CoordinatorParameters parameters, CoordinatorDependencies dependencies, CoordinatorHooks hooks, CoordinatorConfiguration configuration)
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
                entry.Commy.SendKeepAlive();
            }
        }

        public bool Connect(NetworkConnection connection, Handshake handshake)
        {
            CoordinatorEntry entry = collection.Add(connection, handshake);

            if (entry != null)
            {
                entry.Loopy = CreateReceiver();
                entry.Commy = CreateCommy(entry.Peer, connection);

                entry.More = new MoreContainer();
                entry.Extensions = handshake.Options.HasFlag(HandshakeOptions.Extended);

                hooks.CallPeerConnected(entry.Peer);
                entry.Loopy.StartProcessing(entry.Peer, connection);

                SendBitfieldIfNeeded(entry);
                SendActiveHandshakeWithExtensionsIfNeeded(entry);
            }

            return entry != null;
        }

        private void SendBitfieldIfNeeded(CoordinatorEntry entry)
        {
            if (configuration.AnnounceBitfield && facts.Bitfield != null)
            {
                entry.Commy.SendBitfield(facts.Bitfield);
            }
        }

        private void SendActiveHandshakeWithExtensionsIfNeeded(CoordinatorEntry entry)
        {
            bool supportExtensions = entry.Extensions;
            bool isOutgoing = entry.Direction == NetworkDirection.Outgoing;

            if (supportExtensions && isOutgoing)
            {
                entry.Commy.SendExtended(facts.GetHandshake());
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
                entry.Commy.SendChoke();
                entry.State.IsLocalChokingRemote = true;
                hooks.CallPeerStatusChanged(entry.Peer, entry.State);
            }
        }

        public void SendUnchoke(PeerHash peer)
        {
            CoordinatorEntry entry = collection.Find(peer);

            if (entry != null)
            {
                entry.Commy.SendUnchoke();
                entry.State.IsLocalChokingRemote = false;
                hooks.CallPeerStatusChanged(entry.Peer, entry.State);
            }
        }

        public void SendInterested(PeerHash peer)
        {
            CoordinatorEntry entry = collection.Find(peer);

            if (entry != null)
            {
                entry.Commy.SendInterested();
                entry.State.IsLocalInterestedInRemote = true;
                hooks.CallPeerStatusChanged(entry.Peer, entry.State);
            }
        }

        public void SendBitfield(PeerHash peer, Bitfield bitfield)
        {
            CoordinatorEntry entry = collection.Find(peer);

            if (entry != null)
            {
                entry.Commy.SendBitfield(bitfield);
            }
        }

        public void SendRequest(PeerHash peer, BlockIndex block)
        {
            CoordinatorEntry entry = collection.Find(peer);
            Request request = new Request(block);

            if (entry != null)
            {
                entry.Commy.SendRequest(request);
            }
        }

        public void SendPiece(PeerHash peer, BlockIndex block, DataBlock payload)
        {
            CoordinatorEntry entry = collection.Find(peer);
            Piece piece = new Piece(block, payload);

            entry?.Commy.SendPiece(piece);
        }

        public void SendHave(PeerHash peer, int piece)
        {
            CoordinatorEntry entry = collection.Find(peer);

            if (entry != null)
            {
                entry.Commy.SendHave(new PieceInfo(piece));
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

                entry.Commy.SendExtended(extended);
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

        private ReceiverService CreateReceiver()
        {
            return
                new ReceiverBuilder()
                    .WithDefinition(configuration.ReceiverDefinition)
                    .Build(CreateLoopyHooks());
        }

        private ReceiverHooks CreateLoopyHooks()
        {
            return new ReceiverHooks
            {
                OnMessageReceived = OnMessageReceived
            };
        }

        private SenderService CreateCommy(PeerHash peer, NetworkConnection connection)
        {
            SenderHooks hooks = new SenderHooks();
            SenderConfiguration config = new SenderConfiguration { Definition = configuration.SenderDefinition };

            return new SenderService(peer, connection, hooks, config);
        }

        private void OnMessageReceived(MessageReceived data)
        {
            CoordinatorEntry entry = collection.Find(data.Peer);

            if (entry != null)
            {
                switch (data.Type)
                {
                    case "choke":
                        entry.State.IsRemoteChokingLocal = true;
                        hooks.CallPeerStatusChanged(entry.Peer, entry.State);
                        break;

                    case "unchoke":
                        entry.State.IsRemoteChokingLocal = false;
                        hooks.CallPeerStatusChanged(entry.Peer, entry.State);
                        break;

                    case "interested":
                        entry.State.IsRemoteInterestedInLocal = true;
                        hooks.CallPeerStatusChanged(entry.Peer, entry.State);
                        break;

                    case "have":
                        entry.Bitfield = facts.ApplyHave(entry.Bitfield, data.Payload.GetInt32(0));
                        hooks.CallPeerBitfieldChanged(entry.Peer, entry.Bitfield, new PieceInfo(data.Payload.GetInt32(0)));
                        break;

                    case "bitfield":
                        entry.Bitfield = facts.ApplyBitfield(entry.Bitfield, data.Payload.GetBitfield());
                        hooks.CallPeerBitfieldChanged(entry.Peer, entry.Bitfield);
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
                    entry.Commy.SendExtended(facts.GetHandshake());
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