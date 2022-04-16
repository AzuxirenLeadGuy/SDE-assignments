using MongoDB.Bson.Serialization.Attributes;

namespace TaskB
{
    /// <summary>
    /// My custom implementation of Date, so that I can print it however I like
    /// </summary>

    [BsonIgnoreExtraElements]
    public class SimpleDate
    {
        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public override string ToString()
        {
            return $"{Day}/{Month}/{Year}";
        }
        public SimpleDate()
        {
            Day = Month = Year = 1;
        }
    }
    /// <summary>
    /// Reader structure to store in NoSQL
    /// </summary>
    [BsonIgnoreExtraElements]
    public class Reader
    {
        public uint Reader_Id { get; set; }
        public ulong Accession_No { get; set; }
        public SimpleDate Issue_Date { get; set; }
        public SimpleDate Return_Date { get; set; }
        public Reader()
        {
            Issue_Date=new();
            Return_Date=new();
        }
    }
    /// <summary>
    /// Reader structure to store in NoSQL
    /// </summary>
    [BsonIgnoreExtraElements]
    public class Reader_Accession_Column
    {
        public uint Reader_Id {get; set;}
        public ulong Accession_No { get; set; }
    }
    /// <summary>
    /// Reader structure to store in NoSQL
    /// </summary>
    [BsonIgnoreExtraElements]
    public class Reader_Issue_Column
    {
        public uint Reader_Id {get; set;}
        public SimpleDate Issue_Date { get; set; }
        public Reader_Issue_Column()
        {
            Issue_Date=new();
        }
    }
    /// <summary>
    /// Reader structure to store in NoSQL
    /// </summary>
    [BsonIgnoreExtraElements]
    public class Reader_Return_Column
    {
        public uint Reader_Id {get; set;}
        public SimpleDate Return_Date { get; set; }
        public Reader_Return_Column()
        {
            Return_Date=new();
        }
    }
}