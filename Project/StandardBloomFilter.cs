using System;
using System.Collections;

namespace SDE_Project
{
    /// <summary>
    /// A standard bloom filter
    /// </summary>
    /// <typeparam name="ItemType">The type of objects this filter maintains</typeparam>
    public class StandardBloomFilter<ItemType>
    {
        /// <summary>
        /// The collection of bits maintained by the BloomFilter
        /// </summary>
        protected readonly BitArray _bits;
        /// <summary>
        /// The number of hash functions per element
        /// </summary>
        public readonly byte K;
        /// <summary>
        /// The number of bits maintianed by the bloom filter
        /// </summary>
        public readonly int M;
        /// <summary>
        /// The hash functions used by the bloom filter
        /// </summary>
        public readonly HashStruct[] DefaultHashes;
        /// <summary>
        /// Constructs a new bloom filter
        /// </summary>
        /// <param name="bits">The number of bits to maintain</param>
        /// <param name="hashes">The number of hash functions to generate</param>
        public StandardBloomFilter(int bits, HashStruct[] hashes)
        {
            K = (byte)hashes.Length;
            if (K < 2) throw new ArgumentException($"Number of hash functions must be at least 2, recived an array whose (byte) casted length is {K}", nameof(hashes));
            if (bits < K) throw new ArgumentException($"Number of bits must be positive and greater than K(number of hashs). Recived K={K} and bits={bits}", nameof(bits));
            M = bits;
            DefaultHashes = hashes;
            _bits = new BitArray(length: bits, defaultValue: false);
        }
        /// <summary>
        /// Constructs a new bloom filter
        /// </summary>
        /// <param name="bits">The number of bits to maintain</param>
        /// <param name="k">The number of hash functinons</param>
        /// <param name="Hv">The minimum coefficient value for the hash functions (by default 1)</param>
        public StandardBloomFilter(int bits, byte k, byte Hv = 1)
        {
            K = k;
            if (K < 2) throw new ArgumentException($"Number of hash functions must be at least 2, recived an array whose (byte) casted length is {K}", nameof(k));
            if (bits < K) throw new ArgumentException($"Number of bits must be positive and greater than K(number of hashs). Recived K={K} and bits={bits}", nameof(bits));
            M = bits;
            DefaultHashes = HashStruct.GenerateRandomHashes(K, Hv);
            _bits = new(bits);
        }
        /// <summary>
        /// Get all hash values for this element
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected int[] GetHashes(ItemType item)
        {
            if (item == null) throw new ArgumentException($"Cannot find HashCode of a null object!", nameof(item));
            int[] result = new int[K];
            var funcs = DefaultHashes;
            var orighash = item.GetHashCode();
            for (int i = 0; i < K; i++)
            {
                result[i] = funcs[i].Hash(orighash);
            }
            return result;
        }
        /// <summary>
        /// Check membership of this item in the filter
        /// </summary>
        /// <param name="item">The item to check</param>
        /// <returns>If item was not inserted returns false. true is returned if the element was probably inserted</returns>
        public bool Check(ItemType item)
        {
            int[] positions = GetHashes(item);
            if (positions.Length != K) throw new Exception($"The Hashes() method should return {K} hashes, but obtained {positions.Length} hashes!");
            return CheckPositions(positions);
        }
        /// <summary>
        /// Check in these positions of the bitarray
        /// </summary>
        /// <param name="positions">The indices of the bitarray to check</param>
        /// <returns>returns true if all index positions are 1, otherwise 0</returns>
        internal bool CheckPositions(int[] positions)
        {
            for (int i = 0; i < K; i++)
            {
                if (GetFilterIndex(positions[i]) == false) return false;
            }
            return true;
        }
        /// <summary>
        /// Insert this element in the bloom filter
        /// </summary>
        /// <param name="item">The item to insert</param>
        public void Insert(ItemType item)
        {
            var positions = GetHashes(item);
            if (positions.Length != K) throw new Exception($"The Hashes() method should return {K} hashes, but obtained {positions.Length} hashes!");
            for (int i = 0; i < K; i++) SetFilterIndex(positions[i], true);
        }
        /// <summary>
        /// Removes all elements from the array
        /// </summary>
        public void Clear() => _bits.SetAll(false);
        /// <summary>
        /// Mod function implemnetation that always returns a positive value
        /// </summary>
        /// <param name="a">the element to take mod of</param>
        /// <returns>a mod M</returns>
        internal int Mod(int a) => (a %= M) < 0 ? a + M : a;
        /// <summary>
        /// Get the position of the bitarray at this positiion
        /// </summary>
        /// <param name="hash">The position of the bitarray to check</param>
        /// <returns>true if 1 is present at the index. false otherwise</returns>
        internal bool GetFilterIndex(int hash) => _bits.Get(Mod(hash));
        /// <summary>
        /// Inserts a 1 or 0 in the bitarray
        /// </summary>
        /// <param name="hash">The position to insert at</param>
        /// <param name="value">The value to insert</param>
        internal void SetFilterIndex(int hash, bool value) => _bits.Set(Mod(hash), value);
    }
}