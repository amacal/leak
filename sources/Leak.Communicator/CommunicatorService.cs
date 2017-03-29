using System;
using Leak.Common;
using Leak.Communicator.Messages;

namespace Leak.Communicator
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
            SendAndCall(new KeepAliveMessage(), "keep-alive");
        }

        public void SendChoke()
        {
            SendAndCall(new ChokeMessage(), "choke");
        }

        public void SendUnchoke()
        {
            SendAndCall(new UnchokeMessage(), "unchoke");
        }

        public void SendInterested()
        {
            SendAndCall(new InterestedMessage(), "interested");
        }

        public void SendHave(int piece)
        {
            SendAndCall(new HaveOutgoingMessage(piece), "have");
        }

        public void SendBitfield(Bitfield bitfield)
        {
            SendAndCall(new BitfieldOutgoingMessage(bitfield), "bitfield");
        }

        public void SendRequest(Request request)
        {
            SendAndCall(new RequestOutgoingMessage(request), "request");
        }

        public void SendPiece(Piece piece)
        {
            SendAndCall(new PieceOutgoingMessage(piece), "piece");
        }

        public void SendExtended(Extended extended)
        {
            SendAndCall(new ExtendedOutgoingMessage(extended), "extended");
        }

        private void SendAndCall(NetworkOutgoingMessage message, string type)
        {
            connection.Send(message);
            hooks.CallMessageSent(peer, type, message);
        }
    }
}