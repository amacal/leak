using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Leak.Common;
using Leak.Completion;
using Leak.Connector;
using Leak.Data.Get;
using Leak.Data.Map;
using Leak.Data.Store;
using Leak.Events;
using Leak.Extensions.Metadata;
using Leak.Extensions.Peers;
using Leak.Files;
using Leak.Glue;
using Leak.Listener;
using Leak.Listener.Events;
using Leak.Memory;
using Leak.Memory.Events;
using Leak.Meta.Get;
using Leak.Meta.Store;
using Leak.Negotiator;
using Leak.Networking;
using Leak.Tasks;
using Leak.Tracker.Get;
using Leak.Tracker.Get.Events;

namespace Leak.Client.Swarm
{
    public class SwarmConnect
    {
        public SwarmSettings Settings { get; set; }

        public CompletionWorker Worker { get; set; }
        public NetworkPool Network { get; set; }
        public PipelineService Pipeline { get; set; }
        public FileFactory Files { get; set; }
        public MemoryService Memory { get; set; }

        public FileHash Hash { get; set; }
        public PeerHash Localhost { get; set; }
        public Metainfo Metainfo { get; set; }

        public HashSet<PeerHash> Peers { get; set; }
        public HashSet<PeerAddress> Remotes { get; set; }

        public NotificationCollection Notifications { get; set; }
        public TaskCompletionSource<SwarmSession> Completion { get; set; }

        public TrackerGetService TrackerGet { get; set; }
        public PeerConnector Connector { get; set; }
        public PeerListener Listener { get; set; }
        public HandshakeNegotiator Negotiator { get; set; }
        public GlueService Glue { get; set; }

        public MetafileService MetaStore { get; set; }
        public MetagetService MetaGet { get; set; }

        public RepositoryService DataStore { get; set; }
        public RetrieverService DataGet { get; set; }
        public OmnibusService DataMap { get; set; }

        public void Start(string[] trackers)
        {
            StartMemory();
            StartNetwork();
            StartNegotiator();
            StartConnector();
            StartListener();
            StartGlue();
            StartDataMap();
            StartTrackerGet();

            Announce(trackers);
            Completion.SetResult(new SwarmSession(this));
        }

        private void Announce(string[] trackers)
        {
            foreach (string tracker in trackers)
            {
                TrackerGet.Register(new TrackerGetRegistrant
                {
                    Hash = Hash,
                    Address = new Uri(tracker, UriKind.Absolute),
                    Port = Settings.ListenerPort
                });
            }
        }

        public void Download(string destination)
        {
            StartMetaStore(destination);
            StartMetaGet();
            StartDataStore(destination);
            StartDataGet();
        }

        private void StartMemory()
        {
            MemoryHooks hooks = new MemoryHooks
            {
                OnMemorySnapshot = OnMemorySnapshot
            };

            Memory =
                new MemoryBuilder()
                    .WithMaxRequestSize(256 * 1024)
                    .WithThresholds(20 * 1024)
                    .Build(hooks);
        }

        private void StartNetwork()
        {
            NetworkPoolHooks hooks = new NetworkPoolHooks
            {
                OnConnectionTerminated = OnConnectionTerminated
            };

            Network =
                new NetworkPoolBuilder()
                    .WithPipeline(Pipeline)
                    .WithWorker(Worker)
                    .WithMemory(Memory.AsNetwork())
                    .WithBufferSize(256 * 1024)
                    .Build(hooks);

            Network.Start();
        }

        private void StartNegotiator()
        {
            HandshakeNegotiatorHooks hooks = new HandshakeNegotiatorHooks
            {
                OnHandshakeCompleted = OnHandshakeCompleted
            };

            Negotiator =
                new HandshakeNegotiatorBuilder()
                    .WithNetwork(Network)
                    .Build(hooks);
        }

        private void StartConnector()
        {
            if (Settings.Connector)
            {
                PeerConnectorHooks hooks = new PeerConnectorHooks
                {
                    OnConnectionEstablished = OnConnectionEstablished
                };

                Connector =
                    new PeerConnectorBuilder()
                        .WithNetwork(Network)
                        .WithPipeline(Pipeline)
                        .Build(hooks);

                Connector.Start();
            }
        }

        private void StartGlue()
        {
            MetadataHooks metadata = new MetadataHooks
            {
                OnMetadataMeasured = data => MetaGet?.Handle(data),
                OnMetadataPieceReceived = OnMetadataPieceReceived,
                OnMetadataRequestSent = OnMetadataRequestSent
            };

            PeersHooks exchange = new PeersHooks
            {
                OnPeersDataReceived = OnPeerDataReceived
            };

            GlueHooks hooks = new GlueHooks
            {
                OnPeerConnected = OnPeerConnected,
                OnPeerDisconnected = OnPeerDisconnected,
                OnPeerBitfieldChanged = OnPeerBitfieldChanged,
                OnPeerStatusChanged = OnPeerStatusChanged,
                OnBlockReceived = data => DataGet?.Handle(data)
            };

            Glue =
                new GlueBuilder()
                    .WithHash(Hash)
                    .WithMemory(Memory)
                    .WithPipeline(Pipeline)
                    .WithMetadata(Settings, metadata)
                    .WithExchange(Settings, exchange)
                    .Build(hooks);

            Glue.Start();
        }

        private void StartDataMap()
        {
            OmnibusHooks hooks = new OmnibusHooks
            {
                OnBlockReserved = data => DataGet?.Handle(data),
                OnPieceReady = data => DataGet?.Handle(data),
                OnPieceCompleted = OnPieceCompleted,
                OnThresholdReached = data => DataGet?.Handle(data),
                OnDataCompleted = OnDataCompleted,
                OnDataChanged = OnDataChanged
            };

            DataMap =
                new OmnibusBuilder()
                    .WithHash(Hash)
                    .WithPipeline(Pipeline)
                    .WithSchedulerThreshold(160)
                    .WithPoolSize(512)
                    .Build(hooks);

            DataMap.Start();
        }

        private void StartTrackerGet()
        {
            TrackerGetHooks hooks = new TrackerGetHooks
            {
                OnAnnounced = OnAnnounced
            };

            TrackerGet =
                new TrackerGetBuilder()
                    .WithPipeline(Pipeline)
                    .WithWorker(Worker)
                    .WithPeer(Localhost)
                    .Build(hooks);

            TrackerGet.Start();
        }

        private void StartListener()
        {
            if (Settings.Listener)
            {
                PeerListenerHooks hooks = new PeerListenerHooks
                {
                    OnConnectionArrived = OnConnectionArrived,
                    OnListenerStarted = OnListenerStarted,
                    OnListenerFailed = OnListenerFailed
                };

                Listener =
                    new PeerListenerBuilder()
                        .WithPeer(Localhost)
                        .WithNetwork(Network)
                        .WithPort(Settings)
                        .WithExtensions()
                        .Build(hooks);

                Listener.Start();
            }
        }

        private void StartMetaStore(string destination)
        {
            MetafileHooks hooks = new MetafileHooks
            {
                OnMetafileVerified = OnMetafileVerified
            };

            MetaStore =
                new MetafileBuilder()
                    .WithHash(Hash)
                    .WithPipeline(Pipeline)
                    .WithFiles(Files)
                    .WithDestination(Path.Combine(destination, $"{Hash}.metainfo"))
                    .Build(hooks);

            MetaStore.Start();
        }

        private void StartMetaGet()
        {
            MetagetHooks hooks = new MetagetHooks
            {
                OnMetafileMeasured = OnMetafileMeasured
            };

            MetaGet =
                new MetagetBuilder()
                    .WithHash(Hash)
                    .WithGlue(Glue.AsMetaGet())
                    .WithPipeline(Pipeline)
                    .WithMetafile(MetaStore.AsMetaGet())
                    .Build(hooks);

            MetaGet.Start();
        }

        private void StartDataStore(string destination)
        {
            RepositoryHooks hooks = new RepositoryHooks
            {
                OnDataAllocated = OnDataAllocated,
                OnDataVerified = OnDataVerified,
                OnBlockWritten = data => DataGet?.Handle(data),
                OnPieceAccepted = data => DataGet?.Handle(data),
                OnPieceRejected = OnPieceRejected
            };

            DataStore =
                new RepositoryBuilder()
                    .WithHash(Hash)
                    .WithDestination(Path.Combine(destination, Hash.ToString()))
                    .WithPipeline(Pipeline)
                    .WithFiles(Files)
                    .WithMemory(Memory.AsDataStore())
                    .WithBufferSize(256 * 1024)
                    .Build(hooks);

            DataStore.Start();
        }

        private void StartDataGet()
        {
            RetrieverHooks hooks = new RetrieverHooks
            {
            };

            DataGet =
                new RetrieverBuilder()
                    .WithHash(Hash)
                    .WithStrategy(Settings.Strategy)
                    .WithGlue(Glue.AsDataGet())
                    .WithPipeline(Pipeline)
                    .WithRepository(DataStore.AsDataGet())
                    .WithOmnibus(DataMap.AsDataGet())
                    .Build(hooks);

            DataGet.Start();
        }

        private void OnMemorySnapshot(MemorySnapshot data)
        {
            Notification notification = new Notification
            {
                Type = NotificationType.MemorySnapshot,
                Size = data.Allocation
            };

            Notifications.Enqueue(notification);
        }

        private void OnConnectionTerminated(ConnectionTerminated data)
        {
            Glue?.Disconnect(data.Connection);
        }

        private void OnHandshakeCompleted(HandshakeCompleted data)
        {
            if (Glue.Connect(data.Connection, data.Handshake) == false)
            {
                data.Connection.Terminate();
            }
        }

        public void OnConnectionEstablished(ConnectionEstablished data)
        {
            Negotiator.Start(data.Connection, new HandshakeNegotiatorActiveInstance(Localhost, Hash, HandshakeOptions.Extended));
        }

        private void OnConnectionArrived(ConnectionArrived data)
        {
            if (Settings.Filter?.Accept(data.Remote) != false)
            {
                Negotiator.Handle(data.Connection, new HandshakeNegotiatorPassiveInstance(Localhost, Hash, HandshakeOptions.Extended));
            }
            else
            {
                Notification notification = new Notification
                {
                    Type = NotificationType.PeerRejected,
                    Remote = data.Remote
                };

                Notifications.Enqueue(notification);
                data.Connection.Terminate();
            }
        }

        private void OnListenerStarted(ListenerStarted data)
        {
            Notification notification = new Notification
            {
                Type = NotificationType.ListenerStarted,
                Remote = PeerAddress.Parse(new IPEndPoint(IPAddress.Any, data.Port))
            };

            Notifications.Enqueue(notification);
            Settings.ListenerPort = data.Port;
        }

        private void OnListenerFailed(ListenerFailed data)
        {
            Notification notification = new Notification
            {
                Type = NotificationType.ListenerFailed,
                Description = data.Reason
            };

            Notifications.Enqueue(notification);
        }

        private void OnMetadataPieceReceived(MetadataReceived data)
        {
            Notification notification = new Notification
            {
                Type = NotificationType.MetafileReceived,
                Piece = new PieceInfo(data.Piece)
            };

            Notifications.Enqueue(notification);
            MetaGet?.Handle(data);
        }

        private void OnMetadataRequestSent(MetadataRequested data)
        {
            Notification notification = new Notification
            {
                Type = NotificationType.MetafileRequested,
                Piece = new PieceInfo(data.Piece)
            };

            Notifications.Enqueue(notification);
        }

        private void OnPeerDataReceived(PeersReceived data)
        {
            foreach (PeerAddress peer in data.Remotes)
            {
                if (Remotes.Add(peer))
                {
                    if (Settings.Filter?.Accept(peer) != false)
                    {
                        Connector?.ConnectTo(Hash, peer);
                    }
                    else
                    {
                        Notification notification = new Notification
                        {
                            Type = NotificationType.PeerRejected,
                            Remote = peer
                        };

                        Notifications.Enqueue(notification);
                    }
                }
            }
        }

        private void OnPeerConnected(PeerConnected data)
        {
            Notification notification = new Notification
            {
                Type = NotificationType.PeerConnected,
                Peer = data.Peer
            };

            Notifications.Enqueue(notification);

            Peers.Add(data.Peer);
            DataMap?.Handle(data);
        }

        private void OnPeerDisconnected(PeerDisconnected data)
        {
            Notification notification = new Notification
            {
                Type = NotificationType.PeerDisconnected,
                Peer = data.Peer
            };

            Notifications.Enqueue(notification);

            Peers.Remove(data.Peer);
            Remotes.Remove(data.Remote);
        }

        private void OnPeerBitfieldChanged(PeerBitfieldChanged data)
        {
            Notification notification = new Notification
            {
                Type = NotificationType.PeerBitfieldChanged,
                Bitfield = data.Bitfield
            };

            Notifications.Enqueue(notification);
            DataMap?.Handle(data);
        }

        private void OnPeerStatusChanged(PeerStatusChanged data)
        {
            Notification notification = new Notification
            {
                Type = NotificationType.PeerStatusChanged,
                Peer = data.Peer,
                State = data.State
            };

            Notifications.Enqueue(notification);
            DataMap?.Handle(data);
        }

        private void OnPieceCompleted(PieceCompleted data)
        {
            Notification notification = new Notification
            {
                Type = NotificationType.PieceCompleted,
                Piece = new PieceInfo(data.Piece)
            };

            Notifications.Enqueue(notification);
        }

        private void OnDataCompleted(DataCompleted data)
        {
            Notification notification = new Notification
            {
                Type = NotificationType.DataCompleted
            };

            Notifications.Enqueue(notification);
        }

        private void OnDataChanged(DataChanged data)
        {
            Notification notification = new Notification
            {
                Type = NotificationType.DataChanged,
                Completed = data.Completed
            };

            Notifications.Enqueue(notification);
        }

        private void OnAnnounced(TrackerAnnounced data)
        {
            foreach (PeerAddress peer in data.Peers)
            {
                if (Remotes.Add(peer))
                {
                    if (Settings.Filter?.Accept(peer) != false)
                    {
                        Connector?.ConnectTo(Hash, peer);
                    }
                    else
                    {
                        Notification notification = new Notification
                        {
                            Type = NotificationType.PeerRejected,
                            Remote = peer
                        };

                        Notifications.Enqueue(notification);
                    }
                }
            }
        }

        private void OnMetafileVerified(MetafileVerified data)
        {
            Notification notification = new Notification
            {
                Type = NotificationType.MetafileCompleted,
                Metainfo = data.Metainfo
            };

            Metainfo = data.Metainfo;
            Notifications.Enqueue(notification);

            Glue?.Handle(data);
            DataStore?.Handle(data);
            DataMap?.Handle(data);
        }

        private void OnMetafileMeasured(MetafileMeasured data)
        {
            Notification notification = new Notification
            {
                Type = NotificationType.MetafileMeasured,
                Size = new Size(data.Size)
            };

            Notifications.Enqueue(notification);
        }

        private void OnDataAllocated(DataAllocated data)
        {
            Notification notification = new Notification
            {
                Type = NotificationType.DataAllocated
            };

            Notifications.Enqueue(notification);
            DataStore?.Verify(new Bitfield(Metainfo.Pieces.Length));
        }

        private void OnDataVerified(DataVerified data)
        {
            Notification notification = new Notification
            {
                Type = NotificationType.DataVerified,
                Bitfield = data.Bitfield
            };

            Notifications.Enqueue(notification);

            DataMap?.Handle(data);
            DataGet?.Handle(data);
            TrackerGet.Announce(Hash);
        }

        private void OnPieceRejected(PieceRejected data)
        {
            Notification notification = new Notification
            {
                Type = NotificationType.PieceRejected,
                Piece = data.Piece
            };

            Notifications.Enqueue(notification);
            DataGet?.Handle(data);
        }
    }
}