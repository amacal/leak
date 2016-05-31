namespace Leak.Core.Net
{
    public interface PeerNegotiatorEncryptedCallback
    {
        void OnData(byte[] data);
    }
}