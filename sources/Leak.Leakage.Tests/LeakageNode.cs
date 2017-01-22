using F2F.Sandbox;
using Leak.Common;
using System;

namespace Leak.Leakage.Tests
{
    public class LeakageNode : IDisposable
    {
        private readonly LeakHooks hooks;
        private readonly LeakClient client;
        private readonly LeakRegistrant registrant;
        private readonly IFileSandbox sandbox;
        private readonly LeakageEvents events;

        private PeerAddress address;

        public LeakageNode(LeakHooks hooks, LeakClient client, LeakRegistrant registrant, IFileSandbox sandbox, LeakageEvents events)
        {
            this.hooks = hooks;
            this.client = client;
            this.registrant = registrant;
            this.sandbox = sandbox;
            this.events = events;
        }

        public LeakClient Client
        {
            get { return client; }
        }

        public LeakRegistrant Registrant
        {
            get { return registrant; }
        }

        public LeakHooks Hooks
        {
            get { return hooks; }
        }

        public LeakageEvents Events
        {
            get { return events; }
        }

        public PeerAddress Address
        {
            get { return address; }
            set { address = value; }
        }

        public void Dispose()
        {
            client.Dispose();
            sandbox.Dispose();
        }
    }
}