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

                hooks.CallPeerConnected(entry.Peer);
                entry.Loopy.StartProcessing(entry.Peer, connection);
            }

            return entry != null;
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
                }
            }
        }
    }
}