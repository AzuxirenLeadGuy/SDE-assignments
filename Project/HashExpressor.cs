using System;
using System.Collections;
using System.Linq;
namespace SDE_Project
{
    internal class HashExpressor<ItemType>
    {
        protected readonly int[,] Cells;
        protected readonly HashStruct[] GlobalHashes;
        public readonly int Omega, HashLength, K;
        public readonly StandardBloomFilter<ItemType> filter;
        public HashExpressor(int omg, HashStruct[] h, StandardBloomFilter<ItemType> parent, int k)
        {
            Omega = omg;
            GlobalHashes = h;
            K = k;
            HashLength = GlobalHashes.Length;
            Cells = new int[Omega, K];
            for (int i = 0; i < Omega; i++)
            {
                for (int j = 0; j < K; j++)
                {
                    Cells[i, j] = -1;
                }
            }
            filter = parent;
        }
        public int Mod(int a) => (a %= Omega) < 0 ? a + Omega : a;
        public bool Optimize(ItemType item)
        {
            if (item == null) throw new ArgumentException("Cannot process null object");
            if (filter.Check(item) == false) return true;
            int key_e = item.GetHashCode();
            int cell = Mod(key_e);
            Random random = new();
            if (Cells[cell, 0] != -1)
                return true; // This cell is already occupied
            var neghashes = Enumerable.Range(0, HashLength).OrderBy(x => random.Next()).Where(x => filter.GetFilterIndex(GlobalHashes[x].Hash(key_e)) == false).ToList();
            if (neghashes.Count < K) return false; // Enough keys are not positive
            for (int i = 0; i < K; i++)
            {
                int idx = neghashes[i];
                Cells[cell, i] = idx;
            }
            return true;
        }
        public int[]? Query(ItemType item)
        {
            if (item == null) throw new ArgumentException("Cannot process null object");
            int key_e = item.GetHashCode();
            int cell = Mod(key_e);
            if (Cells[cell, 0] == -1) return null;
            int[] positions = new int[K];
            for (int i = 0; i < K; i++)
            {
                int idx = Cells[cell, i];
                positions[i] = GlobalHashes[idx].Hash(key_e);
            }
            return positions;
        }
    }
}