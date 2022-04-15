using System;
using System.Collections;

namespace SDE_Project
{
    public struct HashStruct
    {
        public readonly byte A, B;
        public int Hash(int x) => x * A + B;
        public HashStruct(byte a, byte b) => (A, B) = (a, b);
    }
    public class StandardBloomFilter<ItemType>
    {
        protected readonly BitArray _bits;
        protected readonly byte K;
        protected readonly int X;
        protected readonly HashStruct[] DefaultHashes;
        public StandardBloomFilter(int bits, HashStruct[] hashes)
        {
            K = (byte)hashes.Length;
            if (K < 2) throw new ArgumentException($"Number of hash functions must be at least 2, recived an array whose (byte) casted length is {K}", nameof(hashes));
            if (bits < K) throw new ArgumentException($"Number of bits must be positive and greater than K(number of hashs). Recived K={K} and bits={bits}", nameof(bits));
            X = bits;
            DefaultHashes = hashes;
            _bits = new BitArray(length: bits, defaultValue: false);
        }
        protected virtual int[] GetHashes(ItemType item)
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
        public virtual bool Check(ItemType item)
        {
            var positions = GetHashes(item);
            if (positions.Length != K) throw new Exception($"The Hashes() method should return {K} hashes, but obtained {positions.Length} hashes!");
            for (int i = 0; i < K; i++)
            {
                if (_bits.Get(Mod(positions[i])) == false) return false;
            }
            return true;
        }
        public virtual void Insert(ItemType item)
        {
            var positions = GetHashes(item);
            if (positions.Length != K) throw new Exception($"The Hashes() method should return {K} hashes, but obtained {positions.Length} hashes!");
            for (int i = 0; i < K; i++)
            {
                _bits.Set(Mod(positions[i]), true);
            }
        }
        public int Mod(int a) => (a %= X) < 0 ? a + X : a;
    }
}