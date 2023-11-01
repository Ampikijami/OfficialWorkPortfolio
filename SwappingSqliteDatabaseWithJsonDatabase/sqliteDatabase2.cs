using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.Data.Sqlite;

namespace SwappingSqliteDatabaseWithJsonDatabase.SqliteDatabase2
{
    class SqliteDatabase2 : IDatabase
    {
        private const string dbName = "sqliteproductiondb.db";
        private SqliteConnection sqliteConnection;
        private const string connectionString = "FileName={0}";
        private const string databaseFileName = "sqliteproductiondb2.db";
        private readonly bool databaseContinuity = false; //overwrite database data stored in previous session or continue with last session?

        private readonly Dictionary<string, string> CToSQLTypeMap = new Dictionary<string, string>()
        {
            {"System.Int32", "int" },
            {"System.String", "varchar(255)" }
        };
        public void Connect()
        {
            if(databaseContinuity == false)
            {
                if (File.Exists(databaseFileName))
                    File.Delete(databaseFileName);
            }
            sqliteConnection = new SqliteConnection(String.Format(connectionString, databaseFileName));
            sqliteConnection.Open(); //Creates the database if it doesn't exist.
            sqliteConnection.Close(); //Closes the connection to the database. Open transactions are rolled back
        }
        public void CreateTable(Type tableDataType, object schema)
        {
            sqliteConnection.Open();
            SqliteCommand sqliteCommand = sqliteConnection.CreateCommand();
            string strType = tableDataType.Name;
            PropertyInfo[] properties = tableDataType.GetProperties();

            string tableFields = "(";
            foreach(PropertyInfo info in properties)
            {
                tableFields += info.Name + " " + CToSQLTypeMap[info.PropertyType.FullName] + ", ";
            }
            tableFields = tableFields.Trim(',', ' ');
            tableFields += ")";
            string sql = "CREATE TABLE IF NOT EXISTS " + tableDataType.Name + " " + tableFields;
            sqliteCommand.CommandText = sql;
            sqliteCommand.ExecuteNonQuery();
            sqliteConnection.Close();
        }
        public void AddToTable(Type tableDataType, object objectInstance)
        {
            sqliteConnection.Open();
            SqliteCommand sqliteCommand = sqliteConnection.CreateCommand();
            string strType = tableDataType.Name;
            PropertyInfo[] properties = tableDataType.GetProperties();
            string tableFields = "(";
            foreach (PropertyInfo info in properties)
            {
                tableFields += info.Name + ", ";
            }
            tableFields = tableFields.Trim(',', ' ');
            tableFields += ")";
            string sql = "INSERT INTO " + tableDataType.Name + " " + tableFields;

            string tableValues = "VALUES (";
            foreach(PropertyInfo info in properties)
            {
                if(info.PropertyType.FullName == "System.String")
                    tableValues += "\'";
                tableValues += info.GetValue(objectInstance).ToString() + (info.PropertyType.FullName == "System.String" ? "\'" : "") +  ", ";
            }
            tableValues = tableValues.Trim(',', ' ');
            tableValues += ")";
            sql += " " + tableValues;
            sqliteCommand.CommandText = sql;
            sqliteCommand.ExecuteNonQuery();
            sqliteConnection.Close();
        }
        public void PrintContentsOfTable(Type tableDataType)
        {
            sqliteConnection.Open();
            var command = sqliteConnection.CreateCommand();
            command.CommandText =
                    $@"
                SELECT *
                FROM {tableDataType.Name}
            ";

            using (var reader = command.ExecuteReader())
            {
                object[] values = new object[reader.FieldCount];
                Console.WriteLine();
                Console.WriteLine($"Getting database data for table {tableDataType.Name}");
                while (reader.Read())
                {
                    int columnCount = reader.GetValues(values);
                    for(int i = 0; i < columnCount; i++)
                    {
                        Console.Write($"{values[i]}\t|");
                    }
                    System.Console.WriteLine();
                }
            }
            sqliteConnection.Close();
        }
        
        public bool Save()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            System.Console.WriteLine("Database viewable @\n" + Directory.GetCurrentDirectory() + "\\" + databaseFileName);
            Console.ForegroundColor = ConsoleColor.Gray;
            return true; //Do nothing... for now. This is useful for open transactions only. Which is not implemented yet.
        }
    }
  
}
