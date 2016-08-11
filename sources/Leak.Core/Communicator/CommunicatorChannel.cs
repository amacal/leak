using Leak.Core.Messages;

namespace Leak.Core.Communicator
{
    public class CommunicatorChannel
    {
        private readonly CommunicatorEntry entry;

        public CommunicatorChannel(CommunicatorEntry entry)
        {
            this.entry = entry;
        }

        public void Send(InterestedMessage message)
        {
            entry.Internal?.Send(message);
        }

        public void Send(BitfieldMessage message)
        {
            entry.Internal?.Send(message);
        }

        public void Send(ExtendedOutgoingMessage message)
        {
            entry.Internal?.Send(message);
        }

        public void Send(RequestOutgoingMessage[] messages)
        {
            entry.Internal?.Send(messages);
        }
    }
}