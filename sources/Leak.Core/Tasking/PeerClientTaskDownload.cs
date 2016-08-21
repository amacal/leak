using Leak.Core.Common;
using Leak.Core.Retriever;
using System;
using System.IO;

namespace Leak.Core.Tasking
{
    public class PeerClientTaskDownload : PeerClientTask
    {
        private readonly PeerClientTaskDownloadContext inside;

        public PeerClientTaskDownload(Action<PeerClientTaskDownloadContext> configurer)
        {
            inside = configurer.Configure(with =>
            {
                with.Task = this;
            });
        }

        public FileHash Hash
        {
            get { return inside.Metainfo.Hash; }
        }

        public PeerClientTaskCallback Start(PeerClientTaskContext context)
        {
            inside.Retriever = new RetrieverService(with =>
            {
                with.Metainfo = inside.Metainfo;
                with.Bitfield = inside.Bitfield;
                with.Destination = Path.Combine(inside.Destination, $"{inside.Metainfo.Hash}");
                with.Collector = context.Collector.CreateView(inside.Metainfo.Hash);
                with.Callback = new PeerClientTaskDownloadRetrieverCallback(inside);
            });

            inside.Retriever.Start();

            return new PeerClientTaskDownloadTaskCallback(inside);
        }
    }
}