using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypto.HelperClass
{
    internal class Converter
    {
        public static byte[] HexToBytes(string hex)
        {
            return Enumerable.Range(0, hex.Length / 2)
                .Select(x => Convert.ToByte(hex.Substring(x * 2, 2), 16))
                .ToArray();
        }

        public static string ToHexString(byte[] ba) => BitConverter.ToString(ba).Replace("-", "").ToLowerInvariant();
    }
}
