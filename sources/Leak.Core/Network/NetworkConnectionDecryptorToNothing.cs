namespace Leak.Core.Network
{
    public class NetworkConnectionDecryptorToNothing : NetworkConnectionDecryptor
    {
        public override byte[] Decrypt(byte[] data)
        {
            return data;
        }
    }
}