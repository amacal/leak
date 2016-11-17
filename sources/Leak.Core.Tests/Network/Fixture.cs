using F2F.Sandbox;
using Leak.Core.Common;
using Leak.Core.Metadata;

namespace Leak.Core.Tests.Network
{
    public static class Fixture
    {
        public static void Metainfo(out FileHash hash, out byte[] metadata)
        {
            using (FileSandbox forBuilder = Sandbox.New())
            {
                MetainfoBuilder builder = new MetainfoBuilder(forBuilder.Directory);
                string file = forBuilder.CreateFile("abc.txt");

                builder.AddFile(file);

                hash = builder.ToHash();
                metadata = builder.ToBytes();
            }
        }
    }
}