using System;
using System.Collections;

namespace SDE_Project
{
    public class StandardBloomFilter<ItemType>
    {
        protected readonly BitArray _bits;
        public readonly byte K;
        public readonly int M;
        public readonly HashStruct[] DefaultHashes;
        public StandardBloomFilter(int bits, HashStruct[] hashes)
        {
            K = (byte)hashes.Length;
            if (K < 2) throw new ArgumentException($"Number of hash functions must be at least 2, recived an array whose (byte) casted length is {K}", nameof(hashes));
            if (bits < K) throw new ArgumentException($"Number of bits must be positive and greater than K(number of hashs). Recived K={K} and bits={bits}", nameof(bits));
            M = bits;
            DefaultHashes = hashes;
            _bits = new BitArray(length: bits, defaultValue: false);
        }
        public StandardBloomFilter(int bits, byte k, byte Hv = 0)
        {
            K = k;
            if (K < 2) throw new ArgumentException($"Number of hash functions must be at least 2, recived an array whose (byte) casted length is {K}", nameof(k));
            if (bits < K) throw new ArgumentException($"Number of bits must be positive and greater than K(number of hashs). Recived K={K} and bits={bits}", nameof(bits));
            M = bits;
            DefaultHashes = HashStruct.GenerateRandomHashes(K, Hv);
            _bits = new(bits);
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
        public bool Check(ItemType item)
        {
            int[] positions = GetHashes(item);
            if (positions.Length != K) throw new Exception($"The Hashes() method should return {K} hashes, but obtained {positions.Length} hashes!");
            return CheckPositions(positions);
        }
        internal bool CheckPositions(int[] positions)
        {
            for (int i = 0; i < K; i++)
            {
                if (GetFilterIndex(positions[i]) == false) return false;
            }
            return true;
        }
        public void Insert(ItemType item)
        {
            var positions = GetHashes(item);
            if (positions.Length != K) throw new Exception($"The Hashes() method should return {K} hashes, but obtained {positions.Length} hashes!");
            for (int i = 0; i < K; i++) SetFilterIndex(positions[i], true);
        }
        public int Mod(int a) => (a %= M) < 0 ? a + M : a;
        public bool GetFilterIndex(int hash) => _bits.Get(Mod(hash));
        public void SetFilterIndex(int hash, bool value) => _bits.Set(Mod(hash), value);
    }
}