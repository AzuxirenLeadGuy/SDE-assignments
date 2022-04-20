using System;
using System.Linq;
using System.Collections.Generic;
namespace SDE_Project
{
    public static class Program
    {
        public static void Main()
        {
            GenericTest(32, 2, 10);
            GenericTest(64, 3, 20);
            GenericTest(1024, 5, 40);
            GenericTest(8192, 6, 64);
            GenericTest(65536, 6, 128);
        }
        public struct BloomFilterResult
        {
            public uint TruePositive, FalsePositive, TrueNegative, FalseNegative;
        }
        public static (BloomFilterResult standard, BloomFilterResult habf) GenericTest(int bitsize, byte ksize, byte extrahashsize)
        {
            Console.WriteLine("Showcasing the Bloom filter and the Hash adaptive bloom filter");
            HABF<int> hashbloomfilter = new(bitsize, ksize, extrahashsize);
            Random randomGen = new();
            StandardBloomFilter<int> normalfilter = new(bitsize, ksize, (byte)randomGen.Next(ksize, extrahashsize));
            int insertionsize = (int)(bitsize * 0.69f / ksize) * 2;
            int[] allitems = Enumerable.Range(0, 65536).OrderBy(x => randomGen.Next()).Take(insertionsize).ToArray();
            double sum = insertionsize;
            insertionsize /= 2;
            int[] poskeys = allitems.Take(insertionsize).ToArray();
            int[] negkeys = allitems.Skip(insertionsize).ToArray();
            BloomFilterResult a = new(), b = new();
            int i;
            for (i = 0; i < poskeys.Length; i++)
            {
                normalfilter.Insert(poskeys[i]);
                hashbloomfilter.Add(poskeys[i]);
            }
            int failed = 0;
            for (i = 0; i < negkeys.Length; i++)
            {
                if (hashbloomfilter.OptimizeFalsePositiveKey(negkeys[i]) == false)
                {
                    failed++;
                }
            }
            Console.WriteLine($"Optimization failed for {failed}/{poskeys.Length} items!");
            for (i = 0; i < poskeys.Length; i++)
            {
                if (normalfilter.Check(poskeys[i]))
                    a.TruePositive++;
                else
                    a.FalseNegative++;
                if (hashbloomfilter.Check(poskeys[i]))
                    b.TruePositive++;
                else
                    b.FalseNegative++;
            }
            for (i = 0; i < negkeys.Length; i++)
            {
                if (normalfilter.Check(negkeys[i]))
                    a.FalsePositive++;
                else
                    a.TrueNegative++;
                if (hashbloomfilter.Check(negkeys[i]))
                    b.FalsePositive++;
                else
                    b.TrueNegative++;
            }
            Console.WriteLine(@"=================== Standard Bloom Filter | Hash Adaptive Bloom Filter ");
            Console.WriteLine($"{$"True Positive  = {a.TruePositive}, {a.TruePositive * 100 / sum:00.000} % ",24}|{b.TruePositive}, {b.TruePositive * 100 / sum:00.000} %");
            Console.WriteLine($"{$"False Positive = {a.FalsePositive}, {a.FalsePositive * 100 / sum:00.000} % ",24}|{b.FalsePositive}, {b.FalsePositive * 100 / sum:00.000} %");
            Console.WriteLine($"{$"True Negative  = {a.TrueNegative}, {a.TrueNegative * 100 / sum:00.000} % ",24}|{b.TrueNegative}, {b.TrueNegative * 100 / sum:00.000} %");
            Console.WriteLine($"{$"False Negative = {a.FalseNegative}, {a.FalseNegative * 100 / sum:00.000} % ",24}|{b.FalseNegative}, {b.FalseNegative * 100 / sum:00.000} %");
            Console.WriteLine("\n\n");
            return (a, b);
        }
    }
}