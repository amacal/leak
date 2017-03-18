using System;
using System.IO;
using System.Threading.Tasks;
using Leak.Client.Swarm;
using Leak.Common;
using Pargos;

namespace Leak
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            MainAsync(args).Wait();
        }

        public static async Task MainAsync(string[] args)
        {
            Options options = Argument.Parse<Options>(args);

            if (options.IsValid())
            {
                string[] trackers = options.Trackers;
                FileHash hash = FileHash.Parse(options.Hash);
                SwarmSettings settings = options.ToSettings();

                using (SwarmClient client = new SwarmClient(settings))
                {
                    SwarmSession session = await client.Connect(hash, trackers);

                    Console.WriteLine($"Hash: {hash}");
                    Console.WriteLine();

                    switch (options.Command)
                    {
                        case "download":
                            session.Download(options.Destination);
                            break;
                    }

                    while (true)
                    {
                        SwarmNotification notification = await session.Next();

                        switch (notification.Type)
                        {
                            case SwarmNotificationType.PeerConnected:
                                Console.WriteLine($"Peer: connected {notification.Peer}");
                                break;

                            case SwarmNotificationType.PeerDisconnected:
                                Console.WriteLine($"Peer: disconnected {notification.Peer}");
                                break;

                            case SwarmNotificationType.PeerRejected:
                                Console.WriteLine($"Peer: rejected {notification.Remote}");
                                break;

                            case SwarmNotificationType.PeerBitfieldChanged:
                                Console.WriteLine($"Bitfield: {notification.Bitfield.Completed}/{notification.Bitfield.Length} pieces completed");
                                break;

                            case SwarmNotificationType.PeerStatusChanged:
                                Console.WriteLine($"Status: {notification.State}");
                                break;

                            case SwarmNotificationType.MetafileMeasured:
                                Console.WriteLine($"Metadata: {notification.Size}");
                                break;

                            case SwarmNotificationType.MetafileRequested:
                                Console.WriteLine($"Metadata: requested piece {notification.Piece}");
                                break;

                            case SwarmNotificationType.MetafileReceived:
                                Console.WriteLine($"Metadata: received piece {notification.Piece}");
                                break;

                            case SwarmNotificationType.MetafileCompleted:

                                Console.WriteLine($"Metadata: {notification.Metainfo.Pieces.Length} pieces [{notification.Metainfo.Properties.PieceSize} bytes]");

                                foreach (MetainfoEntry entry in notification.Metainfo.Entries)
                                {
                                    Console.WriteLine($"Metadata: {String.Join(Path.DirectorySeparatorChar.ToString(), entry.Name)} [{entry.Size} bytes]");
                                }

                                break;

                            case SwarmNotificationType.DataAllocated:
                                Console.WriteLine($"Data: allocated");
                                break;

                            case SwarmNotificationType.DataVerified:
                                Console.WriteLine($"Data: verified {notification.Bitfield.Length} pieces");
                                break;

                            case SwarmNotificationType.DataCompleted:
                                Console.WriteLine($"Data: completed");
                                return;

                            case SwarmNotificationType.PieceCompleted:
                                Console.WriteLine($"Data; completed piece {notification.Piece}");
                                break;

                            case SwarmNotificationType.PieceRejected:
                                Console.WriteLine($"Data; rejected piece {notification.Piece}");
                                break;

                            case SwarmNotificationType.MemorySnapshot:
                                Console.WriteLine($"Memory: snapshot {notification.Size}");
                                break;
                        }
                    }
                }
            }
        }
    }
}