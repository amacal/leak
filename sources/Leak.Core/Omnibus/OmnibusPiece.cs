namespace Leak.Core.Omnibus
{
    public interface OmnibusPiece
    {
        int Blocks { get; }

        bool IsComplete();

        bool IsComplete(int block);

        OmnibusPiece Complete();

        OmnibusPiece Complete(int block);

        OmnibusPiece Invalidate();
    }
}