using Leak.Core.Retriever;

namespace Leak.Core.Tasking
{
    public class PeerClientTaskDownloadRetrieverCallback : RetrieverCallbackBase
    {
        private readonly PeerClientTaskDownloadContext context;

        public PeerClientTaskDownloadRetrieverCallback(PeerClientTaskDownloadContext context)
        {
            this.context = context;
        }
    }
}