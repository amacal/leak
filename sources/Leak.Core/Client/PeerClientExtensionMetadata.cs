using Leak.Core.Extensions;
using Leak.Core.Extensions.Metadata;
using System.Collections.Generic;

namespace Leak.Core.Client
{
    public class PeerClientExtensionMetadata : PeerClientExtension
    {
        public void Register(ICollection<ExtenderHandler> handlers, PeerClientExtensionContext context)
        {
            MetadataHandler handler = new MetadataHandler(with =>
            {
                with.Callback = new PeerClientToMetadata(context);
            });

            handlers.Add(handler);
        }

        public void Register(ICollection<string> extensions)
        {
            extensions.Add("ut_metadata");
        }
    }
}