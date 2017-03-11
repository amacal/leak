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

                using (PeerClient client = new PeerClient(hash))
                {
                    PeerAddress address = new PeerAddress(options.Host, Int32.Parse(options.Port));
                    PeerSession session = await client.Connect(address);

                    Console.WriteLine($"Hash: {hash} ");
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
                            case PeerNotificationType.Disconnected:
                                Console.WriteLine("disconneced");
                                return;

                            case PeerNotificationType.BitfieldChanged:
                                Console.WriteLine($"Bitfield: {notification.Bitfield.Completed}/{notification.Bitfield.Length} pieces completed");
                                break;

                            case PeerNotificationType.StatusChanged:
                                Console.WriteLine($"Status: {notification.State}");
                                break;

                            case PeerNotificationType.MetadataMeasured:
                                Console.WriteLine($"Metadata: {notification.Size} bytes");
                                break;

                            case PeerNotificationType.MetadataReceived:

                                Console.WriteLine($"Metadata: {notification.Metainfo.Pieces.Length} pieces [{notification.Metainfo.Properties.PieceSize} bytes]");

                                foreach (MetainfoEntry entry in notification.Metainfo.Entries)
                                {
                                    Console.WriteLine($"Metadata: {String.Join(Path.DirectorySeparatorChar.ToString(), entry.Name)} [{entry.Size} bytes]");
                                }

                                break;
                        }
                    }
                }
            }
        }
    }
}