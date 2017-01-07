using System;
using Leak.Testing;
using Moq;

namespace Leak.Retriever.Tests
{
    public class RetrieverSession : IDisposable
    {
        private readonly PipelineSimulator pipeline;
        private readonly RetrieverService retriever;
        private readonly Mock<RetrieverRepository> repository;
        private readonly Mock<RetrieverGlue> glue;
        private readonly Mock<RetrieverOmnibus> omnibus;

        public RetrieverSession(PipelineSimulator pipeline, RetrieverService retriever, Mock<RetrieverRepository> repository, Mock<RetrieverGlue> glue, Mock<RetrieverOmnibus> omnibus)
        {
            this.pipeline = pipeline;
            this.retriever = retriever;
            this.repository = repository;
            this.glue = glue;
            this.omnibus = omnibus;
        }

        public RetrieverService Retriever
        {
            get { return retriever; }
        }

        public Mock<RetrieverRepository> Repository
        {
            get { return repository; }
        }

        public Mock<RetrieverGlue> Glue
        {
            get { return glue; }
        }

        public Mock<RetrieverOmnibus> Omnibus
        {
            get { return omnibus; }
        }

        public PipelineSimulator Pipeline
        {
            get { return pipeline; }
        }

        public void Dispose()
        {
        }
    }
}
