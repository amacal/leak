using System;
using System.Threading;
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
                TrackerLogger logger = new Logger();

                using (TrackerClient client = new TrackerClient(tracker, logger))
                {
                    foreach (string data in options.Hash)
                    {
                        FileHash hash = FileHash.Parse(data);

                        try
                        {
                            client.AnnounceAsync(hash);
                        }
                        catch (AggregateException ex)
                        {
                            Console.Error.WriteLine($"{hash}:{ex.InnerExceptions[0].Message}");
                        }
                        catch (Exception ex)
                        {
                            Console.Error.WriteLine($"{hash}:{ex.Message}");
                        }
                    }

                    Thread.Sleep(50000);
                }
            }
        }
    }
}