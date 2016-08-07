namespace Leak.Core.Omnibus
{
    public interface OmnibusPiece
    {
        int Size { get; }

        bool IsComplete();

        bool IsComplete(int block);

        OmnibusPiece Complete();

        OmnibusPiece Complete(int block);

        OmnibusPiece Invalidate();
    }
}