using System;
using Leak.Common;
using Leak.Networking.Core;

namespace Leak.Peer.Sender.Tests
{
    public class SenderSession : IDisposable
    {
        private readonly SenderService sender;

        public SenderSession(SenderService sender)
        {
            this.sender = sender;
        }

        public FileHash Hash
        {
            get { return FileHash.Random(); }
        }

        public SenderService Sender
        {
            get { return sender; }
        }

        public SenderHooks Hooks
        {
            get { return sender.Hooks; }
        }

        public void Dispose()
        {
        }
    }
}