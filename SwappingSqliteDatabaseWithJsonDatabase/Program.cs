using System;
using System.Reflection;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;

namespace SwappingSqliteDatabaseWithJsonDatabase
{
    /*
     Program summary: This program demonstrates how to program against an interface,
    which makes swapping between a .json type database and a sqlite type database effortless.
     */
    class Program
    {
        //In order to allow child methods of the Main() thread to run asynchronously, I made static Main(string[] args) into >> static async Task Main(string[] args)
        private static IConfigurationRoot configuration;
        static async Task Main(string[] args)
        {
            Assembly DLL = Assembly.LoadFile(Path.Combine(Directory.GetCurrentDirectory() + "\\" + "SwappingSqliteDatabaseWithJsonDatabase.dll"));//this would be helpful if the type that we wanted to use was defined in another assembly in another solution folder
            Type sqliteDatabase = DLL.GetType("SwappingSqliteDatabaseWithJsonDatabase.SqliteDatabase"); //this would be helpful if the type that we wanted to use was defined in another assembly in another solution folder
            Type jsonDatabase = DLL.GetType("SwappingSqliteDatabaseWithJsonDatabase.JsonDatabase"); //this would be helpful if the type we wanted to use was defined in another assembly in another solution folder

            object database;
            IDatabase database2AlternativeDemonstration = null;
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
            configuration = GetConfiguration();
            switch (userInput)
            {
                case '1':
                    database = (IDatabase)Activator.CreateInstance(Type.GetType(configuration["SqliteDatabase2"], throwOnError: true));
                    database2AlternativeDemonstration = (IDatabase)Activator.CreateInstance(Type.GetType(configuration["SqliteDatabase2"], throwOnError: true));
                    break;
                case '2':
                    database = (IDatabase)Activator.CreateInstance(Type.GetType(configuration["JsonDatabase"], throwOnError: true));
                    database2AlternativeDemonstration = (IDatabase)Activator.CreateInstance(Type.GetType(configuration["JsonDatabase"], throwOnError: true));
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

            Console.ForegroundColor = ConsoleColor.White;
            System.Console.WriteLine("\n\n\nThe following is a demonstration of an alternative to using MethodInfo to invoke methods against objects, and thus demonstrates the advantage of programming against an interface [IDatabase].\n");
            database2AlternativeDemonstration.Connect();
            database2AlternativeDemonstration.CreateTable(typeof(Movie), null);
            database2AlternativeDemonstration.AddToTable(typeof(Movie), new Movie(3, "Alternative demonstration", 2222));
            database2AlternativeDemonstration.AddToTable(typeof(Movie), new Movie(4, "Anger Management", 2011));
            database2AlternativeDemonstration.PrintContentsOfTable(typeof(Movie));
            database2AlternativeDemonstration.Save();
        }
        private static IConfigurationRoot GetConfiguration()
        {
            //Hello stranger! Cześć nieznajomy!
            //need to install nuget package Microsoft.Extensions.Configuration 7.0.0
            //need to install nuget package Microsoft.Extensions.Configuration.FileExtensions
            //need to install nuget package Microsoft.Extensions.Configuration.Json

            string currentDirectory = Directory.GetCurrentDirectory();
            Regex exPathToProjectCS = new Regex($@".*" + Assembly.GetExecutingAssembly().GetName().Name);
            string strPathToProjectCS = exPathToProjectCS.Match(currentDirectory).Value;
            configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(strPathToProjectCS + @"\appsettings.json")
                .Build();

            return configuration;
        }
    }
}
