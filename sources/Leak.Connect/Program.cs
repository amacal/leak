using System;
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

                using (PeerClient client = new PeerClient(address, hash))
                {
                    PeerConnect connect = await client.Connect();

                    while (true)
                    {
                        PeerNotification notification = await client.Next();

                        switch (notification.Type)
                        {
                            case PeerNotificationType.Disconnected:
                                Console.WriteLine("disconneced");
                                return;

                            case PeerNotificationType.BitfieldChanged:
                                Console.WriteLine("bitfield changed");
                                break;
                        }
                    }
                }
            }
        }
    }
}