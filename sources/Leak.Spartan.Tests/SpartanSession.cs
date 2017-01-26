using Leak.Common;
using System;
using Leak.Testing;

namespace Leak.Spartan.Tests
{
    public class SpartanSession : IDisposable
    {
        private readonly Metainfo metainfo;
        private readonly SpartanMeta meta;
        private readonly SpartanData data;
        private readonly SpartanService spartan;
        private readonly SpartanStage stage;

        public SpartanSession(Metainfo metainfo, SpartanMeta meta, SpartanData data, SpartanService spartan, SpartanStage stage)
        {
            this.metainfo = metainfo;
            this.meta = meta;
            this.data = data;
            this.spartan = spartan;
            this.stage = stage;
        }

        public SpartanService Spartan
        {
            get { return spartan; }
        }

        public SpartanHooks Hooks
        {
            get { return spartan.Hooks; }
        }

        public FileHash Hash
        {
            get { return meta.Hash; }
        }

        public SpartanMeta Meta
        {
            get { return meta; }
        }

        public SpartanData Data
        {
            get { return data; }
        }

        public SpartanStage Stage
        {
            get { return stage; }
        }

        public Metainfo Metainfo
        {
            get { return metainfo; }
        }

        public PipelineSimulator Pipeline
        {
            get { return (PipelineSimulator)spartan.Dependencies.Pipeline; }
        }

        public SpartanMetaget Metaget
        {
            get { return spartan.Dependencies.Metaget; }
        }

        public SpartanMetashare Metashare
        {
            get { return spartan.Dependencies.Metashare; }
        }

        public SpartanRetriever Retriever
        {
            get { return spartan.Dependencies.Retriever; }
        }

        public SpartanRepository Repository
        {
            get { return spartan.Dependencies.Repository; }
        }

        public void Dispose()
        {
            spartan.Dispose();
            Pipeline.Process();
        }
    }
}