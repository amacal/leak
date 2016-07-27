namespace Leak.Core.Retriever
{
    public interface ResourceQueueItem
    {
        void Handle(ResourceQueueContext context);
    }
}