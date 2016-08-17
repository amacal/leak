using Leak.Core.Common;
using Leak.Core.Metafile;

namespace Leak.Core.Metaget
{
    public class MetagetMetafile : MetafileCallbackBase
    {
        private readonly MetagetContext context;

        public MetagetMetafile(MetagetContext context)
        {
            this.context = context;
        }

        public override void OnCompleted(FileHash hash)
        {
            context.Callback.OnMetadataCompleted(hash);
        }
    }
}