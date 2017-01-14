using System;
using Leak.Testing;

namespace Leak.Retriever.Tests
{
    public class RetrieverSession : IDisposable
    {
        private readonly RetrieverService retriever;

        public RetrieverSession(RetrieverService retriever)
        {
            this.retriever = retriever;
        }

        public RetrieverService Retriever
        {
            get { return retriever; }
        }

        public PipelineSimulator Pipeline
        {
            get { return (PipelineSimulator)retriever.Dependencies.Pipeline; }
        }

        public RetrieverRepository Repository
        {
            get { return retriever.Dependencies.Repository; }
        }

        public RetrieverGlue Glue
        {
            get { return retriever.Dependencies.Glue; }
        }

        public RetrieverOmnibus Omnibus
        {
            get { return retriever.Dependencies.Omnibus; }
        }

        public void Dispose()
        {
        }
    }
}