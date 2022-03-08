using MongoDB.Driver;
namespace TaskA
{
    public class MongoConnector
    {
        protected readonly string _password;
        protected readonly IMongoDatabase _database;
        protected readonly IMongoCollection<Book> _books;
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
        public bool Insert(Book book)
        {
            if(book.Category!="C"&&book.Category!="Java"&&book.Category!="Python"&&book.Category!="DBMS")
            return false;
            foreach(var otherbook in GetAllBooks())
            {
                if(book.ISBN==otherbook.ISBN||book.Accession_No==otherbook.Accession_No) return false;
            }// Check that Accession_No and ISBN are indeed unique
            _books.InsertOne(book);
            return true;
        }
        public IEnumerable<Book> GetAllBooks()
        {
            return _books.Find(s=>true).ToEnumerable();
        }
    }
}