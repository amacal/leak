using Leak.Core.Negotiator;
using Leak.Core.Network;
using System;

namespace Leak.Core.Tests.Stubs
{
    public class HandshakeNegotiatorPassiveStub
    {
        private readonly HandshakeNegotiatorPassiveStubContext context;
        private readonly HandshakeNegotiatorPassive negotiator;

        public HandshakeNegotiatorPassiveStub(NetworkConnection connection, Action<HandshakeNegotiatorPassiveStubConfiguration> configurer)
        {
            context = new HandshakeNegotiatorPassiveStubContext(configurer);
            negotiator = new HandshakeNegotiatorPassive(connection, context);
        }

        public HandshakeNegotiatorPassiveStubContext Context
        {
            get { return context; }
        }

        public void Execute()
        {
            negotiator.Execute();
        }
    }
}