using System;
using Leak.Common;

namespace Leak.Omnibus.Tests
{
    public class OmnibusSession : IDisposable
    {
        private readonly Metainfo metainfo;
        private readonly OmnibusService service;

        public OmnibusSession(Metainfo metainfo, OmnibusService service)
        {
            this.metainfo = metainfo;
            this.service = service;
        }

        public OmnibusService Service
        {
            get { return service; }
        }

        public FileHash Hash
        {
            get { return service.Hash; }
        }

        public OmnibusHooks Hooks
        {
            get { return service.Hooks; }
        }

        public Metainfo Metainfo
        {
            get { return metainfo; }
        }

        public void Dispose()
        {
            service.Dispose();
        }
    }
}
