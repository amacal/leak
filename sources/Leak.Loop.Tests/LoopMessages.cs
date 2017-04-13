using System.Collections.Generic;

namespace Leak.Peer.Receiver.Tests
{
    public class LoopMessages : ReceiverDefinition
    {
        public string GetName(byte identifier)
        {
            return identifier == 1 ? "found" : null;
        }
    }
}