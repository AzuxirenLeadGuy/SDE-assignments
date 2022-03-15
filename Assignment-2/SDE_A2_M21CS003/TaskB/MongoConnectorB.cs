using MongoDB.Driver;
using TaskA;
namespace TaskB
{
    public class MongoConnector2 : MongoConnector
    {
        /// <summary>
        /// The collection of `Reader` objects
        /// </summary>
        protected readonly IMongoCollection<Reader> _readers;
        /// <summary>
        /// Constructor for this class
        /// </summary>
        /// <param name="pass">The password for the MongoDB Database</param>
        /// <returns></returns>
        public MongoConnector2(string pass) : base(pass)
        {
            _readers = _database.GetCollection<Reader>("Readers");
        }
        /// <summary>
        /// Deletes records from the `Books` collection. 
        /// Deletes additional records from `Readers` collection if necessary. 
        /// Returns a tuple indicating deleted records for both collections.
        /// </summary>
        /// <param name="accession_No">The key value of `Accession_No` to delete</param>
        /// <returns>Returns a tuple indicating number of documents deleted for both collections</returns>
        public (bool BookDeleted, long DeletedReaders) Delete(ulong accession_No)
        {
            if (_books.DeleteOne(x => x.Accession_No == accession_No).DeletedCount != 0) //If books deleted are not 0
                return (true, _readers.DeleteMany(x => x.Accession_No == accession_No).DeletedCount);  //delete all records that are linked
            else
                return (false, 0);
        }
        /// <summary>
        /// Updates records from the `Books` collection. 
        /// Updates additional records from `Readers` collection if necessary. 
        /// Returns a tuple indicating updated records for both collections.
        /// </summary>
        /// <param name="accession_No">The key value of `Accession_No` to update</param>
        /// <returns>Returns a tuple indicating number of documents updated for both collections</returns>
        public (bool Updated, long UpdatedReaders) Update(ulong accession_No, Book book)
        {
            if (_books.Find(x => x.Accession_No == accession_No).Any())// If any book exists with this accession_no
            {
                ulong updated_accession_no = book.Accession_No;
                if (book.Category != "C" && book.Category != "Java" && book.Category != "Python" && book.Category != "DBMS")
                {
                    Console.WriteLine("ERROR: Category must only be one of 'C','Java','Python' and 'DBMS'");
                    return (false, 0);
                }
                else if (accession_No != updated_accession_no && _books.Find(y => y.Accession_No == updated_accession_no).Any())
                {// If accession_no is being updated, the updated accession_no already exists in the collection, then this update would push duplicate elements
                    Console.WriteLine("ERROR: Accession_No must be unique in the collection!");
                    return (false, 0);
                }
                else if (_books.Find(y => y.Accession_No == accession_No).First().ISBN != book.ISBN && _books.Find(x => x.ISBN == book.ISBN).Any())
                {// Same as above but for ISBN value
                    Console.WriteLine("ERROR: ISBN must be unique in the collection!");
                    return (false, 0);
                }
                _books.DeleteOne(x=>x.Accession_No==accession_No);
                _books.InsertOne(book);
                if (updated_accession_no != accession_No) // and accession number is updated
                {
                    return (true, _readers.UpdateMany(x => x.Accession_No == accession_No, Builders<Reader>.Update.Set("Accession_No", updated_accession_no)).MatchedCount); //update all records to the updated accession_no
                }
                else return (true, 0); // Updates are made to a record in Books, but not needed in Readers collection 
            }
            else return (false, 0); // No updates made in any collection
        }
        /// <summary>
        /// Inserts a document in the `Readers` collection
        /// </summary>
        /// <param name="reader">The instance to insert</param>
        /// <returns>true if insertion is successful, otherwise false</returns>
        public bool Insert(Reader reader)
        {
            if (_readers.Find(x => x.Reader_Id == reader.Reader_Id).Any())
            {// Reader_ID is primary key and it must be unique
                Console.WriteLine("ERROR: Another document already exists with the same ID");
                return false;
            }
            else if (_books.Find(x => x.Accession_No == reader.Accession_No).CountDocuments() == 0)
            {// A document with the given Accession_No must exist in the Books collection
                Console.WriteLine("ERROR: There does not exist a document with this accession number");
                return false;
            }
            _readers.InsertOne(reader);
            return true;
        }
        /// <summary>
        /// Deletes a document in the `Readers` collection
        /// </summary>
        /// <param name="reader_id">The `Reader_Id` value of the duocument to delete</param>
        /// <returns>true if deletion is successful, otherwise false</returns>
        public bool Delete(uint reader_id)
        {
            return _readers.DeleteOne(x => x.Reader_Id == reader_id).DeletedCount != 0;
        }
        /// <summary>
        /// Updates a document with the given Reader_Id, and replaces it with the provided instance
        /// </summary>
        /// <param name="reader_id">The value of the reader_id of document to alter</param>
        /// <param name="reader">The instance which relpaces the docuement in the collection</param>
        /// <returns>true if updation is successful, otherwise false</returns>
        public bool Update(uint reader_id, Reader reader)
        {
            if (!_readers.Find(r => r.Reader_Id == reader_id).Any())
            {// If no such docuement exists with the given reader_id, we can skip all actions
                Console.WriteLine("ERROR: Did not find the docuemnt to update!");
                return false;
            }
            else if (reader.Reader_Id != reader_id && _readers.Find(r => r.Reader_Id == reader.Reader_Id).Any())
            {// If a document is found and is to be updated with a *different* reader_id value, 
             // but this key value already exists, then the updation process cannot be done
                Console.WriteLine("ERROR: Another docuemnt already exists with updated instance's Reader_id");
                return false;
            }
            else if (!_books.Find(b => b.Accession_No == reader.Accession_No).Any())
            {// The foreign key for this document must exist to maintain referential integrity
                Console.WriteLine("ERROR: Could not find a document in Books having the updated accession_no");
                return false;
            }
            _readers.DeleteOne(x => x.Reader_Id == reader_id);
            _readers.InsertOne(reader);
            return true;
        }
        /// <summary>
        /// Returns an array of `Reader` objects from the `Readers` collection.
        /// </summary>
        /// <returns>Array of `Reader` objects</returns>
        public IEnumerable<Reader> GetAllReaders()
        {
            return _readers.Find(x => true).ToEnumerable();
        }
    }
}