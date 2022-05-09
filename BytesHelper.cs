using System;

namespace GomuLibrary
{
    /// <summary>
    /// Classe fournissant des méthodes pour manipuler les bytes.
    /// </summary>
    internal static class BytesHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool CompareBytesArray(byte[] a, byte[] b)
        {
            if (a == null)
                return false;

            if (b == null)
                return false;

            if (a.Length != b.Length)
                return false;

            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="srcOffset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static byte[] CopyRange(byte[] source, int srcOffset, int length)
        {
            byte[] buf = new byte[length];

            if (source.Length < length)
                return null;

            Buffer.BlockCopy(source, srcOffset, buf, 0, buf.Length);

            return buf;
        }
    }
}
