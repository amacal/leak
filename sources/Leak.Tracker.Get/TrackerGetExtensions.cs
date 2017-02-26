using System;
using Leak.Common;
using Leak.Events;

namespace Leak.Tracker.Get
{
    public static class TrackerGetExtensions
    {
        public static void CallTrackerAnnounced(this TrackerGetContext context, Uri address, FileHash hash, TimeSpan interval, int seeders, int leachers, PeerAddress[] peers)
        {
            context.Hooks.OnAnnounced?.Invoke(new TrackerAnnounced
            {
                Hash = hash,
                Peer = context.Configuration.Peer,
                Address = address,
                Interval = interval,
                Peers = peers,
                Seeders = seeders,
                Leechers = leachers
            });
        }

        public static void CallTrackerFailed(this TrackerGetContext context, Uri address, FileHash hash, string reason)
        {
            context.Hooks.OnFailed?.Invoke(new TrackerFailed
            {
                Hash = hash,
                Peer = context.Configuration.Peer,
                Address = address,
                Reason = reason
            });
        }
    }
}