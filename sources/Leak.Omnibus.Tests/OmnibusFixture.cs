﻿using System;
using System.IO;
using F2F.Sandbox;
using Leak.Common;
using Leak.Metadata;
using Leak.Tasks;

namespace Leak.Omnibus.Tests
{
    public class OmnibusFixture : IDisposable
    {
        private readonly LeakPipeline pipeline;

        public OmnibusFixture()
        {
            pipeline = new LeakPipeline();
            pipeline.Start();
        }

        public OmnibusSession Start()
        {
            Metainfo metainfo;
            byte[] data = Bytes.Random(20000);

            using (FileSandbox temp = new FileSandbox(new EmptyFileLocator()))
            {
                MetainfoBuilder builder = new MetainfoBuilder(temp.Directory);
                string path = temp.CreateFile("debian-8.5.0-amd64-CD-1.iso");

                File.WriteAllBytes(path, data);
                builder.AddFile(path);

                metainfo = builder.ToMetainfo();
            }

            OmnibusService service =
                new OmnibusBuilder()
                    .WithHash(metainfo.Hash)
                    .WithPipeline(pipeline)
                    .Build();

            return new OmnibusSession(metainfo, service);
        }

        public void Dispose()
        {
            pipeline.Stop();
        }
    }
}