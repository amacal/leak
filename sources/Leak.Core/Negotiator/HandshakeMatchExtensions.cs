using Leak.Common;

namespace Leak.Core.Negotiator
{
    public static class HandshakeMatchExtensions
    {
        public static FileHash Find(this FileHashCollection hashes, HandshakeMatch match)
        {
            foreach (FileHash hash in hashes)
                if (match.Matches(hash))
                    return hash;

            return null;
        }
    }
}