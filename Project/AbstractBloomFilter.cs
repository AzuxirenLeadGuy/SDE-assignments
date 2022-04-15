using System;
using System.Collections;

namespace SDE_Project
{
    public class StandardBloomFilter<ItemType>
    {
        protected readonly BitArray _bits;
        protected readonly byte K;
        protected readonly int X;
        protected readonly (byte A, byte B)[] DefaultHashes;
        public StandardBloomFilter(int bits, (byte A, byte B)[] hashes)
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
            int[] hs = new int[K];
            var hf = DefaultHashes;
            var hv = item.GetHashCode();
            for (int i = 0; i < K; i++)
            {
                hs[i] = hf[i].A * hv + hf[i].B;
            }
            return hs;
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