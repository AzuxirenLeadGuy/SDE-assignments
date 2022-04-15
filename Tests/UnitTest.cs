using Xunit;
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
        filter.Insert(0);
        Assert.False(filter.Check(1));
        Assert.True(filter.Check(5));
        filter.Insert(1);
        Assert.True(filter.Check(0));
        Assert.True(filter.Check(1));
        Assert.True(filter.Check(2));
    }
}