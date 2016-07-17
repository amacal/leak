using Leak.Core.Negotiator;
using Leak.Core.Network;
using System;

namespace Leak.Core.Tests.Stubs
{
    public class HandshakeNegotiatorActiveStub
    {
        private readonly HandshakeNegotiatorActiveStubContext context;
        private readonly HandshakeNegotiatorActive negotiator;

        public HandshakeNegotiatorActiveStub(NetworkConnection connection, Action<HandshakeNegotiatorActiveStubConfiguration> configurer)
        {
            context = new HandshakeNegotiatorActiveStubContext(configurer);
            negotiator = new HandshakeNegotiatorActive(connection, context);
        }

        public HandshakeNegotiatorActiveStubContext Context
        {
            get { return context; }
        }

        public void Execute()
        {
            negotiator.Execute();
        }
    }
}