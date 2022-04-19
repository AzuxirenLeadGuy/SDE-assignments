using System;
using System.Linq;
namespace SDE_Project
{
    public struct HashStruct
    {
        public readonly byte A;
        public readonly byte B;
        public int Hash(int x) => x * A + B;
        public HashStruct(byte a, byte b) => (A, B) = (a, b);
        public static HashStruct[] GenerateRandomHashes(int count, byte start = 0)
        {
            Random random = new();
            byte[] av = Enumerable.Range(start, 2 * count).OrderBy(x => random.Next()).Select(x => (byte)x).ToArray();
            return Enumerable.Range(0, count).Select(x => new HashStruct(av[x], av[count + x])).ToArray();
        }
    }
}