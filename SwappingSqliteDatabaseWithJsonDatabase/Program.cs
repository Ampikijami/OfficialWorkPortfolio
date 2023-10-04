using System;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace SwappingSqliteDatabaseWithJsonDatabase
{
    /*
     Program summary: This program demonstrates how to program against an interface,
    which makes swapping between a .json type database and a sqlite type database effortless.
     */
    class Program
    {
        //In order to allow child methods of the Main() thread to run asynchronously, I made static Main(string[] args) into >> static async Task Main(string[] args)
        static async Task Main(string[] args)
        {
            Assembly DLL = Assembly.LoadFile(Path.Combine(Directory.GetCurrentDirectory() + "\\" + "SwappingSqliteDatabaseWithJsonDatabase.dll"));
            Type sqliteDatabase = DLL.GetType("SwappingSqliteDatabaseWithJsonDatabase.SqliteDatabase"); //this would be helpful if the type that we wanted to use was defined in another assembly.
            Type jsonDatabase = DLL.GetType("SwappingSqliteDatabaseWithJsonDatabase.JsonDatabase"); //this would be helpful if the type we wanted to use was defined in another assembly.

            object database;
            char userInput;
            do
            {
                System.Console.WriteLine();
                System.Console.WriteLine("Choose a database type to run...");
                System.Console.WriteLine("1. SQLiteDatabase");
                System.Console.WriteLine("2. JsonDatabase");
                System.Console.WriteLine("3. JsonDatabase version 2 (Coming soon!)");
                System.Console.WriteLine();
                userInput = System.Console.ReadKey().KeyChar;

            } while (userInput != '1' && userInput != '2'); // && userInput != '3'
            switch (userInput)
            {
                case '1':
                    database = database = Activator.CreateInstance(typeof(SwappingSqliteDatabaseWithJsonDatabase.SqliteDatabase2.SqliteDatabase2));
                    break;
                case '2':
                    database = Activator.CreateInstance(typeof(JsonDatabase));
                    break;
                case '3':
                    database = Activator.CreateInstance(typeof(SwappingSqliteDatabaseWithJsonDatabase.SqliteDatabase.SqliteDatabase));
                    break;
                default:
                    throw new Exception("This should be impossible!");
            };
            MethodInfo ConnectMethod = database.GetType().GetMethod("Connect");
            MethodInfo CreateTableMethod = database.GetType().GetMethod("CreateTable");
            MethodInfo AddToTableMethod = database.GetType().GetMethod("AddToTable");
            MethodInfo PrintContentsOfTableMethod = database.GetType().GetMethod("PrintContentsOfTable");
            MethodInfo SaveMethod = database.GetType().GetMethod("Save");

            ConnectMethod.Invoke(database, null);
            CreateTableMethod.Invoke(database, new object[] { typeof(Movie), null });//create a Movie table
            AddToTableMethod.Invoke(database, new object[] { typeof(Movie), new Movie(1, "Men in Black", 2000) });
            AddToTableMethod.Invoke(database, new object[] { typeof(Movie), new Movie(2, "Shrek", 2005) });
            PrintContentsOfTableMethod.Invoke(database, new object[] { typeof(Movie)});
            SaveMethod.Invoke(database, null);
        }
    }
}
