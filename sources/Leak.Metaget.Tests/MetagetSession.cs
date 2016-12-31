using System;
using F2F.Sandbox;
using Leak.Common;
using Leak.Metafile;

namespace Leak.Metaget.Tests
{
    public class MetagetSession : IDisposable
    {
        private readonly IFileSandbox sandbox;
        private readonly string destination;
        private readonly FileHash hash;
        private readonly MetagetData data;
        private readonly MetagetService service;
        private readonly MetafileService metafile;

        public MetagetSession(IFileSandbox sandbox, string destination, FileHash hash, MetagetData data, MetagetService service, MetafileService metafile)
        {
            this.sandbox = sandbox;
            this.destination = destination;
            this.hash = hash;
            this.data = data;
            this.service = service;
            this.metafile = metafile;
        }

        public FileHash Hash
        {
            get { return hash; }
        }

        public MetagetService Service
        {
            get { return service; }
        }

        public MetagetHooks Hooks
        {
            get { return service.Hooks; }
        }

        public int Size
        {
            get { return 10; }
        }

        public MetagetData Data
        {
            get { return data; }
        }

        public string Destination
        {
            get { return destination; }
        }

        public void Dispose()
        {
            metafile.Dispose();
            sandbox.Dispose();
            service.Stop();
        }
    }
}
