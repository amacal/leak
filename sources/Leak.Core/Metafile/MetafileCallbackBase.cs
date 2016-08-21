using Leak.Core.Common;

namespace Leak.Core.Metafile
{
    public abstract class MetafileCallbackBase : MetafileCallback
    {
        public virtual void OnCompleted(FileHash hash, byte[] data)
        {
        }
    }
}