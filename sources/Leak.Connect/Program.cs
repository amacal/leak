using System;
using System.IO;
using System.Threading.Tasks;
using Leak.Client.Peer;
using Leak.Common;
using Pargos;

namespace Leak.Connect
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
                FileHash hash = FileHash.Parse(options.Hash);
                PeerAddress address = new PeerAddress(options.Host, Int32.Parse(options.Port));

                using (PeerClient client = new PeerClient())
                {
                    PeerSession session = await client.Connect(hash, address);

                    Console.WriteLine($"Hash: {hash}");
                    Console.WriteLine($"Peer: {session.Peer}");
                    Console.WriteLine();

                    switch (options.Command)
                    {
                        case "download":
                            session.Download(options.Destination);
                            break;
                    }

                    while (true)
                    {
                        PeerNotification notification = await session.Next();

                        switch (notification.Type)
                        {
                            case PeerNotificationType.PeerDisconnected:
                                Console.WriteLine("disconneced");
                                return;

                            case PeerNotificationType.PeerBitfieldChanged:
                                Console.WriteLine($"Bitfield: {notification.Bitfield.Completed}/{notification.Bitfield.Length} pieces completed");
                                break;

                            case PeerNotificationType.PeerStatusChanged:
                                Console.WriteLine($"Status: {notification.State}");
                                break;

                            case PeerNotificationType.MetafileMeasured:
                                Console.WriteLine($"Metadata: {notification.Size} bytes");
                                break;

                            case PeerNotificationType.MetafileRequested:
                                Console.WriteLine($"Metadata: requested piece {notification.Piece}");
                                break;

                            case PeerNotificationType.MetafileReceived:
                                Console.WriteLine($"Metadata: received piece {notification.Piece}");
                                break;

                            case PeerNotificationType.MetafileCompleted:

                                Console.WriteLine($"Metadata: {notification.Metainfo.Pieces.Length} pieces [{notification.Metainfo.Properties.PieceSize} bytes]");

                                foreach (MetainfoEntry entry in notification.Metainfo.Entries)
                                {
                                    Console.WriteLine($"Metadata: {String.Join(Path.DirectorySeparatorChar.ToString(), entry.Name)} [{entry.Size} bytes]");
                                }

                                break;

                            case PeerNotificationType.DataAllocated:
                                Console.WriteLine($"Data: allocated");
                                break;

                            case PeerNotificationType.DataVerified:
                                Console.WriteLine($"Data: verified {notification.Bitfield.Length} pieces");
                                break;

                            case PeerNotificationType.DataCompleted:
                                Console.WriteLine($"Data: completed");
                                break;

                            case PeerNotificationType.PieceCompleted:
                                Console.WriteLine($"Data; completed piece {notification.Piece}");
                                break;
                        }
                    }
                }
            }
        }
    }
}