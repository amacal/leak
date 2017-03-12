using Leak.Common;

namespace Leak.Data.Store
{
    public class BitfileService
    {
        private readonly BitfileContext context;

        public BitfileService(FileHash hash, string path)
        {
            context = new BitfileContext(hash, path);
        }

        public Bitfield Read()
        {
            return context.Destination.Read();
        }

        public void Write(Bitfield bitfield)
        {
            context.Destination.Write(bitfield);
        }
    }
}