using System;
using System.Threading.Tasks;
using Leak.Client.Peer;
using Leak.Common;

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
            FileHash hash = FileHash.Parse("11D7CF23BA7AD66845C69FFF32B33FE395ABEBD2");
            PeerAddress address = new PeerAddress("127.0.0.1", 8081);

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