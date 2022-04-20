using Xunit;
using System;
using System.Linq;
using System.Collections.Generic;
using SDE_Project;
namespace Tests;

public class UnitTest
{
    [Fact]
    public void HashForIntTest()
    {
        for (int i = 0; i < 1000; i++)
        {
            Assert.True(i == i.GetHashCode());
        }
    }
    [Fact]
    public void StandardBloomFilterTest1()
    {
        StandardBloomFilter<int> filter = new(5, new HashStruct[] { new(1, 0), new(2, 1), new(3, 2) });
        for (int i = 0; i < 1000; i++)
        {
            Assert.False(filter.Check(i));
        }
        System.Console.WriteLine("Testing script 1");
        filter.Insert(0);
        Assert.False(filter.Check(1));
        Assert.True(filter.Check(5));
        filter.Insert(1);
        Assert.True(filter.Check(0));
        Assert.True(filter.Check(1));
        Assert.True(filter.Check(2));
    }
    protected static void GenericTestHABF(int bits, byte K, byte ExtraHash)
    {
        var (std, habf) = Program.GenericTest(bits, K, ExtraHash);
        Assert.True(std.FalseNegative == 0);
        Assert.True(habf.FalseNegative == 0);
        Assert.Equal(std.FalsePositive + std.TrueNegative + std.TruePositive, habf.FalsePositive + habf.TrueNegative + habf.TruePositive);
        Assert.Equal(std.TruePositive, habf.TruePositive);
        Assert.True(std.TrueNegative <= habf.TrueNegative);
    }
    [Fact]
    public void HABFIntTest1()
    {
        GenericTestHABF(32, 2, 8);
    }
    [Fact]
    public void HABFIntTest2()
    {
        GenericTestHABF(128, 3, 16);
    }
    [Fact]
    public void HABFIntTest3()
    {
        GenericTestHABF(2048, 4, 24);
    }
}