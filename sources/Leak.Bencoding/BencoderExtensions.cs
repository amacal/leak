using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Leak.Bencoding
{
    public static class BencoderExtensions
    {
        public static T Find<T>(this BencodedValue value, string name, Func<BencodedValue, T> selector)
        {
            if (value != null && value.Dictionary != null)
            {
                foreach (BencodedEntry entry in value.Dictionary)
                {
                    if (entry.Key.Text != null && entry.Key.Text.GetString() == name)
                    {
                        return selector.Invoke(entry.Value);
                    }
                }
            }

            return selector.Invoke(null);
        }

        public static string ToText(this BencodedValue value)
        {
            return ToText(value, Encoding.ASCII);
        }

        public static string ToText(this BencodedValue value, Encoding encoding)
        {
            return AllTexts(value, encoding).FirstOrDefault();
        }

        public static int ToInt32(this BencodedValue value)
        {
            return value.Number.ToInt32();
        }

        public static long ToInt64(this BencodedValue value)
        {
            return value.Number.ToInt64();
        }

        private static List<string> AllKeys(List<string> output, BencodedValue value)
        {
            AllKeys(output, value.Dictionary);
            AllKeys(output, value.Array);

            return output;
        }

        private static void AllKeys(List<string> output, BencodedValue[] array)
        {
            if (array != null)
            {
                foreach (BencodedValue value in array)
                {
                    AllKeys(output, value);
                }
            }
        }

        private static void AllKeys(List<string> output, BencodedEntry[] dictionary)
        {
            if (dictionary != null)
            {
                foreach (BencodedEntry entry in dictionary)
                {
                    output.Add(entry.Key.ToString());
                }

                foreach (BencodedEntry entry in dictionary)
                {
                    AllKeys(output, entry.Value);
                }
            }
        }

        public static string[] AllTexts(this BencodedValue value)
        {
            return AllTexts(new List<string>(), value, Encoding.ASCII).ToArray();
        }

        public static string[] AllTexts(this BencodedValue value, Encoding encoding)
        {
            return AllTexts(new List<string>(), value, encoding).ToArray();
        }

        private static List<string> AllTexts(List<string> output, BencodedValue value, Encoding encoding)
        {
            if (value != null)
            {
                AllTexts(output, value.Text, encoding);
                AllTexts(output, value.Dictionary, encoding);
                AllTexts(output, value.Array, encoding);
            }

            return output;
        }

        private static void AllTexts(List<string> output, BencodedText text, Encoding encoding)
        {
            if (text != null)
            {
                output.Add(text.GetString(encoding));
            }
        }

        private static void AllTexts(List<string> output, BencodedValue[] array, Encoding encoding)
        {
            if (array != null)
            {
                foreach (BencodedValue value in array)
                {
                    AllTexts(output, value, encoding);
                }
            }
        }

        private static void AllTexts(List<string> output, BencodedEntry[] dictionary, Encoding encoding)
        {
            if (dictionary != null)
            {
                foreach (BencodedEntry entry in dictionary)
                {
                    AllTexts(output, entry.Value, encoding);
                }
            }
        }
    }
}