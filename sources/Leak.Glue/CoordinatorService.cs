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
        private readonly CoordinatorContext context;

        public CoordinatorService(CoordinatorParameters parameters, CoordinatorDependencies dependencies, CoordinatorHooks hooks, CoordinatorConfiguration configuration)
        {
            context = new CoordinatorContext(parameters, dependencies, hooks, configuration);
        }

        public FileHash Hash
        {
            get { return context.Parameters.Hash; }
        }

        public CoordinatorParameters Parameters
        {
            get { return context.Parameters; }
        }

        public CoordinatorDependencies Dependencies
        {
            get { return context.Dependencies; }
        }

        public CoordinatorHooks Hooks
        {
            get { return context.Hooks; }
        }

        public CoordinatorConfiguration Configuration
        {
            get { return context.Configuration; }
        }

        public void Start()
        {
            context.Facts.Install(context.Configuration.Plugins);
            context.Dependencies.Pipeline.Register(TimeSpan.FromSeconds(15), OnTick);
        }

        private void OnTick()
        {
            foreach (CoordinatorEntry entry in context.Collection.All())
            {
                context.Hooks.SendKeepAlive(entry.Peer);
            }
        }

        public void Connect(NetworkConnection connection, Handshake handshake)
        {
            CoordinatorEntry entry = context.Collection.Add(connection, handshake);

            if (entry != null)
            {
                entry.More = new MoreContainer();
                entry.Extensions = handshake.Options.HasFlag(HandshakeOptions.Extended);

                context.Hooks.CallPeerConnected(entry.Peer, connection);

                SendBitfieldIfNeeded(entry);
                SendActiveHandshakeWithExtensionsIfNeeded(entry);
            }
            else
            {
                connection.Terminate();
            }
        }

        private void SendBitfieldIfNeeded(CoordinatorEntry entry)
        {
            if (context.Configuration.AnnounceBitfield && context.Facts.Bitfield != null)
            {
                context.Hooks.SendBitfield(entry.Peer, context.Facts.Bitfield);
            }
        }

        private void SendActiveHandshakeWithExtensionsIfNeeded(CoordinatorEntry entry)
        {
            bool supportExtensions = entry.Extensions;
            bool isOutgoing = entry.Direction == NetworkDirection.Outgoing;

            if (supportExtensions && isOutgoing)
            {
                context.Hooks.SendExtended(entry.Peer, context.Facts.GetHandshake());
                context.Hooks.CallExtensionListSent(entry.Peer, context.Facts.GetExtensions());
            }
        }

        public void Disconnect(NetworkConnection connection)
        {
            CoordinatorEntry entry = context.Collection.Remove(connection);

            if (entry != null)
            {
                context.Hooks.CallPeerDisconnected(entry.Peer, entry.Remote);
            }
        }

        public void Handle(MetafileVerified data)
        {
            context.Facts.Handle(data);
        }

        public void Handle(DataVerified data)
        {
            context.Facts.Handle(data);
        }

        public void Choke(PeerHash peer, bool value)
        {
            CoordinatorEntry entry = context.Collection.Find(peer);

            if (entry != null)
            {
                context.Hooks.SendChoke(entry.Peer, value);
                entry.State.IsLocalChokingRemote = value;
                context.Hooks.CallStatusChanged(entry.Peer, entry.State);
            }
        }

        public void Interested(PeerHash peer)
        {
            CoordinatorEntry entry = context.Collection.Find(peer);

            if (entry != null)
            {
                context.Hooks.SendInterested(entry.Peer);
                entry.State.IsLocalInterestedInRemote = true;
                context.Hooks.CallStatusChanged(entry.Peer, entry.State);
            }
        }

        public void SendRequest(PeerHash peer, BlockIndex block)
        {
            CoordinatorEntry entry = context.Collection.Find(peer);
            Request request = new Request(block);

            if (entry != null)
            {
                context.Hooks.SendRequest(entry.Peer, request);
            }
        }

        public void SendPiece(PeerHash peer, BlockIndex block, DataBlock payload)
        {
            CoordinatorEntry entry = context.Collection.Find(peer);
            Piece piece = new Piece(block, payload);

            if (entry != null)
            {
                context.Hooks.SendPiece(entry.Peer, piece);
            }
        }

        public void SendHave(PeerHash peer, int piece)
        {
            CoordinatorEntry entry = context.Collection.Find(peer);

            if (entry != null)
            {
                context.Hooks.SendHave(entry.Peer, new PieceInfo(piece));
            }
        }

        public void SendExtension(PeerHash peer, string extension, byte[] payload)
        {
            CoordinatorEntry entry = context.Collection.Find(peer);

            if (entry != null)
            {
                byte identifier = entry.More.Translate(extension);
                Extended extended = new Extended(identifier, payload);
                MoreHandler handler = context.Facts.GetHandler(extension);

                context.Hooks.SendExtended(entry.Peer, extended);
                context.Hooks.CallExtensionDataSent(entry.Peer, extension, payload.Length);
                handler.OnMessageSent(context.Parameters.Hash, entry.Peer, payload);
            }
        }

        public bool IsSupported(PeerHash peer, string extension)
        {
            return context.Collection.Find(peer)?.More.Supports(extension) == true;
        }

        public void ForEachPeer(Action<PeerHash> callback)
        {
            foreach (CoordinatorEntry entry in context.Collection.All())
            {
                callback.Invoke(entry.Peer);
            }
        }

        public void ForEachPeer(Action<PeerHash, NetworkAddress> callback)
        {
            foreach (CoordinatorEntry entry in context.Collection.All())
            {
                callback.Invoke(entry.Peer, entry.Remote);
            }
        }

        public void ForEachPeer(Action<PeerHash, NetworkAddress, NetworkDirection> callback)
        {
            foreach (CoordinatorEntry entry in context.Collection.All())
            {
                callback.Invoke(entry.Peer, entry.Remote, entry.Direction);
            }
        }

        public void Handle(MessageReceived data)
        {
            CoordinatorEntry entry = context.Collection.Find(data.Peer);

            if (entry != null)
            {
                switch (data.Type)
                {
                    case "choke":
                        entry.State.IsRemoteChokingLocal = true;
                        context.Hooks.CallStatusChanged(entry.Peer, entry.State);
                        break;

                    case "unchoke":
                        entry.State.IsRemoteChokingLocal = false;
                        context.Hooks.CallStatusChanged(entry.Peer, entry.State);
                        break;

                    case "interested":
                        entry.State.IsRemoteInterestedInLocal = true;
                        context.Hooks.CallStatusChanged(entry.Peer, entry.State);
                        break;

                    case "have":
                        entry.Bitfield = context.Facts.ApplyHave(entry.Bitfield, data.Payload.GetInt32(0));
                        context.Hooks.CallBitfieldChanged(entry.Peer, entry.Bitfield, new PieceInfo(data.Payload.GetInt32(0)));
                        break;

                    case "bitfield":
                        entry.Bitfield = context.Facts.ApplyBitfield(entry.Bitfield, data.Payload.GetBitfield());
                        context.Hooks.CallBitfieldChanged(entry.Peer, entry.Bitfield);
                        break;

                    case "request":
                        context.Hooks.CallBlockRequested(context.Parameters.Hash, entry.Peer, data.Payload.GetRequest());
                        break;

                    case "piece":
                        context.Hooks.CallBlockReceived(context.Parameters.Hash, entry.Peer, data.Payload.GetPiece(context.Dependencies.Blocks));
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
                context.Hooks.CallExtensionListReceived(entry.Peer, entry.More.ToArray());
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
                    context.Hooks.SendExtended(entry.Peer, context.Facts.GetHandshake());
                    context.Hooks.CallExtensionListSent(entry.Peer, context.Facts.GetExtensions());
                }
            }
        }

        private void HandleExtensionIfNeeded(CoordinatorEntry entry, MessageReceived data)
        {
            if (data.Payload.IsExtensionHandshake())
            {
                foreach (MoreHandler handler in context.Facts.AllHandlers())
                {
                    handler.OnHandshake(context.Parameters.Hash, entry.Peer, data.Payload.GetExtensionData());
                }
            }

            if (data.Payload.IsExtensionHandshake() == false)
            {
                MoreHandler handler;
                byte id = data.Payload.GetExtensionIdentifier();
                string code = context.Facts.Translate(id, out handler);

                context.Hooks.CallExtensionDataReceived(entry.Peer, code, data.Payload.GetExtensionSize());
                handler.OnMessageReceived(context.Parameters.Hash, entry.Peer, data.Payload.GetExtensionData());
            }
        }
    }
}