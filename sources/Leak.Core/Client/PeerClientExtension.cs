using Leak.Core.Extensions;
using System.Collections.Generic;

namespace Leak.Core.Client
{
    public interface PeerClientExtension
    {
        void Register(ICollection<ExtenderHandler> handlers, PeerClientExtensionContext context);

        void Register(ICollection<string> extensions);
    }
}