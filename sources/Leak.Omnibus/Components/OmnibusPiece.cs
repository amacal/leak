namespace Leak.Datamap.Components
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