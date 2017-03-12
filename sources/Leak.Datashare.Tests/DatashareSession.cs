using System;
using F2F.Sandbox;
using Leak.Common;
using Leak.Data.Store;
using Leak.Glue;

namespace Leak.Data.Share.Tests
{
    public class DatashareSession : IDisposable
    {
        private readonly Metainfo metainfo;
        private readonly DatashareData data;
        private readonly IFileSandbox sandbox;
        private readonly DatashareService datashare;
        private readonly RepositoryService repository;
        private readonly GlueService glue;

        public DatashareSession(Metainfo metainfo, DatashareData data, IFileSandbox sandbox, DatashareService datashare, RepositoryService repository, GlueService glue)
        {
            this.metainfo = metainfo;
            this.data = data;
            this.sandbox = sandbox;
            this.datashare = datashare;
            this.repository = repository;
            this.glue = glue;
        }

        public Metainfo Metainfo
        {
            get { return metainfo; }
        }

        public DatashareData Data
        {
            get { return data; }
        }

        public DatashareService Datashare
        {
            get { return datashare; }
        }

        public RepositoryService Repository
        {
            get { return repository; }
        }

        public GlueService Glue
        {
            get { return glue; }
        }

        public void Dispose()
        {
            repository.Dispose();
            datashare.Dispose();
            sandbox.Dispose();
        }
    }
}