namespace Leak.Core.Network
{
    public class NetworkConnectionDecryptorToNothing : NetworkConnectionDecryptor
    {
        public override void Decrypt(byte[] data, int index, int count)
        {
        }
    }
}