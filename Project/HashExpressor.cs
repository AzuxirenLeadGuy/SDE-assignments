using System;
using System.Collections.Generic;
using System.Linq;
namespace SDE_Project
{
    internal class HashExpressor<ItemType>
    {
        protected readonly int[,] Cells;
        protected readonly HashStruct[] GlobalHashes;
        public readonly int Omega, HashLength, K;
        public readonly StandardBloomFilter<ItemType> filter;
        private int DefaultHash;
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
        public int Optimize(ItemType[] tp, ItemType[] fp)
        {
            fp = fp.Where(x => filter.Check(x)).ToArray();
            int[] NCFK = Array.Empty<int>();
            Console.WriteLine($"Found {fp.Length} false positive keys!");
            int i, j, best = 0, leastclash = tp.Length, clash, succ = 0;
            {
                List<int> tpKeys = new(), fpKeys = new(), clashKeys = new();
                for (i = tp.Length - 1; i >= 0; i--)
                {
                    var item = tp[i];
                    if (item == null) throw new ArgumentException("Null object found");
                    tpKeys.Add(item.GetHashCode());
                }
                for (i = fp.Length - 1; i >= 0; i--)
                {
                    var item = fp[i];
                    if (item == null) throw new ArgumentException("Null object found");
                    fpKeys.Add(item.GetHashCode());
                }
                for (i = 0; i < HashLength; i++)
                {
                    List<int> poskeys = new(), negkeys = new();
                    var Hashfn = GlobalHashes[i];
                    foreach (int item in tpKeys)
                    {
                        poskeys.Add(Mod(Hashfn.Hash(item.GetHashCode())));
                    }
                    foreach (int item in fpKeys)
                    {
                        negkeys.Add(Mod(Hashfn.Hash(item.GetHashCode())));
                    }
                    poskeys.RemoveAll(x => negkeys.Contains(x) == false);
                    negkeys.RemoveAll(x => poskeys.Contains(x));
                    clash = poskeys.Count;
                    if (leastclash > clash)
                    {
                        leastclash = clash;
                        best = i;
                        NCFK = fpKeys.Where(x => negkeys.Contains(Mod(Hashfn.Hash(x)))).ToArray();
                    }
                }
            }
            Console.WriteLine($"Selecting a default hash causing {leastclash} collisions, and can only optimize {NCFK.Length} points");
            DefaultHash = best;
            for (i = 0; i < Omega; i++)
            {
                for (j = 0; j < K; j++)
                {
                    Cells[i, j] = -1;
                }
            }
            foreach (int key_e in NCFK)
            {
                int cell = Mod(GlobalHashes[DefaultHash].Hash(key_e));
                Random random = new();
                if (Cells[cell, 0] != -1)
                {
                    succ++;
                    continue; // This cell is already occupied
                }
                var neghashes = Enumerable.Range(0, HashLength).OrderBy(x => random.Next()).Where(x => filter.GetFilterIndex(GlobalHashes[x].Hash(key_e)) == false).ToList();
                if (neghashes.Count < K) continue; // Enough keys are not positive
                for (i = 0; i < K; i++)
                {
                    int idx = neghashes[i];
                    Cells[cell, i] = idx;
                }
                int[]? check = Query(key_e);
                if (check == null || filter.CheckPositions(check) != false) throw new Exception("This should not happen!");
            }
            return succ;
        }
        public int[]? Query(ItemType item)
        {
            if (item == null) throw new ArgumentException("Cannot process null object");
            int key_e = item.GetHashCode();
            return Query(key_e);
        }
        public int[]? Query(int key_e)
        {
            int cell = Mod(GlobalHashes[DefaultHash].Hash(key_e));
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