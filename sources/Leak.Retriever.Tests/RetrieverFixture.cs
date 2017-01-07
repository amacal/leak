using System;
using Leak.Common;
using Leak.Testing;
using Moq;

namespace Leak.Retriever.Tests
{
    public class RetrieverFixture : IDisposable
    {
        public RetrieverSession Start()
        {
            FileHash hash = FileHash.Random();
            PipelineSimulator pipeline = new PipelineSimulator();

            Mock<RetrieverGlue> glue = new Mock<RetrieverGlue>();
            Mock<RetrieverRepository> repository = new Mock<RetrieverRepository>();
            Mock<RetrieverOmnibus> omnibus = new Mock<RetrieverOmnibus>();

            RetrieverService service =
                new RetrieverBuilder()
                    .WithHash(hash)
                    .WithPipeline(pipeline)
                    .WithGlue(glue.Object)
                    .WithRepository(repository.Object)
                    .WithOmnibus(omnibus.Object)
                    .Build();

            return new RetrieverSession(pipeline, service, repository, glue, omnibus);
        }

        public void Dispose()
        {
        }
    }
}
