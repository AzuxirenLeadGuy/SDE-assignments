#  SDE Assignment 2

Submitted by Ashish Jacob Sam(M21CS003)

---

## Implementation notes and assumptions

- This project was programmed using [.NET SDK 6](https://dotnet.microsoft.com/en-us/) and was compiled/built on a linux machine. It is expected to work in a windows/mac machine as well as long as .NET SDK is installed.

- For NoSQL Database, [MongoDB Atlas](https://www.mongodb.com/atlas/database) is used, which offers a cloud based solution for storing NoSQL database. Similarly, for SQL table, [MySQL](https://mysql.com) is used for SQL server, which is a universal open source database.

- For mongoDB atlas remote access, the password "abcd" is required. This password should be manually entered in the program. This password must be manually entered for both the programs Task1 and Task2.

- Task-1 features only insertion operations performed on the predefined `Books` collection in NoSQL and SQL. 

- Task-2 features CRUD operations for both the collections `Books` and `Readers` in NoSQL only, while maintaining referential integrity. SQL is not at all used in Task-2.

- Task-3 features a .NET API hosted on the local machine, and [Postman]() tool is used to test the API. The images are taken as input in the raw content of POST request, and details regarding the Face and Logo detection is returned.

---

# Task 1

## Implementation

MongoDB Atlas is chosen for storing data on the cloud, while MySQL server is chosen for storing database in relational database. For the assignment, C# is used, and a class is created with the following Schema

```csharp
    public class Book
    {
        public string ISBN { get; set; }
        public ulong Accession_No { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Publisher { get; set; }
        public int Edition { get; set; }
        public int YearOfPublication { get; set; }
        public string Category { get; set; }
        public int Pages { get; set; }
        public float Price { get; set; }
    };
```

Each attribute that is to be used by MongoDB is to be marked as a property in C#, for which the `{ get; set; }` token is used.

The following SQL query creates the required table in MySQL:

```sql
create table Books
( 
    ISBN varchar(20) PRIMARY KEY, 
    AccessNo DECIMAL(18) UNIQUE NOT NULL, 
    Title varchar(20), 
    Author varchar(20), 
    Publisher varchar(20), 
    Edition integer, 
    Year_Pub integer, 
    Category varchar(10) Check(Category in ('C','Java','DBMS','Python')),
    Pages integer, 
    Price Decimal(18,4)
);
```

This program accepts input from the user to create an instance of Book and insert the document to the MongoDB database as well as push it to the local MySQL database. The class `book` also provides the following method to generate a SQL script for insertion of an instance as shown:

```csharp
    public class Book
    {
        public string SQL=>$"INSERT INTO Books VALUES ('{ISBN}', '{Accession_No}', '{Title}', '{Author}', '{Publisher}', {Edition}, {YearOfPublication}, '{Category}', {Pages}, {Price});";
    }
```

A class `MongoConnector` is prepared for the connection to the MongoDB Atlas Database as shown:

```csharp

    public class MongoConnector
    {
        protected readonly string _password;
        protected readonly IMongoDatabase _database;
        protected readonly IMongoCollection<Book> _books;
        public MongoConnector(string pass) {...}
        public IEnumerable<Book> GetAllBooks() {...}
        public bool Insert(Book book) {...}
    }
```
This class is responsible for

- Connection to the required MongoDB Atlas cluster
- Initialize the Collections within the database
- Provide the following methods:
    - **`GetAllBooks()`** : Returns the content of all documents in the collection in form of an array of `Book` instances,
    - **`Insert(Book book)`** : Insert an instance of `Book` to the collection. Since the fields `ISBN` and `Accession_No` is a candidate key of the database, it is ensured that they are unique in the collection. The category is also ensured that is only one among {"C", "Java", "Python", "DBMS"} before inserting the document into the collection. This method returns a boolean to signify the success of insertion process 

Finally, there are methods provided to insert the into the SQL table as well as shown:

```csharp

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
```
Needless to say, this SQL script works on the MySQL with username "iitj_free", password "12345678", database name as "LibrarDB" and Table name as "Books".

Both these methods access the hardcoded SQL table in MySQL to insert and display the entries in the SQL table.

## Compilation and Execution output:

Use the command `dotnet run` in the folder of `TaskA.csproj` to execute the application. The sample output is as shown:

**NOTE:** The sample output is a bit too large, since after every insertion the entire table is shown. The following output is truncated, that is, only the first and final bits of the output is included

```
Now connecting to MongoDB database. Please ensure your internet connection is active.
Enter the password for MongoDB Atlas database for user "iitj_free": abcd
Connection to database successful!

You will now be prompted to enter a document in the database
Enter ISBN: a0001
Enter Accession number: 110320220001
Enter Title: Programming in C
Enter Author: Ashish
Enter Publisher: IIT-J
Enter Edition: 1
Enter Year: 2002
Enter Pages: 20
Enter Price: 100
Enter Category: (Should be any one of Java, Python, DBMS, C)  : C
Book inserted successfully!
 Now inserting into an SQL table in MySQL...
Using command INSERT INTO Books VALUES ('a0001', '110320220001', 'Programming in C', 'Ashish', 'IIT-J', 1, 2002, 'C', 20, 100);
Book is now inserted. All entries in SQL table:
a0001   |       110320220001    |       Programming in C        |       Ashish  |       IIT-J   |       1       |       2002    |       C       |       20      |       100.0000


Enter another document? (Press Y/y)
y
You will now be prompted to enter a document in the database
Enter ISBN: a0002
Enter Accession number: 110320220002
Enter Title: Programming in C
Enter Author: Ashish
Enter Publisher: IIT-J
Enter Edition: 2
Enter Year: 2003
Enter Pages: 25
Enter Price: 110
Enter Category: (Should be any one of Java, Python, DBMS, C)  : C
Book inserted successfully!
 Now inserting into an SQL table in MySQL...
Using command INSERT INTO Books VALUES ('a0002', '110320220002', 'Programming in C', 'Ashish', 'IIT-J', 2, 2003, 'C', 25, 110);
Book is now inserted. All entries in SQL table:
a0001   |       110320220001    |       Programming in C        |       Ashish  |       IIT-J   |       1       |       2002    |       C       |       20      |       100.0000
a0002   |       110320220002    |       Programming in C        |       Ashish  |       IIT-J   |       2       |       2003    |       C       |       25      |       110.0000



.
.
.

You will now be prompted to enter a document in the database
Enter ISBN: d0003
Enter Accession number: 110320220010
Enter Title: DBMS in a nutshell
Enter Author: Peter Irwin
Enter Publisher: OReilly Media
Enter Edition: 3
Enter Year: 2017
Enter Pages: 513
Enter Price: 850
Enter Category: (Should be any one of Java, Python, DBMS, C)  : DBMS
Book inserted successfully!
 Now inserting into an SQL table in MySQL...
Using command INSERT INTO Books VALUES ('d0003', '110320220010', 'DBMS in a nutshell', 'Peter Irwin', 'OReilly Media', 3, 2017, 'DBMS', 513, 850);
Book is now inserted. All entries in SQL table:
a0001   |       110320220001    |       Programming in C        |       Ashish  |       IIT-J   |       1       |       2002    |       C       |       20      |       100.0000
a0002   |       110320220002    |       Programming in C        |       Ashish  |       IIT-J   |       2       |       2003    |       C       |       25      |       110.0000
a0003   |       110320220003    |       Programming in C        |       Ashish  |       IIT-J   |       3       |       2004    |       C       |       30      |       120.0000
b0001   |       110320220004    |       Java Programming        |       Steve Bruce     |       Pearson |       1       |       2008    |       Java    |       1438    |       2000.0000
b0002   |       110320220005    |       Java Programming        |       Steve Bruce     |       Pearson |       2       |       2010    |       Java    |       1574    |       2100.0000
c0001   |       110320220006    |       Learning Python |       Shirly Courts   |       Wiley   |       1       |       2010    |       Python  |       305     |       500.0000
c0002   |       110320220007    |       Learning Python |       Shirly Courts   |       Wiley   |       2       |       2011    |       Python  |       411     |       800.0000
d0001   |       110320220008    |       DBMS in a nutshell      |       Peter Irwin     |       OReilly Media   |       1       |       2015    |       DBMS    |       210     |       400.0000
d0002   |       110320220009    |       DBMS in a nutshell      |       Peter Irwin     |       OReilly Media   |       2       |       2016    |       DBMS    |       453     |       700.0000
d0003   |       110320220010    |       DBMS in a nutshell      |       Peter Irwin     |       OReilly Media   |       3       |       2017    |       DBMS    |       513     |       850.0000


Enter another document? (Press Y/y)
n


```

The contents are shown in MongoDB Atlas as shown, using the following python script:

```
import pymongo                  # pip install pymongo && pip install dnspython
from pandas import DataFrame    # pip install pandas
client = pymongo.MongoClient("mongodb+srv://iitj_free:abcd@cluster0.i53a4.mongodb.net/LibraryDB?retryWrites=true&w=majority")
df = DataFrame(client['LibraryDB']['Books'].find())
print(df)
```

Running this script yields the following

```
$ python3 ReadMongoDB.py 
                        _id   ISBN  Accession_No               Title         Author      Publisher  Edition  YearOfPublication Category  Pages   Price
0  622ad85f12ac82907da3035e  a0001  110320220001    Programming in C         Ashish          IIT-J        1               2002        C     20   100.0
1  622ad88e12ac82907da32906  a0002  110320220002    Programming in C         Ashish          IIT-J        2               2003        C     25   110.0
2  622ad8c712ac82907da35281  a0003  110320220003    Programming in C         Ashish          IIT-J        3               2004        C     30   120.0
3  622ad90412ac82907da37da5  b0001  110320220004    Java Programming    Steve Bruce        Pearson        1               2008     Java   1438  2000.0
4  622ad93d12ac82907da3a7f2  b0002  110320220005    Java Programming    Steve Bruce        Pearson        2               2010     Java   1574  2100.0
5  622ad9b812ac82907da4025c  c0001  110320220006     Learning Python  Shirly Courts          Wiley        1               2010   Python    305   500.0
6  622ada0b12ac82907da43f8c  c0002  110320220007     Learning Python  Shirly Courts          Wiley        2               2011   Python    411   800.0
7  622ada6012ac82907da47986  d0001  110320220008  DBMS in a nutshell    Peter Irwin  OReilly Media        1               2015     DBMS    210   400.0
8  622adb9112ac82907da56d3e  d0002  110320220009  DBMS in a nutshell    Peter Irwin  OReilly Media        2               2016     DBMS    453   700.0
9  622adbc312ac82907da597e8  d0003  110320220010  DBMS in a nutshell    Peter Irwin  OReilly Media        3               2017     DBMS    513   850.0
```

This data also exists in the mysql table as shown:

```
$sudo mysql
Welcome to the MySQL monitor.  Commands end with ; or \g.
Your MySQL connection id is 14
Server version: 8.0.28-0ubuntu0.21.10.3 (Ubuntu)

Copyright (c) 2000, 2022, Oracle and/or its affiliates.

Oracle is a registered trademark of Oracle Corporation and/or its
affiliates. Other names may be trademarks of their respective
owners.

Type 'help;' or '\h' for help. Type '\c' to clear the current input statement.

mysql> use LibraryDB;select * from Books;
Reading table information for completion of table and column names
You can turn off this feature to get a quicker startup with -A

Database changed
+-------+--------------+--------------------+---------------+---------------+---------+----------+----------+-------+-----------+
| ISBN  | AccessNo     | Title              | Author        | Publisher     | Edition | Year_Pub | Category | Pages | Price     |
+-------+--------------+--------------------+---------------+---------------+---------+----------+----------+-------+-----------+
| a0001 | 110320220001 | Programming in C   | Ashish        | IIT-J         |       1 |     2002 | C        |    20 |  100.0000 |
| a0002 | 110320220002 | Programming in C   | Ashish        | IIT-J         |       2 |     2003 | C        |    25 |  110.0000 |
| a0003 | 110320220003 | Programming in C   | Ashish        | IIT-J         |       3 |     2004 | C        |    30 |  120.0000 |
| b0001 | 110320220004 | Java Programming   | Steve Bruce   | Pearson       |       1 |     2008 | Java     |  1438 | 2000.0000 |
| b0002 | 110320220005 | Java Programming   | Steve Bruce   | Pearson       |       2 |     2010 | Java     |  1574 | 2100.0000 |
| c0001 | 110320220006 | Learning Python    | Shirly Courts | Wiley         |       1 |     2010 | Python   |   305 |  500.0000 |
| c0002 | 110320220007 | Learning Python    | Shirly Courts | Wiley         |       2 |     2011 | Python   |   411 |  800.0000 |
| d0001 | 110320220008 | DBMS in a nutshell | Peter Irwin   | OReilly Media |       1 |     2015 | DBMS     |   210 |  400.0000 |
| d0002 | 110320220009 | DBMS in a nutshell | Peter Irwin   | OReilly Media |       2 |     2016 | DBMS     |   453 |  700.0000 |
| d0003 | 110320220010 | DBMS in a nutshell | Peter Irwin   | OReilly Media |       3 |     2017 | DBMS     |   513 |  850.0000 |
+-------+--------------+--------------------+---------------+---------------+---------+----------+----------+-------+-----------+
10 rows in set (0.00 sec)
```

## Conclusion

Thus, NoSQL data was read/written in MongoDB database, and it was automatically inserted into a local MySQL Database, and 10 enteries were inserted in this way. The script ensures that all constraints are satisfied before inserting the document in the NoSQL database.

---


# Task 2

## Implementation

We first specify the schema of the "Reader" collection with the following classes

```csharp
    public class SimpleDate
    {
        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public override string ToString()
        {
            return $"{Day}/{Month}/{Year}";
        }
    }
    public class Reader
    {
        public uint Reader_Id { get; set; }
        public ulong Accession_No { get; set; }
        public SimpleDate Issue_Date { get; set; }
        public SimpleDate Return_Date { get; set; }
    }
```

Here, `Reader_ID` is the primary key, and `Accession_No` is the foreign key from the `Books` collection. Since it is a 1:1 mapping, both `Reader_ID` and `Accession_No` should be unique in the table/collection. However, this collection needs to be implemented as column store database. Thus, it is stored in a Wide-Column Database structure as follows:

```csharp
    public class Reader_Accession_Column
    {
        public uint Reader_Id {get; set;}
        public ulong Accession_No { get; set; }
    }
    public class Reader_Issue_Column
    {
        public uint Reader_Id {get; set;}
        public SimpleDate Issue_Date { get; set; }
    }
    public class Reader_Return_Column
    {
        public uint Reader_Id {get; set;}
        public SimpleDate Return_Date { get; set; }
    }

```

Next, the C# library of Task-1 is added as a dependency to extend functionalities of the `MongoConnector` as shown:

```csharp
    public class MongoConnector2 : MongoConnector
    {
        protected readonly IMongoCollection<Reader> _readers;
        public MongoConnector2(string pass) : base(pass) { ...}
        public (bool BookDeleted, long DeletedReaders) Delete(ulong accession_No) {...}
        public (bool Updated, long UpdatedReaders) Update(ulong accession_No, Book updatedbook) {...}
        public bool Insert(Reader reader) {...}
        public bool DeleteReader(uint reader_id) {...}
        public bool UpdatedReaders(uint reader_id, Reader reader) {...}
        public IEnumerable<Reader> GetAllReaders() {...}
    }
```

The following additions are made in the `class MongoConnectorB` over the `class MongoConnector` class used in Task-1

- Support for additonal collection named "Readers"
- Modified constructor to initialize the `_readers` Collection
- **Support for `Delete(ulong accession_no)` which removes the entries in both the "Books" collection,  and also "Readers" collection to maintain referential integrity.** 
- **Support for `Update(ulong Accession_No, Book updatedBook)`, which finds the Book in the "Books" collection with the queried Accession_no to update it, and also updates all mentions of this book in the "Readers" collection as well, to maintain referential integrity.**
- Support for Insertion, Updation, Deletion, and Listing of Documents in the "Readers" collection.

These methods are provided in the fully documented file `TaskB/MongoConnectorB.cs` file. A brief working of these methods are as follows

- CRUD methods for collection `Books`
    - `Insert(Book book)`: Check if all keys (`ISBN` and `Accession_No`) are unique, and `Category` is only one of `{'C', 'Java'. 'Python', 'DBMS'}`. If all constraints are satisfied, then insert the document.
    - `Update(ulong accession_no, Book updatedBook)` : Update the book if it satisfies all constraints, and it exists, and check if the `Accession_No` is changed. If yes, then maintain referential integrity with `Readers` by updating all records having the old `Accession_No` by updating it with new value.
    - `Delete(ulong accession_no)`: Delete the document in `Book` with the given `Accession_No`. If document is deleted, also search and delete all records in `Readers` linked with the same `Accession_No` in order to maintain referential integrity.
    - `GetAllBooks()`: Returns an enumeration of all documents in `Books` collection.
- CRUD methods for collection `Readers`
    - `Insert(Reader reader)`: Check if key `Reader_ID` is unique and a document in `Books` exists with the given `Accession_No`, in order to maintain referential integrity. Since the mapping is 1:1, `Accession_No` must be Unique as well. If all constraints are satisfied, then insert the document in the `Readers` collection.
    - `Update(uint reader_id, Reader updatedReader)`: If a document with the given `Reader_Id` exists, and the updated instance is valid, then update the record. Also ensure that the document linked by the instance's foreign key also exists. Also ensure that the `Accession_no` would unique in the collection. If all constraints are satisfied, update the docuement.
    - `Delete(uint reader_id)`: Delete the document with the given `Reader_Id` value.
    - `GetAllReaders()`: Merge all columns of the `Readers` database and pass it as an enumeration of `Reader` objects.

## Execution

Executing the program can be done by calling `dotnet run` as shown below. For making a short demonstration, here the important methods are showcased which maintain referential integrity, i.e `Update(ulong accession_no, Book updatedBook)`, `Delete(ulong accession_no)` and `Update(uint reader_id, Reader updatedReader)`:

```

$ dotnet run
Now connecting to MongoDB database. Please ensure your internet connection is active.
Enter the password for MongoDB Atlas database for user "iitj_free": abcd
1. Insert a document in 'Books' collection
2. Update a document in 'Books' collection
3. Delete a document in 'Books' collection
4. See all documents in 'Books' collection
5. Insert a document in 'Readers' collection
6. Update a document in 'Readers' collection
7. Delete a document in 'Readers' collection
8. See all documents in 'Readers' collection
9. Exit

Enter your choice: 
4
[  a0001,  110320220001,   Programming in C,         Ashish,            IIT-J,  1, 2002,    20,   100,      C ]
[  a0002,  110320220002,   Programming in C,         Ashish,            IIT-J,  2, 2003,    25,   110,      C ]
[  a0003,  110320220003,   Programming in C,         Ashish,            IIT-J,  3, 2004,    30,   120,      C ]
[  b0001,  110320220004,   Java Programming,    Steve Bruce,          Pearson,  1, 2008,  1438,  2000,   Java ]
[  b0002,  110320220005,   Java Programming,    Steve Bruce,          Pearson,  2, 2010,  1574,  2100,   Java ]
[  c0002,  110320220007,    Learning Python,  Shirly Courts,            Wiley,  2, 2011,   411,   800, Python ]
[  d0001,  110320220008, DBMS in a nutshell,    Peter Irwin,    OReilly Media,  1, 2015,   210,   400,   DBMS ]
[  d0002,  110320220009, DBMS in a nutshell,    Peter Irwin,    OReilly Media,  2, 2016,   453,   700,   DBMS ]
[  d0003,  110320220010, DBMS in a nutshell,    Peter Irwin,    OReilly Media,  3, 2017,   513,   850,   DBMS ]
[  e0001,  120320220001,      Extrmeme Java,         Ashish,            IIT-J,  1, 2010,   120,   200,   Java ]
[  e0002,  120320220002,          Extreme C,         Ashish,            IIT-J,  1, 2010,   130,   250,      C ]
[  e0003,  120320220003,       Extreme DBMS,         Ashish,            IIT-J,  1, 2012,   140,   300,   DBMS ]

______________________________

1. Insert a document in 'Books' collection
2. Update a document in 'Books' collection
3. Delete a document in 'Books' collection
4. See all documents in 'Books' collection
5. Insert a document in 'Readers' collection
6. Update a document in 'Readers' collection
7. Delete a document in 'Readers' collection
8. See all documents in 'Readers' collection
9. Exit

Enter your choice: 
8
[   1, 110320220001, 15/3/2022, 17/3/2022 ]
[   2, 110320220002, 15/3/2022, 16/3/2022 ]
[   3, 110320220003, 15/3/2022, 18/3/2022 ]
[   4, 110320220008, 15/3/2022, 19/3/2022 ]
[   5, 120320220002, 15/3/2022, 21/3/2022 ]

______________________________

1. Insert a document in 'Books' collection
2. Update a document in 'Books' collection
3. Delete a document in 'Books' collection
4. See all documents in 'Books' collection
5. Insert a document in 'Readers' collection
6. Update a document in 'Readers' collection
7. Delete a document in 'Readers' collection
8. See all documents in 'Readers' collection
9. Exit

Enter your choice: 
2
Enter the accession_no of the book to update: 120320220002
Enter book to update the record with
Enter ISBN: f0001
Enter Accession number: 130320220001
Enter Title: Extreme C
Enter Author: Ashish
Enter Publisher: IIT-J
Enter Edition: 1
Enter Year: 2010
Enter Pages: 130
Enter Price: 250
Enter Category: (Should be any one of Java, Python, DBMS, C)  : C
Book is updated, and 1 readers document also affected

______________________________

1. Insert a document in 'Books' collection
2. Update a document in 'Books' collection
3. Delete a document in 'Books' collection
4. See all documents in 'Books' collection
5. Insert a document in 'Readers' collection
6. Update a document in 'Readers' collection
7. Delete a document in 'Readers' collection
8. See all documents in 'Readers' collection
9. Exit

Enter your choice: 
4
[  a0001,  110320220001,   Programming in C,         Ashish,            IIT-J,  1, 2002,    20,   100,      C ]
[  a0002,  110320220002,   Programming in C,         Ashish,            IIT-J,  2, 2003,    25,   110,      C ]
[  a0003,  110320220003,   Programming in C,         Ashish,            IIT-J,  3, 2004,    30,   120,      C ]
[  b0001,  110320220004,   Java Programming,    Steve Bruce,          Pearson,  1, 2008,  1438,  2000,   Java ]
[  b0002,  110320220005,   Java Programming,    Steve Bruce,          Pearson,  2, 2010,  1574,  2100,   Java ]
[  c0002,  110320220007,    Learning Python,  Shirly Courts,            Wiley,  2, 2011,   411,   800, Python ]
[  d0001,  110320220008, DBMS in a nutshell,    Peter Irwin,    OReilly Media,  1, 2015,   210,   400,   DBMS ]
[  d0002,  110320220009, DBMS in a nutshell,    Peter Irwin,    OReilly Media,  2, 2016,   453,   700,   DBMS ]
[  d0003,  110320220010, DBMS in a nutshell,    Peter Irwin,    OReilly Media,  3, 2017,   513,   850,   DBMS ]
[  e0001,  120320220001,      Extrmeme Java,         Ashish,            IIT-J,  1, 2010,   120,   200,   Java ]
[  e0003,  120320220003,       Extreme DBMS,         Ashish,            IIT-J,  1, 2012,   140,   300,   DBMS ]
[  f0001,  130320220001,          Extreme C,         Ashish,            IIT-J,  1, 2010,   130,   250,      C ]

______________________________

1. Insert a document in 'Books' collection
2. Update a document in 'Books' collection
3. Delete a document in 'Books' collection
4. See all documents in 'Books' collection
5. Insert a document in 'Readers' collection
6. Update a document in 'Readers' collection
7. Delete a document in 'Readers' collection
8. See all documents in 'Readers' collection
9. Exit

Enter your choice: 
8
[   1, 110320220001, 15/3/2022, 17/3/2022 ]
[   2, 110320220002, 15/3/2022, 16/3/2022 ]
[   3, 110320220003, 15/3/2022, 18/3/2022 ]
[   4, 110320220008, 15/3/2022, 19/3/2022 ]
[   5, 130320220001, 15/3/2022, 21/3/2022 ]

______________________________

1. Insert a document in 'Books' collection
2. Update a document in 'Books' collection
3. Delete a document in 'Books' collection
4. See all documents in 'Books' collection
5. Insert a document in 'Readers' collection
6. Update a document in 'Readers' collection
7. Delete a document in 'Readers' collection
8. See all documents in 'Readers' collection
9. Exit

Enter your choice: 
3
Enter the accession_no of the book to delete: 130320220001
Book is deleted, and 1 readers document also affected

______________________________

1. Insert a document in 'Books' collection
2. Update a document in 'Books' collection
3. Delete a document in 'Books' collection
4. See all documents in 'Books' collection
5. Insert a document in 'Readers' collection
6. Update a document in 'Readers' collection
7. Delete a document in 'Readers' collection
8. See all documents in 'Readers' collection
9. Exit

Enter your choice: 
4
[  a0001,  110320220001,   Programming in C,         Ashish,            IIT-J,  1, 2002,    20,   100,      C ]
[  a0002,  110320220002,   Programming in C,         Ashish,            IIT-J,  2, 2003,    25,   110,      C ]
[  a0003,  110320220003,   Programming in C,         Ashish,            IIT-J,  3, 2004,    30,   120,      C ]
[  b0001,  110320220004,   Java Programming,    Steve Bruce,          Pearson,  1, 2008,  1438,  2000,   Java ]
[  b0002,  110320220005,   Java Programming,    Steve Bruce,          Pearson,  2, 2010,  1574,  2100,   Java ]
[  c0002,  110320220007,    Learning Python,  Shirly Courts,            Wiley,  2, 2011,   411,   800, Python ]
[  d0001,  110320220008, DBMS in a nutshell,    Peter Irwin,    OReilly Media,  1, 2015,   210,   400,   DBMS ]
[  d0002,  110320220009, DBMS in a nutshell,    Peter Irwin,    OReilly Media,  2, 2016,   453,   700,   DBMS ]
[  d0003,  110320220010, DBMS in a nutshell,    Peter Irwin,    OReilly Media,  3, 2017,   513,   850,   DBMS ]
[  e0001,  120320220001,      Extrmeme Java,         Ashish,            IIT-J,  1, 2010,   120,   200,   Java ]
[  e0003,  120320220003,       Extreme DBMS,         Ashish,            IIT-J,  1, 2012,   140,   300,   DBMS ]

______________________________

1. Insert a document in 'Books' collection
2. Update a document in 'Books' collection
3. Delete a document in 'Books' collection
4. See all documents in 'Books' collection
5. Insert a document in 'Readers' collection
6. Update a document in 'Readers' collection
7. Delete a document in 'Readers' collection
8. See all documents in 'Readers' collection
9. Exit

Enter your choice: 
8
[   1, 110320220001, 15/3/2022, 17/3/2022 ]
[   2, 110320220002, 15/3/2022, 16/3/2022 ]
[   3, 110320220003, 15/3/2022, 18/3/2022 ]
[   4, 110320220008, 15/3/2022, 19/3/2022 ]

______________________________

1. Insert a document in 'Books' collection
2. Update a document in 'Books' collection
3. Delete a document in 'Books' collection
4. See all documents in 'Books' collection
5. Insert a document in 'Readers' collection
6. Update a document in 'Readers' collection
7. Delete a document in 'Readers' collection
8. See all documents in 'Readers' collection
9. Exit

Enter your choice: 
6
Enter the reader_id of the Reader to update: 4
Now taking input of reader structure: 
Enter Reader_ID: 4
Enter Accession_No: 120320220003
Issue date is set for today.
Enter days for return from today: 
9

Reader updated into the collection successfully!

______________________________

1. Insert a document in 'Books' collection
2. Update a document in 'Books' collection
3. Delete a document in 'Books' collection
4. See all documents in 'Books' collection
5. Insert a document in 'Readers' collection
6. Update a document in 'Readers' collection
7. Delete a document in 'Readers' collection
8. See all documents in 'Readers' collection
9. Exit

Enter your choice: 
4
[  a0001,  110320220001,   Programming in C,         Ashish,            IIT-J,  1, 2002,    20,   100,      C ]
[  a0002,  110320220002,   Programming in C,         Ashish,            IIT-J,  2, 2003,    25,   110,      C ]
[  a0003,  110320220003,   Programming in C,         Ashish,            IIT-J,  3, 2004,    30,   120,      C ]
[  b0001,  110320220004,   Java Programming,    Steve Bruce,          Pearson,  1, 2008,  1438,  2000,   Java ]
[  b0002,  110320220005,   Java Programming,    Steve Bruce,          Pearson,  2, 2010,  1574,  2100,   Java ]
[  c0002,  110320220007,    Learning Python,  Shirly Courts,            Wiley,  2, 2011,   411,   800, Python ]
[  d0001,  110320220008, DBMS in a nutshell,    Peter Irwin,    OReilly Media,  1, 2015,   210,   400,   DBMS ]
[  d0002,  110320220009, DBMS in a nutshell,    Peter Irwin,    OReilly Media,  2, 2016,   453,   700,   DBMS ]
[  d0003,  110320220010, DBMS in a nutshell,    Peter Irwin,    OReilly Media,  3, 2017,   513,   850,   DBMS ]
[  e0001,  120320220001,      Extrmeme Java,         Ashish,            IIT-J,  1, 2010,   120,   200,   Java ]
[  e0003,  120320220003,       Extreme DBMS,         Ashish,            IIT-J,  1, 2012,   140,   300,   DBMS ]

______________________________

1. Insert a document in 'Books' collection
2. Update a document in 'Books' collection
3. Delete a document in 'Books' collection
4. See all documents in 'Books' collection
5. Insert a document in 'Readers' collection
6. Update a document in 'Readers' collection
7. Delete a document in 'Readers' collection
8. See all documents in 'Readers' collection
9. Exit

Enter your choice: 
8
[   1, 110320220001, 15/3/2022, 17/3/2022 ]
[   2, 110320220002, 15/3/2022, 16/3/2022 ]
[   3, 110320220003, 15/3/2022, 18/3/2022 ]
[   4, 120320220003, 15/3/2022, 24/3/2022 ]

______________________________

1. Insert a document in 'Books' collection
2. Update a document in 'Books' collection
3. Delete a document in 'Books' collection
4. See all documents in 'Books' collection
5. Insert a document in 'Readers' collection
6. Update a document in 'Readers' collection
7. Delete a document in 'Readers' collection
8. See all documents in 'Readers' collection
9. Exit

Enter your choice: 
9

______________________________



```


## Conslusion

Thus, CRUD operations are defined on both the NoSQL collections, and all the constraints, including referential integrity, is maintianed between collections that are Document databases and Wide-Column databases as well.


# Task 3

For setting up the project, this [Setup guide](https://cloud.google.com/vision/docs/libraries?hl=en_US) from google developers was referred. The JSON key for the same is present in the project folder. This program requires the path of the key as a command line argument.

## Implementation

The project is a typical .NET Web Api. The file `Program.cs` launches the Web api and listens on the port. The Api contoller is defined in the file `Controller/TaskCController`, which contains a POST request for an image file and returns the structure containing all information regarding face and logo recognition.

The structure returned by the Api for a given image is as shown.

```csharp
public class ResultObject
{
    public FaceObject[] Faces { get; set; }
    public LogoObject[] Logos { get; set; }
}
```

The `ResultObject` contains a collection/array of Faces and Logos, which are represented by the following custom structures


```csharp
public class FaceObject
{
    public float Confidence { get; set; }
    public string JoyLiklihood { get; set; }
    public string SadLiklihood { get; set; }
    public string AngryLiklihood { get; set; }
}
public class LogoObject
{
    public float Confidence { get; set; }
    public string Description { get; set; }
}
```

These features are obtained with [Google's Vision API for .NET](https://cloud.google.com/dotnet/docs/reference/Google.Cloud.Vision.V1/latest/index)

The client requires creadentials of the user which is included in json format in the file `TaskC/Keys/assignment2-342913-e2d3ba3b206e.json`. This file will be required to be inputted using command-line arguemnt for this program.


## Execution

Along with the `dotnet run` command, the key also needs to be passed via command line arguments. In the `TaskC/` folder, use

```
$ dotnet run ./Keys/assignment2-342913-e2d3ba3b206e.json 
```

to launch the program.

This will initiate the Api at a localhost at a random port. This address will be mentioned in the output of the program

```

$ dotnet run ./Keys/assignment2-342913-e2d3ba3b206e.json 

..
..
..
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:5001
..
..
..
```

So in this case, the requests can be sent at https://localhost:5001/TaskC

Using postman, the images were added as binary file in the Body of the Post request. Their results are as follows:


![](./s1.png)


- img_01.png

```json
{
    "faces": [],
    "logos": [
        {
            "confidence": 0.08373566,
            "description": "Amazon"
        }
    ]
}
```

- img_02.jpg
```json
{
    "faces": [
        {
            "confidence": 0.9451922,
            "joyLiklihood": "VeryLikely",
            "sadLiklihood": "VeryUnlikely",
            "angryLiklihood": "VeryUnlikely"
        },
        {
            "confidence": 0.5749183,
            "joyLiklihood": "Unlikely",
            "sadLiklihood": "VeryUnlikely",
            "angryLiklihood": "VeryUnlikely"
        },
        {
            "confidence": 0.7792016,
            "joyLiklihood": "VeryUnlikely",
            "sadLiklihood": "VeryUnlikely",
            "angryLiklihood": "VeryUnlikely"
        },
        {
            "confidence": 0.89755946,
            "joyLiklihood": "VeryUnlikely",
            "sadLiklihood": "VeryUnlikely",
            "angryLiklihood": "VeryUnlikely"
        },
        {
            "confidence": 0.93615705,
            "joyLiklihood": "VeryUnlikely",
            "sadLiklihood": "VeryUnlikely",
            "angryLiklihood": "VeryUnlikely"
        },
        {
            "confidence": 0.69231963,
            "joyLiklihood": "VeryUnlikely",
            "sadLiklihood": "VeryUnlikely",
            "angryLiklihood": "VeryUnlikely"
        }
    ],
    "logos": []
}
```

- img_03.jpg
```json
{
    "faces": [
        {
            "confidence": 0.5008719,
            "joyLiklihood": "VeryLikely",
            "sadLiklihood": "VeryUnlikely",
            "angryLiklihood": "VeryUnlikely"
        },
        {
            "confidence": 0.95083404,
            "joyLiklihood": "Likely",
            "sadLiklihood": "VeryUnlikely",
            "angryLiklihood": "VeryUnlikely"
        }
    ],
    "logos": [
        {
            "confidence": 0.05604562,
            "description": "Kid Cuisine"
        }
    ]
}
```


## Conclusion

Thus, an API was created which could take images as input as Content in the POST request, and then details regarding face/logos are returned.
