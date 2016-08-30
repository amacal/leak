using Leak.Core.Common;
using Leak.Core.Omnibus;

namespace Leak.Core.Retriever.Callbacks
{
    public class RetrieverToOmnibus : OmnibusCallbackBase
    {
        private readonly RetrieverContext context;

        public RetrieverToOmnibus(RetrieverContext context)
        {
            this.context = context;
        }

        public override void OnChanged(FileHash hash, BitfieldInfo bitfield)
        {
            context.Callback.OnProgress(hash, bitfield);
        }
    }
}