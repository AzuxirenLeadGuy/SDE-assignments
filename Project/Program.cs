using System;
using System.Linq;
namespace SDE_Project
{
    static class Program
    {
        public static void Main()
        {
            GenericTest(32, 2, 10);
            GenericTest(64, 3, 20);
            GenericTest(1024, 5, 60);
            GenericTest(8192, 6, 120);
            GenericTest(65536, 6, 255);
        }
        public struct BloomFilterResult
        {
            public uint TruePositive, FalsePositive, TrueNegative, FalseNegative;
        }
        public static (BloomFilterResult standard, BloomFilterResult habf) GenericTest(int bitsize, byte ksize, byte extrahashsize)
        {
            Console.WriteLine("Showcasing the Bloom filter and the Hash adaptive bloom filter");
            HABF<int> hashbloomfilter = new(bitsize, ksize, extrahashsize, extrahashsize << 3);
            Random randomGen = new();
            StandardBloomFilter<int> normalfilter = new(bitsize, hashbloomfilter.DefaultBloomFilterHashes);
            int insertionsize = (int)(bitsize * 0.69f / ksize) * 2;
            int[] allitems = Enumerable.Range(0, insertionsize + 5).OrderBy(x => randomGen.Next()).Take(insertionsize).ToArray();
            double sum = insertionsize;
            insertionsize /= 2;
            int[] poskeys = allitems.Take(insertionsize).ToArray();
            int[] negkeys = allitems.Skip(insertionsize).ToArray();
            BloomFilterResult a = new(), b = new();
            int i;
            for (i = 0; i < poskeys.Length; i++)
            {
                normalfilter.Insert(poskeys[i]);
            }
            hashbloomfilter.OptimizeAndInsert(poskeys, negkeys);
            for (i = 0; i < poskeys.Length; i++)
            {
                int item = poskeys[i];
                if (normalfilter.Check(item))
                    a.TruePositive++;
                else
                    a.FalseNegative++;
                if (hashbloomfilter.Check(item))
                    b.TruePositive++;
                else
                    b.FalseNegative++;
            }
            for (i = 0; i < negkeys.Length; i++)
            {
                int item = negkeys[i];
                if (normalfilter.Check(item))
                    a.FalsePositive++;
                else
                    a.TrueNegative++;
                if (hashbloomfilter.Check(item))
                    b.FalsePositive++;
                else
                    b.TrueNegative++;
            }
            Console.WriteLine(@"================= Standard Bloom Filter  | Hash Adaptive Bloom Filter ");
            printhelper("True Positive  = ", a.TruePositive, b.TruePositive);
            printhelper("False Negative = ", a.FalseNegative, b.FalseNegative);
            printhelper("True Negative  = ", a.TrueNegative, b.TrueNegative);
            printhelper("False Positive = ", a.FalsePositive, b.FalsePositive);
            Console.WriteLine("\n\n");
            void printhelper(string title, uint a, uint b)
            {
                Console.WriteLine($"{$"{title} {a * 100 / sum:00.000} ",40} | {b * 100 / sum:00.000} %");
            }
            return (a, b);
        }
    }
}