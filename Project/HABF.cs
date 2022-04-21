using System;
using System.Linq;
namespace SDE_Project
{
    /// <summary>
    /// The custom implementation of Hash Adaptive Bloom Filter
    /// </summary>
    /// <typeparam name="ItemType"></typeparam>
    public class HABF<ItemType>
    {
        /// <summary>
        /// The collection of randomly initialized Hash functions
        /// </summary>
        public readonly HashStruct[] GlobalHashes;
        /// <summary>
        /// The inner Hash-Expressor
        /// </summary>
        private readonly HashExpressor<ItemType> expressor;
        /// <summary>
        /// The inner bloom filter
        /// </summary>
        private readonly StandardBloomFilter<ItemType> filter;
        /// <summary>
        /// The Hash functions for the Bloom filter
        /// </summary>
        public HashStruct[] DefaultBloomFilterHashes => filter.DefaultHashes;
        /// <summary>
        /// Constructs a HABF object
        /// </summary>
        /// <param name="bits">The number of bits in the inner standard bloom filter</param>
        /// <param name="hashes">The hash functions to use for the inner bloom filter</param>
        /// <param name="extraHashes">The number of global hash functions to generate randomly</param>
        /// <param name="omega">The number of cells for the inner `HashExpressor`</param>
        public HABF(int bits, HashStruct[] hashes, int extraHashes, int omega)
        {
            GlobalHashes = HashStruct.GenerateRandomHashes(extraHashes);
            filter = new(bits, hashes);
            expressor = new HashExpressor<ItemType>(omega, GlobalHashes, filter, hashes.Length);
        }
        /// <summary>
        /// Constructs a HABF object
        /// </summary>
        /// <param name="bits">The number of bits in the inner standard bloom filter</param>
        /// <param name="ksize">The number of hash functions to use for the inner bloom filter</param>
        /// <param name="extraHashes">The number of global hash functions to generate randomly</param>
        /// <param name="omega">The number of cells for the inner `HashExpressor`</param>
        public HABF(int bits, int ksize, int extraHashes, int omega)
        {
            GlobalHashes = HashStruct.GenerateRandomHashes(extraHashes + ksize);
            filter = new(bits, GlobalHashes.Take(ksize).ToArray());
            expressor = new HashExpressor<ItemType>(omega, GlobalHashes, filter, ksize);
        }
        /// <summary>
        /// Checks for the membership of an item
        /// </summary>
        /// <param name="item">The item to check</param>
        /// <returns>If the item is not a member, returns false. Returns true if the item may or may not be a member</returns>
        public bool Check(ItemType item)
        {
            if (item == null) throw new ArgumentException("Item cannot be null", nameof(item));
            int[]? posits = expressor.Query(item);
            bool check = filter.Check(item);
            if (posits != null) check = filter.CheckPositions(posits);
            return check;
        }
        /// <summary>
        /// Query the item to be added (Does not optimize the point)
        /// </summary>
        /// <param name="item"></param>
        public void Insert(ItemType item)
        {
            filter.Insert(item);
            if (expressor.Query(item) != null)
            {
                expressor.RemoveOptimizedPoint(item);
            }
        }
        /// <summary>
        /// Optimizes the filter for the given points. Note clears the bloom filter before optimization
        /// </summary>
        /// <param name="TruePositives">The set of positive keys</param>
        /// <param name="FalsePositives">The set of negative keys</param>
        /// <returns></returns>
        public int OptimizeAndInsert(ItemType[] TruePositives, ItemType[] FalsePositives)
        {
            return expressor.Optimize(TruePositives, FalsePositives);
        }
    }
}