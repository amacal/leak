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

        public void SendKeepAlive(PeerHash peer)
        {
            storage.GetChannel(peer)?.Send(new KeepAliveMessage());
        }

        public void SendInterested(PeerHash peer)
        {
            storage.GetChannel(peer)?.Send(new InterestedMessage());
        }

        public void SendBitfield(PeerHash peer, Bitfield bitfield)
        {
            storage.GetChannel(peer)?.Send(new BitfieldMessage(bitfield.Length));
        }

        public void SendPieceRequest(PeerHash peer, int piece, int offset, int size)
        {
            storage.GetChannel(peer)?.Send(new RequestMessage(piece, offset, size));
        }

        public void SendExtended(PeerHash peer, Extended extended)
        {
            storage.GetChannel(peer)?.Send(new ExtendedOutgoingMessage(extended));
        }
    }
}