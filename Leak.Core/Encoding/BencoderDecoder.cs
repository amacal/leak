namespace Leak.Core.Encoding
{
    internal class BencoderDecoder
    {
        private byte[] data;
        private int position;

        public BencoderDecoder(byte[] data)
        {
            this.data = data;
        }

        public BencodedValue Decode()
        {
            if (data[position] == 'd')
            {
                return DecodeDictionary(new BencodedDictionary());
            }

            if (data[position] == 'l')
            {
                return DecodeArray(new BencodedArray());
            }

            if (data[position] == 'i')
            {
                return DecodeNumber();
            }

            return DecodeText();
        }

        private BencodedDictionary DecodeDictionary(BencodedDictionary result)
        {
            int start = position++;

            while (data[position] != 'e')
            {
                result.Add(DecodeText(), Decode());
            }

            result.SetSource(data, start, position);
            position++;

            return result;
        }

        private BencodedArray DecodeArray(BencodedArray result)
        {
            position++;

            while (data[position] != 'e')
            {
                result.Add(Decode());
            }

            position++;

            return result;
        }

        private BencodedNumber DecodeNumber()
        {
            position++;

            int value = 0;
            bool minus = false;

            if (data[position] == '-')
            {
                minus = true;
                position = position + 1;
            }

            while (data[position] >= '0' && data[position] <= '9')
            {
                value = 10 * value + data[position] - '0';
                position = position + 1;
            }

            position++;

            return new BencodedNumber();
        }

        private BencodedText DecodeText()
        {
            int length = 0;
            string value = "";

            while (data[position] >= '0' && data[position] <= '9')
            {
                length = 10 * length + data[position] - '0';
                position = position + 1;
            }

            int start = position + 1;

            if (length > 0)
            {
                value = System.Text.Encoding.ASCII.GetString(data, position + 1, length);
                position = position + length;
            }

            BencodedText result = new BencodedText(value);
            result.SetSource(data, start, position);
            position++;

            return result;
        }
    }
}