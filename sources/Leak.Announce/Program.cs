using System;
using System.Linq;
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
                using (TrackerClient client = new TrackerClient())
                {
                    TrackerSession session = client.ConnectAsync(options.Tracker).Result;
                    session.Announce(options.Hashes.Select(FileHash.Parse).ToArray());

                    while (true)
                    {
                        Console.WriteLine(session.NextAsync().Result);
                    }
                }
            }
        }
    }
}