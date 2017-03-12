using System;
using F2F.Sandbox;
using Leak.Common;
using Leak.Testing;

namespace Leak.Meta.Get.Tests
{
    public class MetagetSession : IDisposable
    {
        private readonly IFileSandbox sandbox;
        private readonly FileHash hash;
        private readonly MetagetData data;
        private readonly MetagetService service;

        public MetagetSession(IFileSandbox sandbox, FileHash hash, MetagetData data, MetagetService service)
        {
            this.sandbox = sandbox;
            this.hash = hash;
            this.data = data;
            this.service = service;
        }

        public FileHash Hash
        {
            get { return hash; }
        }

        public MetagetService Service
        {
            get { return service; }
        }

        public PipelineSimulator Pipeline
        {
            get { return (PipelineSimulator)service.Dependencies.Pipeline; }
        }

        public MetagetGlue Glue
        {
            get { return service.Dependencies.Glue; }
        }

        public MetagetMetafile Metafile
        {
            get { return service.Dependencies.Metafile; }
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

        public void Dispose()
        {
            service.Stop();
            service.Dispose();

            sandbox.Dispose();
            Pipeline.Process();
        }
    }
}