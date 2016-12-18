namespace Leak.Repository.Tests
{
    public static class RepositoryExtensions
    {
        public static void Write(this RepositoryService service, int piece, byte[] data)
        {
            service.Write(new RepositoryBlockData(piece, 0, new FixedDataBlock(data)));
        }
    }
}
