using System;
using FakeItEasy;
using Leak.Common;
using Leak.Testing;

namespace Leak.Data.Share.Tests
{
    public class DataShareFixture : IDisposable
    {
        public DataShareSession Start()
        {
            DataShareService datashare =
                new DataShareBuilder()
                    .WithHash(FileHash.Random())
                    .WithPipeline(new PipelineSimulator())
                    .WithGlue(A.Fake<DataShareToGlue>())
                    .WithDataStore(A.Fake<DataShareToDataStore>())
                    .WithDataMap(A.Fake<DataShareToDataMap>())
                    .Build();

            return new DataShareSession(datashare);
        }

        public void Dispose()
        {
        }
    }
}