using System.Collections.Generic;
using Leak.Core.Network;

namespace Leak.Core.Net
{
    public class PeerChannelCollection
    {
        private readonly Dictionary<PeerChannel, PeerExtendedMapping> mappings;

        public PeerChannelCollection()
        {
            this.mappings = new Dictionary<PeerChannel, PeerExtendedMapping>();
        }

        public void Connect(NetworkConnection connection)
        {
        }

        public void Terminate(NetworkConnection connection)
        {
        }

        public void Attach(PeerChannel channel)
        {
        }

        public void SetMapping(PeerChannel channel, PeerExtendedMapping mapping)
        {
            mappings[channel] = mapping;
        }

        public PeerExtendedMapping GetMapping(PeerChannel channel)
        {
            PeerExtendedMapping mapping;
            mappings.TryGetValue(channel, out mapping);

            return mapping;
        }
    }
}