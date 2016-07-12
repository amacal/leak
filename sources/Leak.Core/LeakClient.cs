using Leak.Core.IO;
using Leak.Core.Net;
using System;

namespace Leak.Core
{
    public class LeakClient : IDisposable
    {
        private readonly LeakData data;
        private readonly LeakConfiguration configuration;

        public LeakClient(Action<LeakConfiguration> configurer)
        {
            data = new LeakData();
            configuration = new LeakConfiguration();
            configurer.Invoke(configuration);
        }

        public void Start()
        {
            StartListener();
            StartConnections();
        }

        private void StartListener()
        {
            data.Peers = new PeerChannelCollection();

            data.Metainfo = new MetainfoRepository(with =>
            {
                with.Memory();

                foreach (LeakConfigurationSchedule schedule in configuration.Torrents.Schedules)
                {
                    if (schedule.Metainfo != null)
                    {
                        with.Include(schedule.Metainfo);
                    }
                    else
                    {
                        with.Include(schedule.Hash);
                    }
                }

                with.OnCompleted = OnMetainfoRepositoryCompleted;
            });

            data.Listener = new PeerListener(with =>
            {
                with.Port = configuration.Listener.Port;
                with.Negotiator = configuration.Listener.Negotiator;
                with.Callback = new LeakClientNegotiationCallback(configuration, data);
                with.Hashes = configuration.Torrents.Schedules.ToHashCollection();
                with.Options = configuration.Extensions.ToOptions();
            });

            if (configuration.Listener.IsEnabled)
            {
                data.Listener.Listen();
            }
        }

        private void StartConnections()
        {
            foreach (LeakConfigurationPeer peer in configuration.Torrents.Peers)
            {
                PeerClientFactory factory = new PeerClientFactory(with =>
                {
                    with.Negotiator = peer.Negotiator;
                    with.Callback = new LeakClientNegotiationCallback(configuration, data);
                    with.Hash = configuration.Torrents.Schedules.ToHash();
                    with.Options = configuration.Extensions.ToOptions();
                });

                factory.Connect(peer.Host, peer.Port);
            }
        }

        private void OnMetainfoRepositoryCompleted(MetainfoFile metainfo)
        {
            LeakCallbackMetadataDownloaded payload = new LeakCallbackMetadataDownloaded
            {
                Metainfo = metainfo
            };

            configuration.Callback.OnMetadataDownloaded?.Invoke(payload);
        }

        public void Dispose()
        {
            data.Dispose();
        }
    }
}