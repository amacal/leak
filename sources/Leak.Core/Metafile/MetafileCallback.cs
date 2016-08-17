using Leak.Core.Common;

namespace Leak.Core.Metafile
{
    public interface MetafileCallback
    {
        void OnCompleted(FileHash hash);
    }
}