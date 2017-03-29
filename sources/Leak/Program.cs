using System;
using System.Threading;
using System.Threading.Tasks;
using Leak.Client;
using Leak.Client.Swarm;
using Leak.Common;
using Pargos;

namespace Leak
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            ThreadPool.SetMinThreads(1, 1);
            ThreadPool.SetMaxThreads(1, 1);

            MainAsync(args).Wait();
        }

        public static async Task MainAsync(string[] args)
        {
            CommandLine options = Argument.Parse<CommandLine>(args);

            if (options.IsValid())
            {
                string[] trackers = options.Trackers;
                FileHash hash = FileHash.Parse(options.Hash);
                SwarmSettings settings = options.ToSettings();

                using (SwarmClient client = new SwarmClient(settings))
                {
                    Reporter reporter = options.ToReporter();
                    SwarmSession session = await client.ConnectAsync(hash, trackers);

                    Console.WriteLine($"Hash: {hash}");

                    switch (options.Command)
                    {
                        case "download":
                            session.Download(options.Destination);
                            break;

                        case "seed":
                            session.Seed(options.Destination);
                            break;
                    }

                    foreach (Notification notification in session.All())
                        if (reporter.Handle(notification) == false)
                            break;
                }
            }
        }
    }
}