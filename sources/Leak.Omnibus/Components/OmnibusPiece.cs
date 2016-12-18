namespace Leak.Omnibus.Components
{
    public interface OmnibusPiece
    {
        int Blocks { get; }

        bool IsReady();

        bool IsComplete();

        bool IsComplete(int block);

        OmnibusPiece Complete(int block);

        OmnibusPiece Invalidate();

        OmnibusPiece Complete();
    }
}