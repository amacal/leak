namespace Leak.Core.Retriever
{
    public interface ResourceRetrieverCallback
    {
        void OnCompleted();

        void OnPieceVerified(ResourcePiece piece);
    }
}