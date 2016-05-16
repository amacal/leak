using Geon;
using Pargos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Leak.Commands
{
    public class AnalyzeCommand : Command
    {
        public override string Name
        {
            get { return "analyze"; }
        }

        public override void Execute(ArgumentCollection arguments)
        {
            Geo geo = GeoFactory.Open();
            Dictionary<string, int> peers = new Dictionary<string, int>();

            string directory = arguments.GetString("event-store");
            EventStore store = new EventStore(directory);

            store.Process(with =>
            {
                with.Handle("announce", @event =>
                {
                    foreach (dynamic peer in @event.data.peers)
                    {
                        string host = peer.host.ToString();
                        GeoData data = geo.Find(host);

                        string code = data?.Code ?? "  ";
                        string value = $"{code}:{host}";

                        if (peers.ContainsKey(value) == false)
                        {
                            peers.Add(value, 0);
                        }

                        peers[value]++;
                    }
                });
            });

            foreach (var entry in peers.OrderByDescending(x => x.Value))
            {
                Console.WriteLine($"{entry.Value.ToString("D2")} = {entry.Key}");
            }
        }
    }
}