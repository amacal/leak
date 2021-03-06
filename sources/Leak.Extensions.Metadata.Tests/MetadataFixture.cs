﻿using Leak.Common;
using Leak.Tasks;
using System;
using Leak.Peer.Coordinator;

namespace Leak.Extensions.Metadata.Tests
{
    public class MetadataFixture : IDisposable
    {
        private readonly LeakPipeline pipeline;

        public MetadataFixture()
        {
            pipeline = new LeakPipeline();
            pipeline.Start();
        }

        public MetadataSession Start()
        {
            MetadataHooks hooks = new MetadataHooks();
            MetadataPlugin plugin = new MetadataPlugin(hooks);

            CoordinatorService coordinator =
                new CoordinatorBuilder()
                    .WithHash(FileHash.Random())
                    .WithPipeline(pipeline)
                    .WithPlugin(plugin)
                    .Build();

            return new MetadataSession(coordinator, plugin);
        }

        public void Dispose()
        {
            pipeline?.Stop();
        }
    }
}