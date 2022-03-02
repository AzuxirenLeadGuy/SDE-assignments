using System;
using TaskAB;
using TaskC;
using System.Drawing;
using Google.Cloud.Vision.V1;

namespace Program
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            if(args.Length==0)
            {
                Console.WriteLine("Need additional arguments for the program!");
            }
            else
            {
                if(args[0].ToUpper()=="C" && args.Length>= 2)
                {
                    string path = args[1];
                    Image img1 = Image.FromFile(path);
                }
                else
                {
                    Console.WriteLine("Undefined arguments!");
                }
            }
            Console.WriteLine("\nThis program features the following modes\nB \t\t\t: Task B\nC <image_path>\t\t: Task C");
        }
    }
}