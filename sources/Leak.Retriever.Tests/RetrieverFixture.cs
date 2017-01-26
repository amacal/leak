using System;
using FakeItEasy;
using Leak.Common;
using Leak.Testing;

namespace Leak.Dataget.Tests
{
    public class RetrieverFixture : IDisposable
    {
        public RetrieverSession Start()
        {
            RetrieverService service =
                new RetrieverBuilder()
                    .WithHash(FileHash.Random())
                    .WithPipeline(new PipelineSimulator())
                    .WithGlue(A.Fake<RetrieverGlue>())
                    .WithRepository(A.Fake<RetrieverRepository>())
                    .WithOmnibus(A.Fake<RetrieverOmnibus>())
                    .Build();

            return new RetrieverSession(service);
        }

        public void Dispose()
        {
        }
    }
}