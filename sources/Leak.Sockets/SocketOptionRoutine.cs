using System;

namespace Leak.Sockets
{
    internal class SocketOptionRoutine
    {
        private readonly int level;
        private readonly int option;
        private readonly int value;

        public SocketOptionRoutine(int level, int option, int value)
        {
            this.level = level;
            this.option = option;
            this.value = value;
        }

        public void Execute(IntPtr handle)
        {
            int reference = value;
            uint result = UdpSocketInterop.setsockopt(handle, level, option, ref reference, sizeof(int));

            if (result != 0)
            {
            }
        }
    }
}