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
                PeerHandshake handshake = new PeerHandshake(task.Hash, task.Hash);
                PeerAnnounce announce = new PeerAnnounce(handshake, with =>
                {
                    if (arguments.Has("ip-address"))
                    {
                        with.SetAddress(arguments.GetString("ip-address"));
                    }
                });

                Console.WriteLine($"Announcing {task.Hash}.");

                foreach (MetainfoTracker tracker in task.Trackers.Where(TrackerClientFactory.IsSupported))
                {
                    Console.WriteLine($"  {tracker.Uri}");
                    Announce(tracker, announce, geo, publish);
                }
            }
        }

        private static void Announce(MetainfoTracker tracker, PeerAnnounce announce, Geo geo, Action<string, object> publish)
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
                    string country = location?.Code ?? String.Empty;
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
                        hash = Bytes.ToString(announce.Handshake.Hash),
                        tracker = tracker.Uri.ToString(),
                        peers = hosts.ToArray()
                    });
                }

                foreach (var entry in peers.OrderByDescending(x => x.Value.Count))
                {
                    Console.WriteLine($"    {entry.Key}: {entry.Value.Count}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    {ex.Message}");
            }
        }
    }
}