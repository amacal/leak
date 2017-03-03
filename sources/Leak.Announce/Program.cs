using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
            List<Task<TrackerAnnounce>> tasks = new List<Task<TrackerAnnounce>>();

            if (options.IsValid())
            {
                Uri tracker = new Uri(options.Tracker);
                TrackerLogger logger = GetLogger(options);

                using (TrackerClient client = new TrackerClient(tracker, logger))
                {
                    foreach (string data in options.Hash)
                    {
                        FileHash hash = FileHash.Parse(data);
                        tasks.Add(client.AnnounceAsync(hash));
                    }

                    foreach (Task<TrackerAnnounce> task in tasks)
                    {
                        Handle(options, task);
                    }
                }
            }
        }

        private static TrackerLogger GetLogger(Options options)
        {
            if (options.Analyze)
            {
                return new Logger();
            }

            return null;
        }

        private static void Handle(Options options, Task<TrackerAnnounce> task)
        {
            try
            {
                if (options.Analyze == false)
                {
                    Console.WriteLine(task.Result.Hash);
                    Console.WriteLine();

                    foreach (PeerAddress peer in task.Result.Peers)
                    {
                        Console.WriteLine($"  {peer}");
                    }

                    Console.WriteLine();
                }
            }
            catch (AggregateException ex)
            {
                Console.WriteLine(ex.InnerExceptions[0].Message);
            }
        }
    }
}