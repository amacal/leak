using Leak.Core.Loop;
using Leak.Core.Messages;

namespace Leak.Core.Responder
{
    public class ResponderChannel
    {
        private readonly ConnectionLoopChannel inner;

        public ResponderChannel(ConnectionLoopChannel inner)
        {
            this.inner = inner;
        }

        public void SendKeepAlive()
        {
            inner.Send(new KeepAliveMessage());
        }
    }
}