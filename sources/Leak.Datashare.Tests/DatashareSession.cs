using System;
using F2F.Sandbox;
using Leak.Common;
using Leak.Data.Store;
using Leak.Glue;

namespace Leak.Data.Share.Tests
{
    public class DataShareSession : IDisposable
    {
        private readonly Metainfo metainfo;
        private readonly DataShareData data;
        private readonly IFileSandbox sandbox;
        private readonly DataShareService datashare;
        private readonly RepositoryService repository;
        private readonly GlueService glue;

        public DataShareSession(Metainfo metainfo, DataShareData data, IFileSandbox sandbox, DataShareService datashare, RepositoryService repository, GlueService glue)
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

        public DataShareData Data
        {
            get { return data; }
        }

        public DataShareService Datashare
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