namespace Leak.Networking
{
    public class NetworkDecryptorToNothing : NetworkDecryptor
    {
        public override void Decrypt(byte[] data, int index, int count)
        {
        }
    }
}