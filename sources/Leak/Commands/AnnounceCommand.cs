using Geon;
using Leak.Core;
using Leak.Core.IO;
using Leak.Core.Net;
using Pargos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Leak.Commands
{
    public class AnnounceCommand : Command
    {
        public override string Name
        {
            get { return "announce"; }
        }

        public override void Execute(ArgumentCollection arguments)
        {
            Geo geo = GeoFactory.Open();
            Action<string, object> publish = null;

            if (arguments.Count("event-store") == 1)
            {
                string directory = arguments.GetString("event-store");
                EventStore store = new EventStore(directory);

                publish = store.Publish;
            }

            foreach (AnnounceTask task in AnnounceTaskFactory.Find(arguments))
            {
                PeerAnnounce announce = new PeerAnnounce(with =>
                {
                    with.Hash = task.Hash;
                    with.Peer = task.Hash;

                    if (arguments.Has("ip-address"))
                    {
                        with.SetAddress(arguments.GetString("ip-address"));
                    }
                });

                Console.WriteLine($"Announcing {Bytes.ToString(task.Hash)}.");

                foreach (MetainfoTracker tracker in task.Trackers.Where(TrackerClientFactory.IsSupported))
                {
                    Console.WriteLine($"  {tracker.Uri}");
                    TimeSpan interval = Announce(tracker, announce, geo, publish);

                    Console.WriteLine($"  {tracker.Uri}; next annouce within {Math.Round(interval.TotalMinutes)} minutes.");
                }
            }
        }

        private static TimeSpan Announce(MetainfoTracker tracker, PeerAnnounce announce, Geo geo, Action<string, object> publish)
        {
            try
            {
                TrackerClient client = TrackerClientFactory.Create(tracker);
                TrackerResonse response = client.Announce(announce);

                Dictionary<string, HashSet<string>> peers = new Dictionary<string, HashSet<string>>();
                List<object> hosts = new List<object>();

                foreach (TrackerResponsePeer peer in response.Peers)
                {
                    GeoData location = geo.Find(peer.Host);
                    string country = location?.Code ?? "  ";
                    HashSet<string> collection;

                    if (peers.TryGetValue(country, out collection) == false)
                    {
                        collection = new HashSet<string>();
                        peers.Add(country, collection);
                    }

                    collection.Add(peer.Host);
                    hosts.Add(new { host = peer.Host, port = peer.Port });
                }

                if (publish != null)
                {
                    publish.Invoke("announce", new
                    {
                        hash = Bytes.ToString(announce.Hash),
                        tracker = tracker.Uri.ToString(),
                        peers = hosts.ToArray()
                    });
                }

                foreach (var entry in peers.OrderByDescending(x => x.Value.Count))
                {
                    Console.WriteLine($"    {entry.Key}: {entry.Value.Count}");
                }

                return response.Interval;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    {ex.Message}");
                return TimeSpan.FromMinutes(15);
            }
        }
    }
}