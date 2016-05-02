namespace Leak.Core.Encoding
{
    public static class Bencoder
    {
        public static BencodedValue Decode(byte[] data)
        {
            return new BencoderDecoder(data).Decode();
        }
    }
}