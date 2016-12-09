namespace Leak.Networking
{
    public class NetworkEncryptorToNothing : NetworkEncryptor
    {
        public override byte[] Encrypt(byte[] data)
        {
            return data;
        }
    }
}