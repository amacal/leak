using Leak.Core.Extensions;
using System.Collections.Generic;

namespace Leak.Core.Client
{
    public static class PeerClientConfigurationExtensions
    {
        public static void AddRange(this List<ExtenderHandler> handlers, PeerClientConfiguration configuration, PeerClientExtensionContext context)
        {
            configuration.Extensions.Register(handlers, context);
        }
    }
}