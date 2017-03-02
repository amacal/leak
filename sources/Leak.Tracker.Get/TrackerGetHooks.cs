using System;
using Leak.Tracker.Get.Events;

namespace Leak.Tracker.Get
{
    public class TrackerGetHooks
    {
        /// <summary>
        /// Called when the service successfully announced hash at the tracker.
        /// </summary>
        public Action<TrackerAnnounced> OnAnnounced;

        /// <summary>
        /// Called when the service reached the timeout when announcing the hash.
        /// </summary>
        public Action<TrackerTimeout> OnTimeout;

        /// <summary>
        /// Called when the service successfully receive connection response from an udp tracker.
        /// </summary>
        public Action<TrackerConnected> OnConnected;

        /// <summary>
        /// Called when the service received error response from the tracker.
        /// </summary>
        public Action<TrackerFailed> OnFailed;

        /// <summary>
        /// Called when the service successfully sent a packet to the remote endpoint.
        /// </summary>
        public Action<TrackerPacketSent> OnPacketSent;

        /// <summary>
        /// Called when the service received some packet from the remote endpoint.
        /// </summary>
        public Action<TrackerPacketReceived> OnPacketReceived;

        /// <summary>
        /// Called when the service received some packet and could not handle it.
        /// </summary>
        public Action<TrackerPacketIgnored> OnPacketIgnored;
    }
}