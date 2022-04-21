using System;
using System.Linq;
namespace SDE_Project
{
    /// <summary>
    /// A structure that stores a simplistic hash function in the form of a linear function Ax+B
    /// </summary>
    public struct HashStruct
    {
        /// <summary>
        /// The coefficient A for the hash function
        /// </summary>
        public readonly byte A;
        /// <summary>
        /// The coefficient B for the hash function
        /// </summary>
        public readonly byte B;
        /// <summary>
        /// The implemented Hash function as a linear function of A, B and input element x
        /// </summary>
        /// <param name="x"></param>
        /// <returns>The hash for a given element/key</returns>
        public int Hash(int x) => x * A + B;
        /// <summary>
        /// Constructor for a `HashStruct` object
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public HashStruct(byte a, byte b) => (A, B) = (a, b);
        /// <summary>
        /// Generates a random list of HashStruct. All coefficients are forced to be unique.
        /// </summary>
        /// <param name="count">The number of hash functions to generate</param>
        /// <param name="start">The minimum coefficent value (if any)</param>
        /// <returns></returns>
        public static HashStruct[] GenerateRandomHashes(int count, byte start = 1)
        {
            Random random = new();
            byte[] av = Enumerable.Range(start, 2 * count).OrderBy(x => random.Next()).Select(x => (byte)x).ToArray();
            return Enumerable.Range(0, count).Select(x => new HashStruct(av[x], av[count + x])).ToArray();
        }
    }
}