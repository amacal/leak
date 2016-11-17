using Leak.Core.Common;
using Leak.Core.Communicator;
using Leak.Core.Events;
using Leak.Core.Loop;
using Leak.Core.Messages;
using Leak.Core.Negotiator;
using Leak.Core.Network;

namespace Leak.Core.Glue
{
    public class GlueImplementation : GlueService
    {
        private readonly FileHash hash;
        private readonly DataBlockFactory factory;
        private readonly GlueHooks hooks;

        private readonly GlueFacts facts;
        private readonly GlueCollection collection;

        public GlueImplementation(FileHash hash, DataBlockFactory factory, GlueHooks hooks, GlueConfiguration configuration)
        {
            this.hash = hash;
            this.factory = factory;
            this.hooks = hooks;

            collection = new GlueCollection();

            facts = new GlueFacts(configuration);
            facts.Install(configuration.Plugins);
        }

        public FileHash Hash
        {
            get { return hash; }
        }

        public bool Connect(NetworkConnection connection, Handshake handshake)
        {
            GlueEntry entry = collection.Add(connection, handshake);

            if (entry != null)
            {
                entry.Loopy = CreateLoopy();
                entry.Commy = CreateCommy(entry.Peer, connection);

                entry.More = new GlueMore();
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

        public void SetPieces(int pieces)
        {
            facts.ApplyPieces(pieces);
        }

        public void SendChoke(PeerHash peer)
        {
            GlueEntry entry = collection.Find(peer);

            if (entry != null)
            {
                entry.Commy.SendChoke();
                entry.State |= GlueState.IsLocalChockingRemote;
                hooks.CallPeerStateChanged(peer, entry.State);
            }
        }

        public void SendUnchoke(PeerHash peer)
        {
            GlueEntry entry = collection.Find(peer);

            if (entry != null)
            {
                entry.Commy.SendUnchoke();
                entry.State &= ~GlueState.IsLocalChockingRemote;
                hooks.CallPeerStateChanged(peer, entry.State);
            }
        }

        public void SendInterested(PeerHash peer)
        {
            GlueEntry entry = collection.Find(peer);

            if (entry != null)
            {
                entry.Commy.SendInterested();
                entry.State |= GlueState.IsLocalInterestedInRemote;
                hooks.CallPeerStateChanged(peer, entry.State);
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

                entry.Commy.SendExtended(extended);
                hooks.CallExtensionDataSent(entry.Peer, extension, payload.Length);
            }
        }

        public bool IsSupported(PeerHash peer, string extension)
        {
            return collection.Find(peer)?.More.Supports(extension) == true;
        }

        private ConnectionLoop CreateLoopy()
        {
            ConnectionLoopHooks other = CreateLoopyHooks();
            ConnectionLoopConfiguration config = new ConnectionLoopConfiguration();

            return new ConnectionLoop(factory, other, config);
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
                        entry.State |= GlueState.IsRemoteChockingLocal;
                        hooks.CallPeerStateChanged(entry.Peer, entry.State);
                        break;

                    case "unchoke":
                        entry.State &= ~GlueState.IsRemoteChockingLocal;
                        hooks.CallPeerStateChanged(entry.Peer, entry.State);
                        break;

                    case "interested":
                        entry.State |= GlueState.IsRemoteInterestedInLocal;
                        hooks.CallPeerStateChanged(entry.Peer, entry.State);
                        break;

                    case "have":
                        entry.Bitfield = facts.ApplyHave(entry.Bitfield, data.Payload.GetInt32(0));
                        hooks.CallPeerBitfieldChanged(entry.Peer, entry.Bitfield);
                        break;

                    case "bitfield":
                        entry.Bitfield = facts.ApplyBitfield(entry.Bitfield, data.Payload.GetBitfield());
                        hooks.CallPeerBitfieldChanged(entry.Peer, entry.Bitfield);
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
                entry.More = new GlueMore(data.Payload.GetBencoded());
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
            if (data.Payload.IsExtensionHandshake() == false)
            {
                GlueHandler handler;
                byte id = data.Payload.GetExtensionIdentifier();
                string code = facts.Translate(id, out handler);

                hooks.CallExtensionDataReceived(entry.Peer, code, data.Payload.GetExtensionSize());
                handler.Handle(hash, entry.Peer, data.Payload.GetExtensionData());
            }
        }
    }
}