using MongoDB.Driver;
using TaskA;
namespace TaskB
{
    public class MongoConnector2 : MongoConnector
    {
        /// <summary>
        /// The collection of `Reader` objects
        /// </summary>
        protected readonly IMongoCollection<Reader_Accession_Column> _r_acc;
        protected readonly IMongoCollection<Reader_Issue_Column> _r_iss;
        protected readonly IMongoCollection<Reader_Return_Column> _r_ret;
        /// <summary>
        /// Constructor for this class
        /// </summary>
        /// <param name="pass">The password for the MongoDB Database</param>
        /// <returns></returns>
        public MongoConnector2(string pass) : base(pass)
        {
            _r_acc = _database.GetCollection<Reader_Accession_Column>("Readers_Accession_Column");
            _r_iss = _database.GetCollection<Reader_Issue_Column>("Readers_Issue_Column");
            _r_ret = _database.GetCollection<Reader_Return_Column>("Readers_Return_Column");
        }
        /// <summary>
        /// Deletes records from the `Books` collection. 
        /// Deletes additional records from `Readers` collection if necessary. 
        /// Returns a tuple indicating deleted records for both collections.
        /// </summary>
        /// <param name="accession_No">The key value of `Accession_No` to delete</param>
        /// <returns>Returns a tuple indicating number of documents deleted for both collections</returns>
        public (bool BookDeleted, bool DeletedReader) Delete(ulong accession_No)
        {
            if (_books.DeleteOne(x => x.Accession_No == accession_No).DeletedCount != 0) //If books deleted are not 0
            {
                Reader_Accession_Column? id = _r_acc.Find(x => x.Accession_No == accession_No).FirstOrDefault();
                if (id == null)
                    return (true, false); // No elements in Readers database are linked with this document
                else
                {
                    uint val = id.Reader_Id;
                    _r_acc.DeleteOne(x => x.Reader_Id == val);
                    _r_iss.DeleteOne(x => x.Reader_Id == val);
                    _r_ret.DeleteOne(x => x.Reader_Id == val);
                    return (true, true); // No elements now remain linked to this document in any collection
                }
            }
            else
                return (false, false); // No such document found!
        }
        /// <summary>
        /// Updates records from the `Books` collection. 
        /// Updates additional records from `Readers` collection if necessary. 
        /// Returns a tuple indicating updated records for both collections.
        /// </summary>
        /// <param name="accession_No">The key value of `Accession_No` to update</param>
        /// <returns>Returns a tuple indicating number of documents updated for both collections</returns>
        public (bool Updated, bool UpdatedReader) Update(ulong accession_No, Book book)
        {
            if (_books.Find(x => x.Accession_No == accession_No).Any())// If any book exists with this accession_no
            {
                ulong updated_accession_no = book.Accession_No;
                if (book.Category != "C" && book.Category != "Java" && book.Category != "Python" && book.Category != "DBMS")
                {
                    Console.WriteLine("ERROR: Category must only be one of 'C','Java','Python' and 'DBMS'");
                    return (false, false);
                }
                else if (accession_No != updated_accession_no && _books.Find(y => y.Accession_No == updated_accession_no).Any())
                {// If accession_no is being updated, the updated accession_no already exists in the collection, then this update would push duplicate elements
                    Console.WriteLine("ERROR: Accession_No must be unique in the collection!");
                    return (false, false);
                }
                else if (_books.Find(y => y.Accession_No == accession_No).First().ISBN != book.ISBN && _books.Find(x => x.ISBN == book.ISBN).Any())
                {// Same as above but for ISBN value
                    Console.WriteLine("ERROR: ISBN must be unique in the collection!");
                    return (false, false);
                }
                _books.DeleteOne(x => x.Accession_No == accession_No);
                _books.InsertOne(book);
                if (updated_accession_no != accession_No && _r_acc.Find(x => x.Accession_No == accession_No).Any())
                {// The Accession_No is updated and linked document exists in Readers
                    _r_acc.UpdateOne(x => x.Accession_No == accession_No, Builders<Reader_Accession_Column>.Update.Set("Accession_No", updated_accession_no));
                    return (true, true); //update the document with the updated accession_no
                }
                else return (true, false); // Updates are made to a record in Books, but not needed in Readers collection 
            }
            else return (false, false); // No updates made in any collection
        }
        /// <summary>
        /// Inserts a document in the `Readers` collection
        /// </summary>
        /// <param name="reader">The instance to insert</param>
        /// <returns>true if insertion is successful, otherwise false</returns>
        public bool Insert(Reader reader)
        {
            if (_r_acc.Find(x => x.Reader_Id == reader.Reader_Id).Any())
            {// Reader_ID is primary key and it must be unique
                Console.WriteLine("ERROR: Another document already exists with the same ID");
                return false;
            }
            else if (_r_acc.Find(x => x.Accession_No == reader.Accession_No).Any())
            {// There should be 1:1 mapping between Books and Readers
                Console.WriteLine("ERROR: This book is already issued to someone else!");
                return false;
            }
            else if (_books.Find(x => x.Accession_No == reader.Accession_No).Any()==false)
            {// A document with the given Accession_No must exist in the Books collection
                Console.WriteLine("ERROR: There does not exist a document with this accession number");
                return false;
            }
            _r_acc.InsertOne(new Reader_Accession_Column() { Reader_Id = reader.Reader_Id, Accession_No = reader.Accession_No });
            _r_iss.InsertOne(new Reader_Issue_Column() { Reader_Id = reader.Reader_Id, Issue_Date = reader.Issue_Date });
            _r_ret.InsertOne(new Reader_Return_Column() { Reader_Id = reader.Reader_Id, Return_Date = reader.Return_Date });
            return true; // Insertion is successful!
        }
        /// <summary>
        /// Deletes a document in the `Readers` collection
        /// </summary>
        /// <param name="reader_id">The `Reader_Id` value of the duocument to delete</param>
        /// <returns>true if deletion is successful, otherwise false</returns>
        public bool Delete(uint reader_id)
        {
            if (_r_acc.DeleteOne(x => x.Reader_Id == reader_id).DeletedCount != 0)
            {// There exists exactly one document, which is now being deleted
                _r_iss.DeleteOne(x => x.Reader_Id == reader_id);
                _r_ret.DeleteOne(x => x.Reader_Id == reader_id);
                return true;
            }
            else return false; // No such document exists in the collections
        }
        /// <summary>
        /// Updates a document with the given Reader_Id, and replaces it with the provided instance
        /// </summary>
        /// <param name="reader_id">The value of the reader_id of document to alter</param>
        /// <param name="reader">The instance which relpaces the docuement in the collection</param>
        /// <returns>true if updation is successful, otherwise false</returns>
        public bool Update(uint reader_id, Reader reader)
        {
            Reader_Accession_Column? element;
            if ((element = _r_acc.Find(r => r.Reader_Id == reader_id).FirstOrDefault()) == null)
            {// If no such docuement exists with the given reader_id, we can skip all actions
                Console.WriteLine("ERROR: Did not find the docuemnt to update!");
                return false;
            }
            else if (reader.Reader_Id != reader_id && _r_acc.Find(r => r.Reader_Id == reader.Reader_Id).Any())
            {// If a document is found and is to be updated with a *different* reader_id value, 
             // but this key value already exists, then the updation process cannot be done
                Console.WriteLine("ERROR: Another docuemnt already exists with updated instance's Reader_id");
                return false;
            }
            else if (_books.Find(b => b.Accession_No == reader.Accession_No).Any() == false)
            {// The foreign key for this document must exist to maintain referential integrity
                Console.WriteLine("ERROR: Could not find a document in Books having the updated accession_no");
                return false;
            }
            else if (element.Accession_No!=reader.Accession_No && _r_acc.Find(x=>x.Accession_No==reader.Accession_No).Any())
            {// The accession_no must stay unique
                Console.WriteLine("ERROR: The updated accession_no is not unique!");
                return false;
            }
            _r_acc.DeleteOne(x => x.Reader_Id == reader_id);
            _r_iss.DeleteOne(x => x.Reader_Id == reader_id);
            _r_ret.DeleteOne(x => x.Reader_Id == reader_id);
            // Delete document from the collection
            _r_acc.InsertOne(new Reader_Accession_Column() { Reader_Id = reader.Reader_Id, Accession_No = reader.Accession_No });
            _r_iss.InsertOne(new Reader_Issue_Column() { Reader_Id = reader.Reader_Id, Issue_Date = reader.Issue_Date });
            _r_ret.InsertOne(new Reader_Return_Column() { Reader_Id = reader.Reader_Id, Return_Date = reader.Return_Date });
            // Insert the updated docuements
            return true;
        }
        /// <summary>
        /// Returns an array of `Reader` objects from the `Readers` collection.
        /// </summary>
        /// <returns>Array of `Reader` objects</returns>
        public IEnumerable<Reader> GetAllReaders()
        {
            var p = _r_acc.Find(x => true).SortBy(x => x.Reader_Id).ToEnumerable() ?? null;
            var q = _r_iss.Find(x => true).SortBy(x => x.Reader_Id).ToEnumerable() ?? null;
            var r = _r_ret.Find(x => true).SortBy(x => x.Reader_Id).ToEnumerable() ?? null;
            if (p == null || q == null || r == null) throw new Exception("ERROR: Could not connect to the said database!");
            //  pq = Join p and q
            var pq = p.Join(q, x => x.Reader_Id, y => y.Reader_Id, (a, b) => (a, b)) ?? null;
            if (pq == null) throw new Exception("ERROR: Could not connect to the database!");
            //  z = Join pq and r
            var z = pq.Join(r, x => x.a.Reader_Id, y => y.Reader_Id, (a, b) => new Reader() { Reader_Id = b.Reader_Id, Accession_No = a.a.Accession_No, Issue_Date = a.b.Issue_Date, Return_Date = b.Return_Date });
            if (z == null) throw new Exception("ERROR: Could not connect to the database!");
            return z;
        }
    }
}