using System;

namespace Leak.Core.Net
{
    public interface PeerNegotiatorAware
    {
        void Receive(Func<PeerMessage, bool> predicate, Action<PeerMessage> callback);

        void Send(PeerMessageFactory data);

        void Handle(PeerHandshake handshake);

        void Remove(int length);

        void Continue(Func<PeerMessage, PeerMessage> encrypt, Func<PeerMessage, PeerMessage> decrypt, Action<PeerBuffer, int> remove);

        void Terminate();
    }
}