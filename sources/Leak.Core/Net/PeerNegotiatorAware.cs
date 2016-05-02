using System;

namespace Leak.Core.Net
{
    public interface PeerNegotiatorAware
    {
        void Receive(Action<PeerMessage> message);

        void Send(PeerMessageFactory data);

        void Handle(PeerHandshake handshake);

        void Remove(PeerMessage message);

        void Continue();

        void Terminate();
    }
}