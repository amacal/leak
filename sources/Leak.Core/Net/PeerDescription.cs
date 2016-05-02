namespace Leak.Core.Net
{
    public abstract class PeerDescription
    {
        public abstract string Host { get; }

        public override string ToString()
        {
            return Host.PadLeft(15);
        }
    }
}