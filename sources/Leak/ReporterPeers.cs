using System.Collections.Generic;
using System.Linq;
using Leak.Client.Notifications;
using Leak.Common;

namespace Leak
{
    public class ReporterPeers
    {
        public ReporterPeers()
        {
            Total = new HashSet<PeerHash>();

            LocalChoking = new HashSet<PeerHash>();
            LocalInterested = new HashSet<PeerHash>();

            RemoteChoking = new HashSet<PeerHash>();
            RemoteInterested = new HashSet<PeerHash>();
        }

        public HashSet<PeerHash> Total { get; set; }

        public HashSet<PeerHash> LocalChoking { get; set; }
        public HashSet<PeerHash> LocalInterested { get; set; }

        public HashSet<PeerHash> RemoteChoking { get; set; }
        public HashSet<PeerHash> RemoteInterested { get; set; }

        public void Handle(PeerConnectedNotification notification)
        {
            Total.Add(notification.Peer);
        }

        public void Handle(PeerDisconnectedNotification notification)
        {
            Total.Remove(notification.Peer);

            LocalChoking.Remove(notification.Peer);
            LocalInterested.Remove(notification.Peer);

            RemoteChoking.Remove(notification.Peer);
            RemoteInterested.Remove(notification.Peer);
        }

        public void Handle(StatusChangedNotification notification)
        {
            Toggle(notification.State.IsLocalChokingRemote, notification.Peer, LocalChoking);
            Toggle(notification.State.IsLocalInterestedInRemote, notification.Peer, LocalInterested);
            Toggle(notification.State.IsRemoteChokingLocal, notification.Peer, RemoteChoking);
            Toggle(notification.State.IsRemoteInterestedInLocal, notification.Peer, RemoteInterested);
        }

        private static void Toggle(bool flag, PeerHash peer, HashSet<PeerHash> peers)
        {
            if (flag)
            {
                peers.Add(peer);
            }
            else
            {
                peers.Remove(peer);
            }
        }

        public override string ToString()
        {
            return $"peers: {Total.Count}, LC, {LocalChoking.Count}, LI: {LocalInterested.Count}, RC: {RemoteChoking.Count}, RI: {RemoteInterested.Count}";
        }
    }
}