namespace Leak.Core.Omnibus
{
    public interface OmnibusBitfieldBlock
    {
        int Size { get; }

        bool IsComplete();

        bool IsComplete(int block);

        OmnibusBitfieldBlock Complete();

        OmnibusBitfieldBlock Complete(int block);

        OmnibusBitfieldBlock Invalidate();
    }
}