namespace Leak.Bencoding
{
    public static class Bencoder
    {
        public static BencodedValue Decode(byte[] data)
        {
            return new BencoderDecoder().Decode(data);
        }

        public static BencodedValue Decode(byte[] data, int offset)
        {
            return new BencoderDecoder().Decode(data, offset);
        }

        public static byte[] Encode(BencodedValue value)
        {
            return new BencoderDecoder().Encode(value);
        }
    }
}