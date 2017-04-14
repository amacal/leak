using System;
using Leak.Networking.Core;

namespace Leak.Peer.Sender.Tests
{
    public class SenderSession : IDisposable
    {
        private readonly SenderService communicator;
        private readonly NetworkConnection sender;
        private readonly NetworkConnection receiver;

        public SenderSession(SenderService communicator, NetworkConnection sender, NetworkConnection receiver)
        {
            this.communicator = communicator;
            this.sender = sender;
            this.receiver = receiver;
        }

        public SenderService Communicator
        {
            get { return communicator; }
        }

        public void Dispose()
        {
            sender.Terminate();
            receiver.Terminate();
        }
    }
}