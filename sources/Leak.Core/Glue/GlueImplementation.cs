using Leak.Core.Common;
using Leak.Core.Events;
using Leak.Core.Loop;
using Leak.Core.Messages;
using Leak.Core.Metadata;
using Leak.Core.Negotiator;
using Leak.Core.Network;
using System;

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
            ConnectionLoop loop = CreateLoopy();
            GlueEntry entry = collection.Add(connection, handshake);

            if (entry != null)
            {
                hooks.CallPeerConnected(entry.Peer);
                loop.StartProcessing(entry.Peer, connection);
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

        public void AddMetainfo(Metainfo metainfo)
        {
            facts.ApplyMetainfo(metainfo);
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
                OnPeerKeepAliveMessageReceived = OnPeerKeepAliveMessageReceived,
                OnPeerChokeMessageReceived = OnPeerChokeMessageReceived,
                OnPeerUnchokeMessageReceived = OnPeerUnchokeMessageReceived,
                OnPeerInterestedMessageReceived = OnPeerInterestedMessageReceived,
                OnPeerHaveMessageReceived = OnPeerHaveMessageReceived,
                OnPeerBitfieldMessageReceived = OnPeerBitfieldMessageReceived,
                OnPeerPieceMessageReceived = OnPeerPieceMessageReceived,
                OnPeerExtendedMessageReceived = OnPeerExtendedMessageReceived,
            };
        }

        private void OnPeerKeepAliveMessageReceived(PeerKeepAliveMessageReceived data)
        {
            WithPeer(data.Peer, hooks?.OnPeerKeepAliveMessageReceived, data, entry =>
            {
            });
        }

        private void OnPeerChokeMessageReceived(PeerChokeMessageReceived data)
        {
            WithPeer(data.Peer, hooks?.OnPeerChokeMessageReceived, data, entry =>
            {
                entry.State |= GlueState.IsRemoteChockingLocal;
                hooks.CallPeerStateChanged(entry.Peer, entry.State);
            });
        }

        private void OnPeerUnchokeMessageReceived(PeerUnchokeMessageReceived data)
        {
            WithPeer(data.Peer, hooks?.OnPeerUnchokeMessageReceived, data, entry =>
            {
                entry.State &= ~GlueState.IsRemoteChockingLocal;
                hooks.CallPeerStateChanged(entry.Peer, entry.State);
            });
        }

        private void OnPeerInterestedMessageReceived(PeerInterestedMessageReceived data)
        {
            WithPeer(data.Peer, hooks?.OnPeerInterestedMessageReceived, data, entry =>
            {
                entry.State |= GlueState.IsRemoteChockingLocal;
                hooks.CallPeerStateChanged(entry.Peer, entry.State);
            });
        }

        private void OnPeerHaveMessageReceived(PeerHaveMessageReceived data)
        {
            WithPeer(data.Peer, hooks?.OnPeerHaveMessageReceived, data, entry =>
            {
                entry.Bitfield = facts.ApplyHave(entry.Bitfield, data.Piece);
                hooks.CallPeerBitfieldChanged(entry.Peer, entry.Bitfield);
            });
        }

        private void OnPeerBitfieldMessageReceived(PeerBitfieldMessageReceived data)
        {
            WithPeer(data.Peer, hooks?.OnPeerBitfieldMessageReceived, data, entry =>
            {
                entry.Bitfield = facts.ApplyBitfield(entry.Bitfield, data.Bitfield);
                hooks.CallPeerBitfieldChanged(entry.Peer, entry.Bitfield);
            });
        }

        private void OnPeerPieceMessageReceived(PeerPieceMessageReceived data)
        {
            WithPeer(data.Peer, hooks?.OnPeerPieceMessageReceived, data, entry =>
            {
            });
        }

        private void OnPeerExtendedMessageReceived(PeerExtendedMessageReceived data)
        {
            WithPeer(data.Peer, hooks?.OnPeerExtendedMessageReceived, data, entry =>
            {
            });
        }

        private void WithPeer<T>(PeerHash peer, Action<T> forwarder, T payload, Action<GlueEntry> callback)
        {
            GlueEntry entry = collection.Find(peer);

            if (forwarder != null)
            {
                forwarder.Invoke(payload);
            }

            if (entry != null)
            {
                SetTimestamp(entry);
                callback.Invoke(entry);
            }
        }

        private void SetTimestamp(GlueEntry entry)
        {
            entry.Timestamp = DateTime.Now;
        }
    }
}