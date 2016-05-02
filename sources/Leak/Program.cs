using Leak.Core.IO;
using Leak.Core.Net;
using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace Leak
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            MetainfoFile metainfo = new MetainfoFile(File.ReadAllBytes(args[0]));
            TorrentDirectory destination = new TorrentDirectory(metainfo, args[1]);

            TorrentRepository repository = new TorrentRepository(destination);
            Callback callback = new Callback(repository);

            PeerListener listener = new PeerListener(callback);
            PeerHandshake handshake = new PeerHandshake(metainfo.Hash, metainfo.Hash);

            repository.Initialize();
            listener.Start(handshake);

            while (repository.Completed.Count() != repository.Directory.Pieces.Count)
            {
                try
                {
                    foreach (MetainfoTracker tracker in metainfo.Trackers.Where(x => x.Protocol == MetainfoTrackerProtocol.Http))
                    {
                        TrackerClient client = new TrackerClient(tracker.Uri);
                        TrackerResonse response = client.Announce(metainfo.Hash);

                        foreach (TrackerResponsePeer peer in response.Peers)
                        {
                            if (callback.Peers.Contains(peer) == false)
                            {
                                lock (callback)
                                {
                                    Console.WriteLine($"Connecting to {peer.Host}");
                                    new PeerClient(callback, peer.Host, peer.Port).Start(handshake);
                                }

                                Thread.Sleep(TimeSpan.FromSeconds(1));
                            }
                        }

                        Thread.Sleep(TimeSpan.FromMinutes(1));
                    }
                }
                catch
                {
                }

                Thread.Sleep(TimeSpan.FromMinutes(10));
            }
        }

        private class Callback : PeerCallbackBase
        {
            private readonly TorrentRepository repository;
            private readonly TorrentPieceQueue queue;

            public Callback(TorrentRepository repository)
                : base(repository.Directory.Pieces)
            {
                this.repository = repository;
                this.queue = new TorrentPieceQueue();
            }

            public override void OnHandshake(PeerChannel channel, PeerHandshake handshake)
            {
                base.OnHandshake(channel, handshake);
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

                    if (next != null && queue.Count() < 10)
                    {
                        queue.Add(next);
                        Console.WriteLine($"{description} - requested: {next.Index}; pending: {pending.Count()}; completed: {repository.Completed.Count()}; available: {available.Count()}; queue: {queue.Count()}");

                        foreach (TorrentBlock block in next.Blocks)
                        {
                            channel.Send(new PeerRequest(next.Index, (int)(block.Offset - next.Offset), block.Size));
                        }
                    }
                }
            }
        }
    }
}