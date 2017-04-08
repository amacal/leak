using System;
using System.Net;
using Leak.Common;
using Leak.Networking.Core;
using Leak.Tracker.Get.Events;

namespace Leak.Tracker.Get
{
    public static class TrackerGetExtensions
    {
        public static void CallAnnounced(this TrackerGetContext context, Uri address, FileHash hash, TimeSpan interval, int seeders, int leachers, NetworkAddress[] peers)
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

        public static void CallAnnounced(this TrackerGetContext context, Uri address, FileHash hash, TimeSpan interval, NetworkAddress[] peers)
        {
            context.Hooks.OnAnnounced?.Invoke(new TrackerAnnounced
            {
                Hash = hash,
                Peer = context.Configuration.Peer,
                Address = address,
                Interval = interval,
                Peers = peers
            });
        }

        public static void CallConnected(this TrackerGetContext context, Uri address, FileHash hash, byte[] transaction, byte[] connection)
        {
            context.Hooks.OnConnected?.Invoke(new TrackerConnected
            {
                Hash = hash,
                Peer = context.Configuration.Peer,
                Address = address,
                Transaction = transaction,
                Connection = connection
            });
        }

        public static void CallConnected(this TrackerGetContext context, Uri address, FileHash hash)
        {
            context.Hooks.OnConnected?.Invoke(new TrackerConnected
            {
                Hash = hash,
                Peer = context.Configuration.Peer,
                Address = address
            });
        }

        public static void CallTimeout(this TrackerGetContext context, Uri address, FileHash hash)
        {
            context.Hooks.OnTimeout?.Invoke(new TrackerTimeout
            {
                Hash = hash,
                Peer = context.Configuration.Peer,
                Address = address,
                Seconds = context.Configuration.Timeout
            });
        }

        public static void CallFailed(this TrackerGetContext context, Uri address, FileHash hash, string reason)
        {
            context.Hooks.OnFailed?.Invoke(new TrackerFailed
            {
                Hash = hash,
                Peer = context.Configuration.Peer,
                Address = address,
                Reason = reason
            });
        }

        public static void CallPacketSent(this TrackerGetContext context, Uri address, FileHash hash, IPEndPoint endpoint, int size)
        {
            context.Hooks.OnPacketSent?.Invoke(new TrackerPacketSent
            {
                Hash = hash,
                Peer = context.Configuration.Peer,
                Address = address,
                Endpoint = endpoint,
                Size = size
            });
        }

        public static void CallPacketReceived(this TrackerGetContext context, IPEndPoint endpoint, int size)
        {
            context.Hooks.OnPacketReceived?.Invoke(new TrackerPacketReceived
            {
                Endpoint = endpoint,
                Size = size
            });
        }

        public static void CallPacketIgnored(this TrackerGetContext context, IPEndPoint endpoint, int size)
        {
            context.Hooks.OnPacketIgnored?.Invoke(new TrackerPacketIgnored
            {
                Endpoint = endpoint,
                Size = size
            });
        }
    }
}