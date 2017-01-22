using System;

namespace Leak.Metashare.Tests
{
    public class MetashareSession : IDisposable
    {
        private readonly MetashareService service;

        public MetashareSession(MetashareService service)
        {
            this.service = service;
        }

        public MetashareService Service
        {
            get { return service; }
        }

        public MetashareHooks Hooks
        {
            get { return service.Hooks; }
        }

        public void Dispose()
        {
            service.Stop();
            service.Dispose();
        }
    }
}