using Leak.Core.Metamine;

namespace Leak.Core.Metaget
{
    public static class MetagetExtensions
    {
        public static void Complete(this MetamineBitfield bitfield, int block, int size)
        {
            bitfield.Complete(new MetamineBlock(block, size));
        }
    }
}