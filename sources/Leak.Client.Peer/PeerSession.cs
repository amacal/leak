using System.Threading.Tasks;
using Leak.Common;

namespace Leak.Client.Peer
{
    public class PeerSession : Session
    {
        private readonly PeerConnect inner;

        internal PeerSession(PeerConnect inner)
        {
            this.inner = inner;
        }

        public FileHash Hash
        {
            get { return inner.Hash; }
        }

        public PeerHash Peer
        {
            get { return inner.Peer; }
        }

        public void Download(string destination)
        {
            inner.Download(destination);
        }

        public Task<Notification> NextAsync()
        {
            return inner.Notifications.NextAsync();
        }
    }
}