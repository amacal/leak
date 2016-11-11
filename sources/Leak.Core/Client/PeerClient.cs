using Leak.Core.Client.Events;
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
            context.Scheduler.Initialize(with =>
            {
                with.Metainfo = metainfo.Data;
                with.Destination = context.Destination;
            });

            context.Hashes.Add(metainfo.Data.Hash);
            context.Bus.Publish("file-scheduled", new FileScheduled
            {
                Hash = metainfo.Data.Hash
            });

            foreach (string tracker in metainfo.Trackers)
            {
                context.Telegraph.Register(tracker, with =>
                {
                    with.Hash = metainfo.Data.Hash;
                });
            }
        }

        public void Start(Action<PeerClientStartConfiguration> configurer)
        {
            PeerClientStartConfiguration configuration = configurer.Configure(with =>
            {
                with.Trackers = new List<string>();
                with.Peers = new List<PeerAddress>();
            });

            context.Scheduler.Metadata(with =>
            {
                with.Hash = configuration.Hash;
                with.Destination = context.Destination;
            });

            context.Hashes.Add(configuration.Hash);
            context.Bus.Publish("file-scheduled", new FileScheduled
            {
                Hash = configuration.Hash
            });

            foreach (string tracker in configuration.Trackers)
            {
                context.Telegraph.Register(tracker, with =>
                {
                    with.Hash = configuration.Hash;
                });
            }
        }
    }
}