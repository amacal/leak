using System;
using Leak.Networking.Core;

namespace Leak.Peer.Communicator.Tests
{
    public class CommunicatorSession : IDisposable
    {
        private readonly CommunicatorService communicator;
        private readonly NetworkConnection sender;
        private readonly NetworkConnection receiver;

        public CommunicatorSession(CommunicatorService communicator, NetworkConnection sender, NetworkConnection receiver)
        {
            this.communicator = communicator;
            this.sender = sender;
            this.receiver = receiver;
        }

        public CommunicatorService Communicator
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