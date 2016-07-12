using Leak.Core.IO;
using Leak.Core.Net;
using System;

namespace Leak.Core
{
    public static class LeakConfigurationExtensions
    {
        public static void Listener(this LeakConfiguration configuration, Action<LeakConfigurationListener> listener)
        {
            configuration.Listener.IsEnabled = true;
            configuration.Listener.Negotiator = new PeerNegotiatorEncrypted(with => { });

            listener.Invoke(configuration.Listener);
        }

        public static void Port(this LeakConfigurationListener configuration, int port)
        {
            configuration.Port = port;
        }

        public static void OnlyEncrypted(this LeakConfigurationListener configuration)
        {
            configuration.Negotiator = new PeerNegotiatorEncrypted(with =>
            {
            });
        }

        public static void Torrents(this LeakConfiguration configuration, Action<LeakConfigurationTorrentCollection> torrents)
        {
            torrents.Invoke(configuration.Torrents);
        }

        public static void Schedule(this LeakConfigurationTorrentCollection configuration, Action<LeakConfigurationSchedule> callback)
        {
            LeakConfigurationSchedule schedule = new LeakConfigurationSchedule
            {
                Operation = LeakConfigurationScheduleOperation.Nothing
            };

            callback.Invoke(schedule);
            configuration.Schedules.Add(schedule);
        }

        public static void Schedule(this LeakConfigurationTorrentCollection configuration, byte[] hash)
        {
            configuration.Schedule(with =>
            {
                with.Hash(hash);
            });
        }

        public static void Hash(this LeakConfigurationSchedule configuration, byte[] hash)
        {
            configuration.Hash = hash;
        }

        public static void Metainfo(this LeakConfigurationSchedule configuration, MetainfoFile metainfo)
        {
            configuration.Hash = metainfo.Hash;
            configuration.Metainfo = metainfo;
        }

        public static void DownloadMetadata(this LeakConfigurationSchedule configuration)
        {
            configuration.Operation = LeakConfigurationScheduleOperation.Metadata;
        }

        public static void Peer(this LeakConfigurationTorrentCollection configuration, string host, int port)
        {
            configuration.Peer(peer =>
            {
                peer.Remote(host, port);
            });
        }

        public static void Peer(this LeakConfigurationTorrentCollection configuration, Action<LeakConfigurationPeer> configurer)
        {
            LeakConfigurationPeer peer = new LeakConfigurationPeer();

            peer.Negotiator = new PeerNegotiatorEncrypted(with => { });
            configurer.Invoke(peer);

            configuration.Peers.Add(peer);
        }

        public static void Remote(this LeakConfigurationPeer configuration, string host, int port)
        {
            configuration.Host = host;
            configuration.Port = port;
        }

        public static void Encrypted(this LeakConfigurationPeer configuration)
        {
            configuration.Negotiator = new PeerNegotiatorEncrypted(x => { });
        }

        public static void Callback(this LeakConfiguration configuration, Action<LeakConfigurationCallback> callback)
        {
            callback.Invoke(configuration.Callback);
        }

        public static void HandshakeExchanged(this LeakConfigurationCallback configuration, Action<LeakCallbackHandshakeExchanged> callback)
        {
            configuration.OnHandshakeExchanged = callback.Invoke;
        }

        public static void ExtensionsExchanged(this LeakConfigurationCallback configuration, Action<LeakCallbackExtensionsExchanged> callback)
        {
            configuration.OnExtensionsExchanged = callback.Invoke;
        }

        public static void MetadataDownloaded(this LeakConfigurationCallback configuration, Action<LeakCallbackMetadataDownloaded> callback)
        {
            configuration.OnMetadataDownloaded = callback.Invoke;
        }

        public static void Extensions(this LeakConfiguration configuration, Action<LeakConfigurationExtensionCollection> extensions)
        {
            configuration.Extensions.IsEnabled = true;
            extensions.Invoke(configuration.Extensions);
        }

        public static void Metadata(this LeakConfigurationExtensionCollection configuration)
        {
            configuration.SupportsMetadata = true;
        }
    }
}