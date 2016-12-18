using System;
using Leak.Common;

namespace Leak.Omnibus.Tests
{
    public class OmnibusSession : IDisposable
    {
        private readonly FileHash hash;
        private readonly OmnibusService service;
        private readonly OmnibusHooks hooks;

        public OmnibusSession(FileHash hash, OmnibusService service, OmnibusHooks hooks)
        {
            this.hash = hash;
            this.service = service;
            this.hooks = hooks;
        }

        public OmnibusHooks Hooks
        {
            get { return hooks; }
        }

        public OmnibusService Service
        {
            get { return service; }
        }

        public FileHash Hash
        {
            get { return hash; }
        }

        public void Dispose()
        {
            service.Dispose();
        }
    }
}
