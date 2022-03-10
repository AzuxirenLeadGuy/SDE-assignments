using MongoDB.Driver;
using TaskA;
namespace TaskB
{
    public class MongoConnectorB : MongoConnector
    {
        protected readonly IMongoCollection<Reader> _readers;
        public MongoConnectorB(string pass) : base(pass)
        {
            _readers = _database.GetCollection<Reader>("Readers");
        }
        public (bool BookDeleted, long DeletedReaders) Delete(ulong accession_No)
        {
            if (_books.DeleteOne(x => x.Accession_No == accession_No).DeletedCount != 0)
                return (true, _readers.DeleteMany(x => x.Accession_No == accession_No).DeletedCount);
            else
                return (false, 0);
        }
        public (bool Updated, long UpdatedReaders) Update(ulong accession_No, Book updatedbook)
        {
            ulong updated_accession_no = updatedbook.Accession_No;
            if (_books.ReplaceOne(x => x.Accession_No == accession_No, updatedbook).ModifiedCount != 0)
                if (updated_accession_no != accession_No)
                    return (true, _readers.UpdateMany(x => x.Accession_No == updated_accession_no, Builders<Reader>.Update.Set("Accession_No", updated_accession_no)).MatchedCount);
                else return (true, 0);
            else return (false, 0);
        }
        public bool Insert(Reader reader)
        {
            if (_readers.Find(x => x.Reader_Id == reader.Reader_Id).Any())
                return false;
            _readers.InsertOne(reader);
            return true;
        }
        public bool DeleteReader(uint reader_id)
        {
            return _readers.DeleteOne(x => x.Reader_Id == reader_id).DeletedCount != 0;
        }
        public IEnumerable<Reader> GetAllReaders()
        {
            return _readers.Find(x => true).ToEnumerable();
        }
    }
}