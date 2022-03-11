using System;
namespace TaskB
{
    public class Reader
    {
        public uint Reader_Id { get; set; }
        public ulong Accession_No { get; set; }
        public DateOnly Issue_Date { get; set; }
        public DateOnly Return_Date { get; set; }
    }
}