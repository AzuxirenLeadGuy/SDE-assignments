namespace SDE_Project
{
    public class HABF<ItemType> : StandardBloomFilter<ItemType>
    {
        public HABF(int bits, HashStruct[] hashes) : base(bits, hashes)
        {
        }
    }
}