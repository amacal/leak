using System.Threading.Tasks;
using Leak.Common;

namespace Leak.Client.Peer
{
    public class PeerSession
    {
        private readonly PeerConnect inner;

        public PeerSession(PeerConnect inner)
        {
            this.inner = inner;
        }

        public PeerHash Peer
        {
            get { return inner.Peer; }
        }

        public void Download(string destination)
        {
            inner.Download(destination);
        }

        public Task<PeerNotification> Next()
        {
            return inner.Notifications.Next();
        }
    }
}