using Leak.Core.Common;

namespace Leak.Core.Extensions.PeerExchange
{
    public static class PeerExchangeExtensions
    {
        public static bool MetadataSupports(this ExtenderFormattable formattable, PeerHash peer)
        {
            return formattable.Supports(peer, "ut_pex");
        }
    }
}