using MongoDB.Bson.Serialization.Attributes;

namespace TaskA
{
    [BsonIgnoreExtraElements]
    public class Book
    {
        public string ISBN { get; set; }
        public ulong Accession_No { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Publisher { get; set; }
        public Book()
        {
            ISBN = Title = Author = Publisher = "";
            Category = "C";
        }
        public int Edition { get; set; }
        public int YearOfPublication { get; set; }
        public string Category { get; set; }
        public int Pages { get; set; }
        public float Price { get; set; }
        public string SQL=>$"INSERT INTO Books VALUES ('{ISBN}', '{Accession_No}', '{Title}', '{Author}', '{Publisher}', {Edition}, {YearOfPublication}, '{Category}', {Pages}, {Price});";
    };
}