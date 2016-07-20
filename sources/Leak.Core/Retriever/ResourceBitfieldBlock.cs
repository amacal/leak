namespace Leak.Core.Retriever
{
    public interface ResourceBitfieldBlock
    {
        int Size { get; }

        bool IsComplete();

        bool IsComplete(int block);

        ResourceBitfieldBlock Complete(int block);

        ResourceBitfieldBlock Invalidate();
    }
}