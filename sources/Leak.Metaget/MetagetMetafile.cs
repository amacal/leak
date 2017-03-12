namespace Leak.Meta.Get
{
    public interface MetagetMetafile
    {
        bool IsCompleted();

        void Write(int piece, byte[] data);

        void Verify();
    }
}