using Leak.Core.Extensions;
using Leak.Core.Extensions.PeerExchange;
using System.Collections.Generic;

namespace Leak.Core.Client
{
    public class PeerClientExtensionPeerExchange : PeerClientExtension
    {
        public void Register(ICollection<ExtenderHandler> handlers, PeerClientExtensionContext context)
        {
            handlers.Add(new PeerExchangeHandler(with =>
            {
                with.Callback = new PeerClientToPeerExchange(context);
            }));
        }

        public void Register(ICollection<string> extensions)
        {
            extensions.Add("ut_pex");
        }
    }
}