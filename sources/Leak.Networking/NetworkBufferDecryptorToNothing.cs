namespace Leak.Networking
{
    public class NetworkBufferDecryptorToNothing : NetworkBufferDecryptor
    {
        public override void Decrypt(byte[] data, int index, int count)
        {
        }
    }
}