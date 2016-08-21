using Leak.Core.Common;
using Leak.Core.Metadata;
using System;
using System.Collections.Generic;

namespace Leak.Core.Client
{
    public class PeerClient
    {
        private readonly PeerClientContext context;

        public PeerClient(Action<PeerClientConfiguration> configurer)
        {
            context = new PeerClientContext(configurer);
        }

        public void Start(MetainfoFile metainfo)
        {
            foreach (string tracker in metainfo.Trackers)
            {
                context.Telegraph.Start(tracker, with =>
                {
                    with.Peer = context.Peer;
                    with.Hash = metainfo.Data.Hash;
                    with.Port = 8080;
                });
            }

            context.Scheduler.Initialize(with =>
            {
                with.Metainfo = metainfo.Data;
                with.Destination = context.Destination;
            });
        }

        public void Start(Action<PeerClientStartConfiguration> configurer)
        {
            PeerClientStartConfiguration configuration = configurer.Configure(with =>
            {
                with.Trackers = new List<string>();
                with.Peers = new List<PeerAddress>();
            });

            foreach (string tracker in configuration.Trackers)
            {
                context.Telegraph.Start(tracker, with =>
                {
                    with.Peer = context.Peer;
                    with.Hash = configuration.Hash;
                    with.Port = 8080;
                });
            }

            context.Scheduler.Metadata(with =>
            {
                with.Hash = configuration.Hash;
                with.Destination = context.Destination;
            });
        }
    }
}