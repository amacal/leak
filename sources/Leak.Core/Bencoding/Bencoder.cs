namespace Leak.Core.Bencoding
{
    public static class Bencoder
    {
        public static BencodedValue Decode(byte[] data)
        {
            return new BencoderDecoder().Decode(data);
        }

        public static byte[] Encode(BencodedValue value)
        {
            return new BencoderDecoder().Encode(value);
        }
    }
}