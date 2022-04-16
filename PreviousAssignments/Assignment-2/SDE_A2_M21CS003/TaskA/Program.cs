using MySql.Data.MySqlClient;
namespace TaskA
{
    public static class Program
    {
        public static void Main()
        {
            Console.WriteLine("Now connecting to MongoDB database. Please ensure your internet connection is active.");
            Console.Write("Enter the password for MongoDB Atlas database for user \"iitj_free\": ");
            string password = Console.ReadLine() ?? "";
            char prompt;
            try
            {
                MongoConnector connector = new(password);
                Console.WriteLine("Connection to database successful!");
                do
                {
                    Console.WriteLine("\nYou will now be prompted to enter a document in the database");
                    Book bk = ConsoleInput();
                    if (connector.Insert(bk))
                    {
                        Console.WriteLine("Book inserted successfully!\n Now inserting into an SQL table in MySQL...");
                        InsertSQL(bk);
                        Console.WriteLine("Book is now inserted. All entries in SQL table:");
                        ViewSQL();
                    }
                    else
                    {
                        Console.WriteLine("Document is not valid. Please ensure that all constraints are followed!");
                    }
                    Console.WriteLine("\n\nEnter another document? (Press Y/y)");
                    prompt = Console.ReadKey(false).KeyChar;
                } while (prompt == 'y' || prompt == 'Y');
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        public static Book ConsoleInput()
        {
            Book book = new();
            Console.Write("Enter ISBN: ");
            book.ISBN = Console.ReadLine() ?? "00000000";
            Console.Write("Enter Accession number: ");
            book.Accession_No = ulong.Parse(Console.ReadLine() ?? "0");
            Console.Write("Enter Title: ");
            book.Title = Console.ReadLine() ?? " ";
            Console.Write("Enter Author: ");
            book.Author = Console.ReadLine() ?? " ";
            Console.Write("Enter Publisher: ");
            book.Publisher = Console.ReadLine() ?? " ";
            Console.Write("Enter Edition: ");
            book.Edition = int.Parse(Console.ReadLine() ?? "0");
            Console.Write("Enter Year: ");
            book.YearOfPublication = int.Parse(Console.ReadLine() ?? "0");
            Console.Write("Enter Pages: ");
            book.Pages = int.Parse(Console.ReadLine() ?? "0");
            Console.Write("Enter Price: ");
            book.Price = float.Parse(Console.ReadLine() ?? "0");
            Console.Write("Enter Category: (Should be any one of Java, Python, DBMS, C)  : ");
            book.Category = Console.ReadLine() ?? "C";
            return book;
        }
        public static void InsertSQL(Book book)
        {
            using var connection = new MySqlConnection
            (
                $"server=localhost;userid=iitj_free;password=12345678;database=LibraryDB;"
            );
            connection.Open();
            using var transaction = connection.BeginTransaction();
            string cmdstr = book.SQL;
            Console.WriteLine($"Using command {cmdstr}");
            using var command = new MySqlCommand(book.SQL, connection, transaction);
            command.ExecuteNonQuery();
            transaction.Commit();
        }
        public static void ViewSQL()
        {
            using var connection = new MySqlConnection
            (
                $"server=localhost;userid=iitj_free;password=12345678;database=LibraryDB;"
            );
            connection.Open();
            using var transaction = connection.BeginTransaction();
            using var selectcommand = new MySqlCommand("SELECT * FROM Books", connection, transaction);
            using var reader = selectcommand.ExecuteReader();
            while (reader.Read())
            {
                var values = new object[reader.FieldCount];
                reader.GetValues(values);
                Console.WriteLine(string.Join("\t|\t", values));
            }
        }
    }
}