namespace Leak.Core.Network
{
    public class NetworkConnectionEncryptorToNothing : NetworkConnectionEncryptor
    {
        public override byte[] Encrypt(byte[] data)
        {
            return data;
        }
    }
}