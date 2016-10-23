using Leak.Core.Bitfile;
using Leak.Core.Metadata;
using Leak.Files;
using System;

namespace Leak.Core.Repository
{
    public class RepositoryContext
    {
        private readonly RepositoryConfiguration configuration;
        private readonly BitfileService bitfile;
        private readonly RepositoryTaskQueue queue;

        private readonly byte[] buffer;
        private RepositoryView view;

        public RepositoryContext(RepositoryContext context, Action<RepositoryConfiguration> configurer)
        {
            configuration = configurer.Configure(with =>
            {
                with.Metainfo = context?.configuration.Metainfo;
                with.Destination = context?.configuration.Destination;
                with.Files = context?.configuration.Files;
                with.Callback = new RepositoryCallbackNothing();
            });

            bitfile = context?.bitfile ?? new BitfileService(with =>
            {
                with.Hash = configuration.Metainfo.Hash;
                with.Destination = configuration.Destination + ".bitfield";
            });

            view = context?.view;
            queue = context?.queue ?? new RepositoryTaskQueue();
            buffer = context?.buffer ?? new byte[16384];
        }

        public RepositoryConfiguration Configuration
        {
            get { return configuration; }
        }

        public RepositoryCallback Callback
        {
            get { return configuration.Callback; }
        }

        public FileFactory Files
        {
            get { return configuration.Files; }
        }

        public Metainfo Metainfo
        {
            get { return configuration.Metainfo; }
        }

        public string Destination
        {
            get { return configuration.Destination; }
        }

        public RepositoryTaskQueue Queue
        {
            get { return queue; }
        }

        public BitfileService Bitfile
        {
            get { return bitfile; }
        }

        public byte[] Buffer
        {
            get { return buffer; }
        }

        public RepositoryView View
        {
            get { return view; }
            set { view = value; }
        }
    }
}