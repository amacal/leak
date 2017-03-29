using System;
using FakeItEasy;
using Leak.Common;
using Leak.Testing;

namespace Leak.Data.Get.Tests
{
    public class DataGetFixture : IDisposable
    {
        public DataGetSession Start()
        {
            DataGetService service =
                new DataGetBuilder()
                    .WithHash(FileHash.Random())
                    .WithPipeline(new PipelineSimulator())
                    .WithGlue(A.Fake<DataGetToGlue>())
                    .WithDataStore(A.Fake<DataGetToDataStore>())
                    .WithDataMap(A.Fake<DataGetToDataMap>())
                    .Build();

            return new DataGetSession(service);
        }

        public void Dispose()
        {
        }
    }
}