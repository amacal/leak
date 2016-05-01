using System;
using System.Collections.Generic;
using System.Linq;

namespace Leak.Core.Encoding
{
    public static class BencoderExtensions
    {
        public static T Find<T>(this BencodedValue value, string name, Func<BencodedValue, T> selector)
        {
            return Find(value as BencodedDictionary, name, selector);
        }

        private static T Find<T>(this BencodedDictionary value, string name, Func<BencodedValue, T> selector)
        {
            if (value != null)
            {
                return selector.Invoke(value.Find(name));
            }

            return selector.Invoke(null);
        }

        public static string ToText(this BencodedValue value)
        {
            return AllTexts(value).FirstOrDefault();
        }

        public static long ToNumber(this BencodedValue value)
        {
            return ((BencodedNumber)value).Value;
        }

        public static BencodedValue[] AllItems(this BencodedValue value)
        {
            return AllItems(new List<BencodedValue>(), value as BencodedArray).ToArray();
        }

        private static List<BencodedValue> AllItems(List<BencodedValue> result, BencodedArray array)
        {
            if (array != null)
            {
                foreach (BencodedValue value in array.Items)
                {
                    result.Add(value);
                }
            }

            return result;
        }

        public static string[] AllKeys(this BencodedValue value)
        {
            return AllKeys(new List<string>(), value).ToArray();
        }

        private static List<string> AllKeys(List<string> output, BencodedValue value)
        {
            AllKeys(output, value as BencodedDictionary);
            AllKeys(output, value as BencodedArray);

            return output;
        }

        private static void AllKeys(List<string> output, BencodedArray array)
        {
            if (array != null)
            {
                foreach (BencodedValue value in array.Items)
                {
                    AllKeys(output, value);
                }
            }
        }

        private static void AllKeys(List<string> output, BencodedDictionary dictionary)
        {
            if (dictionary != null)
            {
                foreach (BencodedText text in dictionary.Keys)
                {
                    output.Add(text.Value);
                }

                foreach (BencodedValue value in dictionary.Values)
                {
                    AllKeys(output, value);
                }
            }
        }

        public static string[] AllTexts(this BencodedValue value)
        {
            return AllTexts(new List<string>(), value).ToArray();
        }

        private static List<string> AllTexts(List<string> output, BencodedValue value)
        {
            AllTexts(output, value as BencodedText);
            AllTexts(output, value as BencodedDictionary);
            AllTexts(output, value as BencodedArray);

            return output;
        }

        private static void AllTexts(List<string> output, BencodedText text)
        {
            if (text != null)
            {
                output.Add(text.Value);
            }
        }

        private static void AllTexts(List<string> output, BencodedArray array)
        {
            if (array != null)
            {
                foreach (BencodedValue value in array.Items)
                {
                    AllTexts(output, value);
                }
            }
        }

        private static void AllTexts(List<string> output, BencodedDictionary dictionary)
        {
            if (dictionary != null)
            {
                foreach (BencodedValue value in dictionary.Values)
                {
                    AllTexts(output, value);
                }
            }
        }
    }
}