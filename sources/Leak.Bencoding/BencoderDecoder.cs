using System.Collections.Generic;
using System.IO;

namespace Leak.Bencoding
{
    public class BencoderDecoder
    {
        public byte[] Encode(BencodedValue data)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                this.Encode(stream, data);
                return stream.ToArray();
            }
        }

        private void Encode(MemoryStream stream, BencodedValue data)
        {
            if (data.Dictionary != null)
            {
                this.Write(stream, "d");

                foreach (BencodedEntry entry in data.Dictionary)
                {
                    this.Encode(stream, entry.Key);
                    this.Encode(stream, entry.Value);
                }

                this.Write(stream, "e");
            }
            else if (data.Array != null)
            {
                this.Write(stream, "l");

                foreach (BencodedValue item in data.Array)
                {
                    this.Encode(stream, item);
                }

                this.Write(stream, "e");
            }
            else if (data.Text != null)
            {
                this.Write(stream, data.Text.Length.ToString());
                this.Write(stream, ":");
                this.Write(stream, data.Text.GetBytes());
            }
            else if (data.Number != null)
            {
                this.Write(stream, "i");
                this.Write(stream, data.Number.ToString());
                this.Write(stream, "e");
            }
            else
            {
                this.Write(stream, data.Data.Length.ToString());
                this.Write(stream, ":");
                this.Write(stream, data.Data.GetBytes());
            }
        }

        private void Write(Stream stream, string data)
        {
            byte[] bytes = new byte[data.Length];

            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = (byte)data[i];
            }

            stream.Write(bytes, 0, bytes.Length);
        }

        private void Write(Stream stream, byte[] data)
        {
            stream.Write(data, 0, data.Length);
        }

        public BencodedValue Decode(byte[] data)
        {
            int position = 0;
            return this.DecodeCore(data, ref position);
        }

        public BencodedValue Decode(byte[] data, int offset)
        {
            int position = offset;
            return this.DecodeCore(data, ref position);
        }

        private BencodedValue DecodeCore(byte[] data, ref int position)
        {
            switch (data[position])
            {
                case 0x69:
                    position++;
                    return this.DecodeInteger(data, ref position);

                case 0x64:
                    position++;
                    return this.DecodeDictionary(data, ref position);

                case 0x6c:
                    position++;
                    return this.DecodeArray(data, ref position);

                case 0x30:
                case 0x31:
                case 0x32:
                case 0x33:
                case 0x34:
                case 0x35:
                case 0x36:
                case 0x37:
                case 0x38:
                case 0x39:
                    return this.DecodeString(data, ref position);
            }

            return new BencodedValue();
        }

        private BencodedValue DecodeString(byte[] data, ref int position)
        {
            int offset = 0;
            int length = 0;

            while (data[position] != 0x3a)
            {
                length = length * 10 + (data[position++] - 0x30);
            }

            offset = position + 1;
            position = position + length + 1;

            return new BencodedValue
            {
                Data = new BencodedData(data, offset, length),
                Text = new BencodedText(new BencodedData(data, offset, length))
            };
        }

        private BencodedValue DecodeInteger(byte[] data, ref int position)
        {
            int offset = position;
            long number = 0L;
            long multiply = 1L;

            if (data[position] == 0x2d)
            {
                multiply = -1L;
                position++;
            }

            while (data[position] != 0x65)
            {
                number = number * 10 + (data[position++] - 0x30);
            }

            position++;
            return new BencodedValue
            {
                Data = new BencodedData(data, offset, position - offset),
                Number = new BencodedNumber(number * multiply)
            };
        }

        private BencodedValue DecodeArray(byte[] data, ref int position)
        {
            int offset = position;
            List<BencodedValue> array = new List<BencodedValue>();

            while (data[position] != 0x65)
            {
                array.Add(this.DecodeCore(data, ref position));
            }

            position++;
            return new BencodedValue
            {
                Data = new BencodedData(data, offset, position - offset),
                Array = array.ToArray()
            };
        }

        private BencodedValue DecodeDictionary(byte[] data, ref int position)
        {
            int offset = position - 1;
            List<BencodedEntry> entries = new List<BencodedEntry>();

            while (data[position] != 0x65)
            {
                BencodedValue key = DecodeCore(data, ref position);
                BencodedValue value = DecodeCore(data, ref position);

                entries.Add(new BencodedEntry { Key = key, Value = value });
            }

            position++;
            return new BencodedValue
            {
                Data = new BencodedData(data, offset, position - offset),
                Dictionary = entries.ToArray()
            };
        }
    }
}