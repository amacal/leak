namespace Leak.Core.Network
{
    public class NetworkBufferDecryptorToNothing : NetworkBufferDecryptor
    {
        public override byte[] Decrypt(byte[] data)
        {
            return data;
        }
    }
}