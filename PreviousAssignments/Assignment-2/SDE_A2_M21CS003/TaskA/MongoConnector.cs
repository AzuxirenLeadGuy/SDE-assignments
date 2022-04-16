using MongoDB.Driver;
namespace TaskA
{
    /// <summary>
    /// Maintains connection with the MongoDB Atlas database for this assignment
    /// </summary>
    public class MongoConnector
    {
        /// <summary>
        /// The password for the connector
        /// </summary>
        protected readonly string _password;
        /// <summary>
        /// The Database containing all collections
        /// </summary>
        protected readonly IMongoDatabase _database;
        /// <summary>
        /// The collection `Books` within the database
        /// </summary>
        protected readonly IMongoCollection<Book> _books;
        /// <summary>
        /// The constructor for this class
        /// </summary>
        /// <param name="pass">The password for the connection</param>
        public MongoConnector(string pass)
        {
            _password = pass;
            var settings = MongoClientSettings.FromConnectionString($"mongodb+srv://iitj_free:{_password}@cluster0.i53a4.mongodb.net/myFirstDatabase?retryWrites=true&w=majority");
            settings.ServerApi = new ServerApi(ServerApiVersion.V1);
            MongoClient? client = new(settings);
            _ = client.ListDatabases(); // Exception is thrown here if password is incorrect
            var d = client.GetDatabase("LibraryDB") ?? null;
            if (d == null) throw new ArgumentException("Could not connect to the Database! Password is incorrect!");
            else _database = d;
            _books = _database.GetCollection<Book>("Books");
        }
        /// <summary>
        /// Returns an enumeration of all documents in the collection `Books`
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Book> GetAllBooks()
        {
            return _books.Find(s => true).ToEnumerable();
        }
        /// <summary>
        /// Inserts a document in the collection `Books`
        /// </summary>
        /// <param name="book"></param>
        /// <returns>true if insertion is successful, otherwise false</returns>
        public bool Insert(Book book)
        {
            if (book.Category != "C" && book.Category != "Java" && book.Category != "Python" && book.Category != "DBMS")
            {
                Console.WriteLine("ERROR: Category must only be one of 'C','Java','Python' and 'DBMS'");
                return false;
            }
            else if(_books.Find(x=>x.ISBN==book.ISBN).Any())
            {
                Console.WriteLine("ERROR: ISBN must be unique in the collection!");
                return false;
            }
            else if(_books.Find(y=>y.Accession_No==book.Accession_No).Any())
            {
                Console.WriteLine("ERROR: Accession_No must be unique in the collection!");
                return false;
            }
            _books.InsertOne(book);
            return true;
        }
    }
}