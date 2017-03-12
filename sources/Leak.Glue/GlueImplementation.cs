using Leak.Common;
using Leak.Communicator;
using Leak.Communicator.Messages;
using Leak.Events;
using Leak.Extensions;
using Leak.Loop;
using System;

namespace Leak.Glue
{
    public class GlueImplementation : GlueService
    {
        private readonly GlueParameters parameters;
        private readonly GlueDependencies dependencies;
        private readonly GlueHooks hooks;
        private readonly GlueConfiguration configuration;

        private readonly GlueFacts facts;
        private readonly GlueCollection collection;

        public GlueImplementation(GlueParameters parameters, GlueDependencies dependencies, GlueHooks hooks, GlueConfiguration configuration)
        {
            this.parameters = parameters;
            this.dependencies = dependencies;
            this.hooks = hooks;
            this.configuration = configuration;

            collection = new GlueCollection();

            facts = new GlueFacts();
            facts.Install(configuration.Plugins);
        }

        public FileHash Hash
        {
            get { return parameters.Hash; }
        }

        public GlueParameters Parameters
        {
            get { return parameters; }
        }

        public GlueDependencies Dependencies
        {
            get { return dependencies; }
        }

        public GlueHooks Hooks
        {
            get { return hooks; }
        }

        public GlueConfiguration Configuration
        {
            get { return configuration; }
        }

        public bool Connect(NetworkConnection connection, Handshake handshake)
        {
            GlueEntry entry = collection.Add(connection, handshake);

            if (entry != null)
            {
                entry.Loopy = CreateLoopy();
                entry.Commy = CreateCommy(entry.Peer, connection);

                entry.More = new MoreContainer();
                entry.Extensions = handshake.Options.HasFlag(HandshakeOptions.Extended);

                hooks.CallPeerConnected(entry.Peer);
                entry.Loopy.StartProcessing(entry.Peer, connection);

                SendActiveHandshakeWithExtensionsIfNeeded(entry);
            }

            return entry != null;
        }

        private void SendActiveHandshakeWithExtensionsIfNeeded(GlueEntry entry)
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
            GlueEntry entry = collection.Remove(connection);

            if (entry != null)
            {
                hooks.CallPeerDisconnected(entry.Peer);
            }

            return entry != null;
        }

        public void Handle(MetafileVerified data)
        {
            facts.Handle(data);
        }

        public void SendChoke(PeerHash peer)
        {
            GlueEntry entry = collection.Find(peer);

            if (entry != null)
            {
                entry.Commy.SendChoke();
                entry.State.IsLocalChokingRemote = true;
                hooks.CallPeerStatusChanged(entry.Peer, entry.State);
            }
        }

        public void SendUnchoke(PeerHash peer)
        {
            GlueEntry entry = collection.Find(peer);

            if (entry != null)
            {
                entry.Commy.SendUnchoke();
                entry.State.IsLocalChokingRemote = false;
                hooks.CallPeerStatusChanged(entry.Peer, entry.State);
            }
        }

        public void SendInterested(PeerHash peer)
        {
            GlueEntry entry = collection.Find(peer);

            if (entry != null)
            {
                entry.Commy.SendInterested();
                entry.State.IsLocalInterestedInRemote = true;
                hooks.CallPeerStatusChanged(entry.Peer, entry.State);
            }
        }

        public void SendBitfield(PeerHash peer, Bitfield bitfield)
        {
            GlueEntry entry = collection.Find(peer);

            if (entry != null)
            {
                entry.Commy.SendBitfield(bitfield);
            }
        }

        public void SendRequest(PeerHash peer, BlockIndex block)
        {
            GlueEntry entry = collection.Find(peer);
            Request request = new Request(block);

            if (entry != null)
            {
                entry.Commy.SendRequest(request);
            }
        }

        public void SendPiece(PeerHash peer, BlockIndex block, DataBlock payload)
        {
            GlueEntry entry = collection.Find(peer);
            Piece piece = new Piece(block, payload);

            entry?.Commy.SendPiece(piece);
        }

        public void SendHave(PeerHash peer, int piece)
        {
            GlueEntry entry = collection.Find(peer);

            if (entry != null)
            {
                entry.Commy.SendHave(piece);
            }
        }

        public void SendExtension(PeerHash peer, string extension, byte[] payload)
        {
            GlueEntry entry = collection.Find(peer);

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
            foreach (GlueEntry entry in collection.All())
            {
                callback.Invoke(entry.Peer);
            }
        }

        public void ForEachPeer(Action<PeerHash, PeerAddress> callback)
        {
            foreach (GlueEntry entry in collection.All())
            {
                callback.Invoke(entry.Peer, entry.Remote);
            }
        }

        public void ForEachPeer(Action<PeerHash, PeerAddress, NetworkDirection> callback)
        {
            foreach (GlueEntry entry in collection.All())
            {
                callback.Invoke(entry.Peer, entry.Remote, entry.Direction);
            }
        }

        private ConnectionLoop CreateLoopy()
        {
            ConnectionLoopHooks other = CreateLoopyHooks();
            ConnectionLoopConfiguration config = new ConnectionLoopConfiguration();

            return new ConnectionLoop(other, config);
        }

        private ConnectionLoopHooks CreateLoopyHooks()
        {
            return new ConnectionLoopHooks
            {
                OnMessageReceived = OnMessageReceived
            };
        }

        private CommunicatorService CreateCommy(PeerHash peer, NetworkConnection connection)
        {
            CommunicatorHooks hooks = new CommunicatorHooks();
            CommunicatorConfiguration config = new CommunicatorConfiguration();

            return new CommunicatorService(peer, connection, hooks, config);
        }

        private void OnMessageReceived(MessageReceived data)
        {
            GlueEntry entry = collection.Find(data.Peer);

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
                        hooks.CallPeerBitfieldChanged(entry.Peer, entry.Bitfield);
                        break;

                    case "bitfield":
                        entry.Bitfield = facts.ApplyBitfield(entry.Bitfield, data.Payload.GetBitfield());
                        hooks.CallPeerBitfieldChanged(entry.Peer, entry.Bitfield);
                        break;

                    case "request":
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

        private void HandleHandshakeWithExtensionsIfNeeded(GlueEntry entry, MessageReceived data)
        {
            if (data.Payload.IsExtensionHandshake())
            {
                entry.More = new MoreContainer(data.Payload.GetBencoded());
                hooks.CallExtensionListReceived(entry.Peer, entry.More.ToArray());
            }
        }

        private void SendPassiveHandshakeWithExtensionsIfNeeded(GlueEntry entry, MessageReceived data)
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

        private void HandleExtensionIfNeeded(GlueEntry entry, MessageReceived data)
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