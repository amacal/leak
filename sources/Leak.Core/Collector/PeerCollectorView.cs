using Leak.Core.Common;
using Leak.Core.Messages;
using System.Collections.Generic;

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

        public void SendPieceRequest(PeerHash peer, Request[] requests)
        {
            List<RequestOutgoingMessage> messages = new List<RequestOutgoingMessage>();

            foreach (Request request in requests)
            {
                messages.Add(new RequestOutgoingMessage(request));
            }

            storage.GetChannel(peer)?.Send(messages.ToArray());
        }

        public void SendExtended(PeerHash peer, Extended extended)
        {
            storage.GetChannel(peer)?.Send(new ExtendedOutgoingMessage(extended));
        }
    }
}