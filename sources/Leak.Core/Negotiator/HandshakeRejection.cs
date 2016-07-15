namespace Leak.Core.Negotiator
{
    public class HandshakeRejection
    {
        private readonly HandshakeHashMatch match;

        public HandshakeRejection(HandshakeHashMatch match)
        {
            this.match = match;
        }

        public HandshakeHashMatch Match
        {
            get { return match; }
        }
    }
}