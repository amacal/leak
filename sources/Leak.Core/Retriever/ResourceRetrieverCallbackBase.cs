namespace Leak.Core.Retriever
{
    public abstract class ResourceRetrieverCallbackBase : ResourceRetrieverCallback
    {
        public virtual void OnCompleted()
        {
        }

        public virtual void OnPieceVerified(ResourcePiece piece)
        {
        }
    }
}