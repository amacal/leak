using System;
using System.Threading.Tasks;
using Leak.Client;
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
                    Notification notification = null;
                    PeerSession session = await client.ConnectAsync(hash, address);

                    Console.WriteLine($"Hash: {hash}");
                    Console.WriteLine($"Peer: {session.Peer}");
                    Console.WriteLine();

                    switch (options.Command)
                    {
                        case "download":
                            session.Download(options.Destination);
                            break;
                    }

                    while (notification?.Type != NotificationType.DataCompleted)
                    {
                        notification = await session.NextAsync();
                        Console.WriteLine(notification);
                    }
                }
            }
        }
    }
}