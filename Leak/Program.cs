using Leak.Core.IO;
using Leak.Core.Net;
using Leak.Core.Tests.Resources;
using System;
using System.Linq;

namespace Leak
{
    public static class Program
    {
        public static void Main()
        {
            MetainfoFile metainfo = new MetainfoFile(Files.Ubuntu);
            MetainfoFileTracker tracker = metainfo.Trackers.First();

            TrackerClient client = new TrackerClient(tracker.Uri);
            TrackerResonse response = client.Announce(metainfo.Hash);

            foreach (TrackerResponsePeer peer in response.Peers)
            {
                Console.WriteLine($"{peer.Host}:{peer.Port}");
            }
        }
    }
}