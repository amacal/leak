using Leak.Core.Common;
using Leak.Core.Messages;

namespace Leak.Core.Collector
{
    public class PeerCollectorView
    {
        private readonly PeerCollectorStorage storage;

        public PeerCollectorView(PeerCollectorStorage storage)
        {
            this.storage = storage;
        }

        public void SendInterested(PeerHash peer)
        {
            storage?.GetChannel(peer).Send(new InterestedMessage());
        }

        public void SendPieceRequest(PeerHash peer, int piece, int offset, int size)
        {
            storage?.GetChannel(peer).Send(new RequestMessage(piece, offset, size));
        }
    }
}