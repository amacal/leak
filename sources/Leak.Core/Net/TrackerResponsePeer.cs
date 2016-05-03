namespace Leak.Core.Net
{
    public abstract class TrackerResponsePeer
    {
        public abstract string Host { get; }

        public abstract int Port { get; }

        public override string ToString()
        {
            return $"{Host}:{Port}";
        }
    }
}