using System;

namespace Leak.Networking
{
    public class NetworkPoolInline : NetworkPoolTask
    {
        private readonly Action callback;

        public NetworkPoolInline(Action callback)
        {
            this.callback = callback;
        }

        public bool CanExecute(NetworkPoolQueue queue)
        {
            return true;
        }

        public void Execute(NetworkPoolInstance context, NetworkPoolTaskCallback callback)
        {
            this.callback.Invoke();
        }

        public void Block(NetworkPoolQueue queue)
        {
        }

        public void Release(NetworkPoolQueue queue)
        {
        }
    }
}