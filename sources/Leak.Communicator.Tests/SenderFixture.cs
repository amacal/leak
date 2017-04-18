using System;
using Leak.Common;

namespace Leak.Peer.Sender.Tests
{
    public class SenderFixture : IDisposable
    {
        public SenderSession Start()
        {
            SenderService sender =
                new SenderBuilder()
                    .WithHash(FileHash.Random())
                    .WithDefinition(new SenderMessages())
                    .Build();

            return new SenderSession(sender);
        }

        public void Dispose()
        {
        }
    }
}