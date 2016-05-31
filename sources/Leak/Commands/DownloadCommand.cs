using Leak.Core.IO;
using Leak.Core.Net;
using Pargos;
using System;
using System.Linq;
using System.Threading;

namespace Leak.Commands
{
    public class DownloadCommand : Command
    {
        public override string Name
        {
            get { return "download"; }
        }

        public override void Execute(ArgumentCollection arguments)
        {
            foreach (DownloadTask task in DownloadTaskFactory.Find(arguments))
            {
                Download(task, arguments);
            }
        }

        private void Download(DownloadTask task, ArgumentCollection arguments)
        {
            TorrentRepository repository = new TorrentRepository(task.Destination);
            PCallback callback = new PCallback(repository);
            PeerNegotiator negotiator = new PeerNegotiatorEncrypted(with =>
            {
            });

            PeerListener listener = new PeerListener(with =>
            {
                with.Port = 8080;
                with.Callback = new CCallback(callback);
                with.Negotiator = negotiator;
                with.Hashes = new PeerNegotiatorHashCollection();
            });

            PeerClientFactory factory = new PeerClientFactory(with =>
            {
                with.Hash = task.Metainfo.Hash;
                with.Callback = new CCallback(callback);
                with.Negotiator = negotiator;
            });

            PeerAnnounce announce = new PeerAnnounce(with =>
            {
                with.Hash = task.Metainfo.Hash;
                with.Peer = task.Metainfo.Hash;

                if (arguments.Has("ip-address"))
                {
                    with.SetAddress(arguments.GetString("ip-address"));
                }
            });

            repository.Initialize();

            if (arguments.Has("enable-listening"))
            {
                listener.Listen();
            }

            while (repository.Completed.Count() != repository.Directory.Pieces.Count)
            {
                foreach (MetainfoTracker tracker in task.Trackers.Where(TrackerClientFactory.IsSupported))
                {
                    try
                    {
                        TrackerClient client = TrackerClientFactory.Create(tracker);
                        TrackerResonse response = client.Announce(announce);

                        foreach (TrackerResponsePeer peer in response.Peers)
                        {
                            if (callback.Peers.Contains(peer) == false)
                            {
                                try
                                {
                                    lock (callback)
                                    {
                                        Console.WriteLine($"Connecting to {peer.Host}:{peer.Port}");
                                        factory.Connect(peer.Host, peer.Port);
                                    }

                                    Thread.Sleep(TimeSpan.FromSeconds(1));
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"{peer.Host}:{peer.Port} {ex.Message}");
                                }
                            }
                        }

                        Thread.Sleep(TimeSpan.FromMinutes(1));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }

                Thread.Sleep(TimeSpan.FromMinutes(10));
            }
        }

        private class CCallback : PeerNegotiatorCallback
        {
            private readonly PCallback callback;

            public CCallback(PCallback callback)
            {
                this.callback = callback;
            }

            public void OnConnect(PeerConnection connection)
            {
            }

            public void OnTerminate(PeerConnection connection)
            {
            }

            public void OnHandshake(PeerConnection connection, PeerHandshake handshake)
            {
                handshake.Accept(callback);
            }
        }

        private class PCallback : PeerCallbackBase
        {
            private readonly TorrentRepository repository;
            private readonly TorrentPieceQueue queue;

            public PCallback(TorrentRepository repository)
                : base(repository.Directory.Pieces)
            {
                this.repository = repository;
                this.queue = new TorrentPieceQueue();
            }

            public override void OnAttached(PeerChannel channel)
            {
                base.OnAttached(channel);
                channel.Send(new PeerInterested());
            }

            public override void OnKeepAlive(PeerChannel channel)
            {
                lock (this)
                {
                    base.OnKeepAlive(channel);
                    Schedule(channel);
                }
            }

            public override void OnUnchoke(PeerChannel channel, PeerUnchoke message)
            {
                lock (this)
                {
                    base.OnUnchoke(channel, message);
                    Schedule(channel);
                }
            }

            public override void OnPiece(PeerChannel channel, PeerPiece message)
            {
                base.OnPiece(channel, message);

                lock (this)
                {
                    repository.Complete(new TorrentBlock((long)message.Piece * repository.Directory.Pieces.Size + message.Offset, message.Size), message.Data);
                }

                TorrentPieceCollection pieces = repository.Directory.Pieces;
                TorrentPiece piece = pieces.ElementAt(message.Piece);

                lock (this)
                {
                    if (piece.IsCompleted(repository))
                    {
                        Peers.ByChannel(channel).Local.Bitfield[piece.Index] = true;

                        queue.Remove(piece);
                        repository.Complete(piece);

                        Schedule(channel);
                    }
                }
            }

            private void Schedule(PeerChannel channel)
            {
                if (Peers.ByChannel(channel).Remote.IsChoking() == false)
                {
                    TorrentPieceView pending = Peers.ByChannel(channel).Pending;
                    TorrentPieceView available = Peers.ByChannel(channel).Available;

                    TorrentPiece next = pending.Intersect(available).Except(queue).Except(repository.Completed).FirstOrDefault();
                    string description = channel.Description.ToString().PadRight(10).Substring(0, 5);

                    if (next != null)
                    {
                        if (queue.Count() < 32)
                        {
                            queue.Add(next);
                            Console.WriteLine($"{description} - requested: {next.Index}; pending: {pending.Count()}; completed: {repository.Completed.Count()}; available: {available.Count()}; queue: {queue.Count()}");

                            foreach (TorrentBlock block in next.Blocks)
                            {
                                channel.Send(new PeerRequest(next.Index, (int)(block.Offset - next.Offset), block.Size));
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine($"{description} - requested: null; pending: {pending.Count()}; completed: {repository.Completed.Count()}; available: {available.Count()}; queue: {queue.Count()}");
                    }
                }
            }
        }
    }
}