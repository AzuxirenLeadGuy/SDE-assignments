using System;
using System.Collections.Generic;
using System.Linq;
namespace SDE_Project
{
    /// <summary>
    /// The hashexpressor object optimizes the hash functions for the small group of false positve keys for recheking the membership obtained from the StandardBloomFilter
    /// </summary>
    /// <typeparam name="ItemType">The type of item that the filter maintains a membership for</typeparam>
    public class HashExpressor<ItemType>
    {
        /// <summary>
        /// The list of cells being used by the structure
        /// </summary>
        protected readonly int[,] Cells;
        /// <summary>
        /// The collection of global hash functions
        /// </summary>
        protected readonly HashStruct[] GlobalHashes;
        /// <summary>
        /// The number of cells in this expressor 
        /// </summary>
        public readonly int Omega;
        /// <summary>
        /// The number of global hash functions
        /// </summary>
        public readonly int HashLength;
        /// <summary>
        /// The number of hash functions per point
        /// </summary>
        public readonly int K;
        /// <summary>
        /// The bloom filter associated with this structure
        /// </summary>
        public readonly StandardBloomFilter<ItemType> filter;
        /// <summary>
        /// The index of the default hash to use
        /// </summary>
        private int DefaultHash;
        /// <summary>
        /// Constructs a HashExpressor object
        /// </summary>
        /// <param name="omg">The number of cells</param>
        /// <param name="h">The array of global hash functions</param>
        /// <param name="parent">The bloom filter to depend on the results for</param>
        /// <param name="k">The number of hash functions for each point</param>
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
        /// <summary>
        /// Preforms the positive modulus operation
        /// </summary>
        /// <param name="a">The element for which the mod should be done</param>
        /// <returns>The positve mod a mod Omega</returns>
        internal int Mod(int a) => (a %= Omega) < 0 ? a + Omega : a;
        /// <summary>
        /// Optimizes the structure for the given positive and negative set of keys 
        /// </summary>
        /// <param name="tp">Postitve key set</param>
        /// <param name="fp">Negative key set</param>
        /// <returns>The number of points optiimized</returns>
        public int Optimize(ItemType[] tp, ItemType[] fp)
        {
            filter.Clear();
            int[] NCFK = Array.Empty<int>();
            int i, j, best = 0, leastclash = tp.Length, clash, succ = 0;
            {
                List<int> tpKeys = new(), fpKeys = new(), clashKeys = new();
                for (i = tp.Length - 1; i >= 0; i--)
                {
                    var item = tp[i];
                    if (item == null) throw new ArgumentException("Null object found");
                    filter.Insert(item);
                    tpKeys.Add(item.GetHashCode());
                }
                fp = fp.Where(x => filter.Check(x)).ToArray();
                Console.WriteLine($"Found {fp.Length} false positive keys!");
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
        /// <summary>
        /// Check if this point has any associated specialized hash functions
        /// </summary>
        /// <param name="item">The item key to check</param>
        /// <returns>Returns K hash function results as an array</returns>
        public int[]? Query(ItemType item)
        {
            if (item == null) throw new ArgumentException("Cannot process null object");
            int key_e = item.GetHashCode();
            return Query(key_e);
        }
        /// <summary>
        /// Check if this point has any associated specialized hash functions
        /// </summary>
        /// <param name="key_e">The key to check</param>
        /// <returns>Returns K hash function results as an array</returns>
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
        /// <summary>
        /// Removes this point as an optimized point
        /// </summary>
        /// <param name="item"></param>
        public bool RemoveOptimizedPoint(ItemType item)
        {
            if (item == null) throw new ArgumentException("Cannot process null object");
            int key_e = item.GetHashCode();
            if (Cells[key_e, 0] == -1) return false;
            for (int i = 0; i < K; i++) Cells[key_e, i] = -1;
            return true;
        }
    }
}