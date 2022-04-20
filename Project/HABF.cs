using System;
using System.Linq;
namespace SDE_Project
{
    public class HABF<ItemType>
    {
        public readonly HashStruct[] GlobalHashes;
        private readonly HashExpressor<ItemType> expressor;
        private readonly StandardBloomFilter<ItemType> filter;
        public HABF(int bits, HashStruct[] hashes, int extraHashes, int omega)
        {
            GlobalHashes = HashStruct.GenerateRandomHashes(extraHashes);
            filter = new(bits, hashes);
            expressor = new HashExpressor<ItemType>(omega, GlobalHashes, filter, hashes.Length);
        }
        public HABF(int bits, byte defaultHashes, byte globalHashes)
        {
            var totalhashes = HashStruct.GenerateRandomHashes(globalHashes + defaultHashes);
            GlobalHashes = totalhashes.Take(globalHashes).ToArray();
            var remaininghashes = totalhashes.TakeLast(defaultHashes);
            filter = new(bits, remaininghashes.ToArray());
            expressor = new(1 + (2 * globalHashes), GlobalHashes, filter, defaultHashes);
        }
        public bool Check(ItemType item)
        {
            if (item == null) throw new ArgumentException("Item cannot be null", nameof(item));
            bool check = filter.Check(item);
            if (check) return true; // No need to check hash-expressor
            int[]? posits = expressor.Query(item);
            if (posits == null) return false; // this point was never queried as a false positive key
            else return filter.CheckPositions(posits);
        }
        public bool Add(ItemType item)
        {
            if (item == null) throw new ArgumentException("Cannot accept a null object", nameof(item));
            bool alreadyPresent = filter.Check(item);
            if (alreadyPresent == false)
                filter.Insert(item);
            return alreadyPresent; // true if the positive was already being returned (This is here just for debugging)
        }
        public bool OptimizeFalsePositiveKey(ItemType item)
        {
            if (item == null) throw new ArgumentException("Cannot accept a null object", nameof(item));
            return expressor.Optimize(item);
        }
    }
}