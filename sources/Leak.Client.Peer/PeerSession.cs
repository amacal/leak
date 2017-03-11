using System.Threading.Tasks;
using Leak.Common;

namespace Leak.Client.Peer
{
    public class PeerSession
    {
        private readonly PeerConnect session;

        public PeerSession(PeerConnect session)
        {
            this.session = session;
        }

        public PeerHash Peer
        {
            get { return session.Peer; }
        }

        public void Download(string destination)
        {
        }

        public Task<PeerNotification> Next()
        {
            return session.Notifications.Next();
        }
    }
}