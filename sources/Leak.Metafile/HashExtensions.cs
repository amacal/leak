using System;
using System.Reflection;
using System.Security.Cryptography;

namespace Leak.Meta.Store
{
    public static class HashExtensions
    {
        private static readonly MethodInfo hashCore;
        private static readonly MethodInfo hashFinal;

        static HashExtensions()
        {
            Type target = typeof(HashAlgorithm);
            BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;

            hashCore = target.GetMethod("HashCore", flags);
            hashFinal = target.GetMethod("HashFinal", flags);
        }

        public static void Push(this HashAlgorithm algorithm, byte[] data, int offset, int count)
        {
            hashCore.Invoke(algorithm, new object[] { data, offset, count });
        }

        public static byte[] Complete(this HashAlgorithm algorithm)
        {
            return (byte[])hashFinal.Invoke(algorithm, new object[] { });
        }
    }
}