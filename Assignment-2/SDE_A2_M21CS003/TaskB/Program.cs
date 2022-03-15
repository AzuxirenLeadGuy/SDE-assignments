using TaskA;
namespace TaskB
{
    public static class Program
    {
        public static void Main()
        {
            Console.WriteLine("Now connecting to MongoDB database. Please ensure your internet connection is active.");
            Console.Write("Enter the password for MongoDB Atlas database for user \"iitj_free\": ");
            string password = Console.ReadLine() ?? "";
            try
            {
                char prompt;
                ulong acc_no;
                uint read_id;
                MongoConnector2 con = new(password);
                do
                {
                    Console.WriteLine("1. Insert a document in 'Books' collection");
                    Console.WriteLine("2. Update a document in 'Books' collection");
                    Console.WriteLine("3. Delete a document in 'Books' collection");
                    Console.WriteLine("4. See all documents in 'Books' collection");
                    Console.WriteLine("5. Insert a document in 'Readers' collection");
                    Console.WriteLine("6. Update a document in 'Readers' collection");
                    Console.WriteLine("7. Delete a document in 'Readers' collection");
                    Console.WriteLine("8. See all documents in 'Readers' collection");
                    Console.WriteLine("9. Exit\n\nEnter your choice: ");
                    prompt = Console.ReadKey().KeyChar;
                    Console.WriteLine();
                    switch (prompt)
                    {
                        case '1':
                            Console.WriteLine("Now taking input of Book structure: ");
                            if (con.Insert(ConsoleInputBook()))
                                Console.WriteLine("\nBook inserted into the collection successfully!");
                            else
                                Console.WriteLine("\nCould not insert book in the collection!!");
                            break;
                        case '2':
                            Console.Write("Enter the accession_no of the book to update: ");
                            acc_no = ulong.Parse(Console.ReadLine() ?? "0");
                            Console.WriteLine("Enter book to update the record with");
                            var (Updated, UpdatedReaders) = con.Update(acc_no, ConsoleInputBook());
                            if (Updated)
                                Console.WriteLine($"Book is updated, and {UpdatedReaders} readers document also affected");
                            else
                                Console.WriteLine("No documents updated!");
                            break;
                        case '3':
                            Console.Write("Enter the accession_no of the book to update: ");
                            acc_no = ulong.Parse(Console.ReadLine() ?? "0");
                            var (BookDeleted, DeletedReaders) = con.Delete(acc_no);
                            if (BookDeleted)
                                Console.WriteLine($"Book is deleted, and {DeletedReaders} readers document also affected");
                            else
                                Console.WriteLine("No docuements deleted!");
                            break;
                        case '4':
                            Display(con.GetAllBooks());
                            break;
                        case '5':
                            Console.WriteLine("Now taking input of Reader structure: ");
                            if (con.Insert(ConsoleInputReader()))
                                Console.WriteLine("\nBook inserted into the collection successfully!");
                            else
                                Console.WriteLine("\nCould not insert book in the collection!!");
                            break;
                        case '6':
                            Console.Write("Enter the reader_id of the book to update: ");
                            read_id = uint.Parse(Console.ReadLine() ?? "0");
                            Console.WriteLine("Now taking input of Book structure: ");
                            if (con.Update(read_id, ConsoleInputReader()))
                                Console.WriteLine("\nReader updated into the collection successfully!");
                            else
                                Console.WriteLine("\nCould not update reader in the collection!!");
                            break;
                        case '7':
                            Console.Write("Enter the reader_id of the book to update: ");
                            read_id = uint.Parse(Console.ReadLine() ?? "0");
                            if (con.Delete(read_id))
                                Console.WriteLine("Document deleted successfully!");
                            else
                                Console.WriteLine("Could not delete the document");
                            break;
                        case '8':
                            Display(con.GetAllReaders());
                            break;
                        default:
                            Console.WriteLine("Invalid choice! try again!!\n\n");
                            goto case '9';
                        case '9':
                            break;
                    }
                    Console.WriteLine("\n______________________________\n");
                } while (prompt != '9');

            }
            catch (Exception e)
            {
                Console.Write("Error connecting to MongoDB database: ");
                Console.WriteLine(e.Message);
            }
        }
        public static TaskA.Book ConsoleInputBook() => TaskA.Program.ConsoleInput();
        public static Reader ConsoleInputReader()
        {
            Reader reader = new();
            Console.Write("Enter Reader_ID: ");
            reader.Reader_Id = uint.Parse(Console.ReadLine() ?? "0");
            Console.Write("Enter Accession_No: ");
            reader.Accession_No = ulong.Parse(Console.ReadLine() ?? "0");
            var today = DateOnly.FromDateTime(DateTime.Today);
            reader.Issue_Date = new() { Day = today.Day, Month = today.Month, Year = today.Year };
            Console.WriteLine("Issue date is set for today.\nEnter days for return from today: ");
            byte days = byte.Parse(Console.ReadLine() ?? "0");
            today = DateOnly.FromDateTime(DateTime.Today + TimeSpan.FromDays(days));
            reader.Return_Date = new() { Day = today.Day, Month = today.Month, Year = today.Year };
            return reader;
        }
        public static void Display(IEnumerable<Book> books)
        {
            foreach (var book in books)
            {
                Console.WriteLine($"[{book.ISBN,7},{book.Accession_No.ToString().PadLeft(12, '0')}, {book.Title,18}, {book.Author, 12}, {book.Publisher,12}, {book.Edition, 2}, {book.YearOfPublication, 4}, {book.Category,6}, {book.Pages, 5}, {book.Price, 5} ]");
            }
        }
        public static void Display(IEnumerable<Reader> readers)
        {
            foreach (var reader in readers)
            {
                Console.WriteLine($"[{reader.Reader_Id,4}, {reader.Accession_No,12}, {reader.Issue_Date,8}, {reader.Return_Date,8} ]");
            }
        }
    }
}