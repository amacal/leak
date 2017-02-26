using System;
using Leak.Client.Tracker;
using Leak.Common;
using Pargos;

namespace Leak.Announce
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Options options = Argument.Parse<Options>(args);

            if (options.IsValid())
            {
                Uri tracker = new Uri(options.Tracker);
                TrackerClient client = new TrackerClient(tracker);

                Console.WriteLine("leak-announce 1.0.0");
                Console.WriteLine("by Adrian Macal");
                Console.WriteLine("at https://github.com/amacal/leak");

                foreach (string data in options.Hash)
                {
                    try
                    {
                        FileHash hash = FileHash.Parse(data);

                        Console.WriteLine("");
                        Console.WriteLine($"announcing {hash} at {tracker}");

                        TrackerAnnounce announce = client.AnnounceAsync(hash).Result;

                        Console.WriteLine("");
                        Console.WriteLine($"interval: {announce.Interval}");
                        Console.WriteLine($"peers: {announce.Seeders} seeders and {announce.Leechers} leechers");

                        foreach (PeerAddress peer in announce.Peers)
                        {
                            Console.WriteLine($"  {peer.Host}:{peer.Port}");
                        }
                    }
                    catch (AggregateException ex)
                    {
                        Console.WriteLine($"announcing failed; {ex.InnerExceptions[0].Message}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"announcing failed; {ex.Message}");
                    }
                }
            }
        }
    }
}