using System.Collections.Generic;

namespace Leak.Core.Negotiator
{
    public class HandshakeHashCollection
    {
        private readonly List<HandshakeHash> hashes;

        public HandshakeHashCollection(params HandshakeHash[] items)
        {
            this.hashes = new List<HandshakeHash>(items);
        }

        public HandshakeHash Find(HandshakeHashMatch match)
        {
            foreach (HandshakeHash hash in hashes)
                if (match.Matches(hash))
                    return hash;

            return null;
        }
    }
}