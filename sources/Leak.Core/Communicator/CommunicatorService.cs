using Leak.Core.Common;
using Leak.Core.Messages;
using Leak.Core.Network;

namespace Leak.Core.Communicator
{
    public class CommunicatorService
    {
        private readonly PeerHash peer;
        private readonly NetworkConnection connection;
        private readonly CommunicatorHooks hooks;
        private readonly CommunicatorConfiguration configuration;

        public CommunicatorService(PeerHash peer, NetworkConnection connection, CommunicatorHooks hooks, CommunicatorConfiguration configuration)
        {
            this.peer = peer;
            this.connection = connection;
            this.hooks = hooks;
            this.configuration = configuration;
        }

        public void SendKeepAlive()
        {
            connection.Send(new KeepAliveMessage());
        }

        public void SendChoke()
        {
            connection.Send(new ChokeMessage());
        }

        public void SendUnchoke()
        {
            connection.Send(new UnchokeMessage());
        }

        public void SendInterested()
        {
            connection.Send(new InterestedMessage());
        }

        public void SendHave(int piece)
        {
            connection.Send(new HaveOutgoingMessage(piece));
        }

        public void SendBitfield(Bitfield bitfield)
        {
            connection.Send(new BitfieldOutgoingMessage(bitfield));
        }

        public void SendPiece(Piece piece)
        {
            connection.Send(new PieceOutgoingMessage(piece));
        }

        public void SendExtended(Extended extended)
        {
            connection.Send(new ExtendedOutgoingMessage(extended));
        }
    }
}