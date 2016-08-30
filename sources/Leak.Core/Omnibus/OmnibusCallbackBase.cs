using Leak.Core.Common;

namespace Leak.Core.Omnibus
{
    public abstract class OmnibusCallbackBase : OmnibusCallback
    {
        public virtual void OnChanged(FileHash hash, BitfieldInfo bitfield)
        {
        }
    }
}