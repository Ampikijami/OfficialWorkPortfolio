using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace SwappingSqliteDatabaseWithJsonDatabase
{
    class JsonDatabase : IDatabase
    {
        private readonly bool _databaseContinuity = false; //overwrite database data stored in previous session or continue with last session?
        private readonly string _databasePath = @".\jsonDatabase.json";
        private JsonDatabaseModel jsonDatabaseModel { get; set; }
        public Newtonsoft.Json.Linq.JObject DatabaseJObject { 
            get 
            {
                if (jsonDatabaseModel == null)
                {
                    throw new Exception("Invoke the Connect method of the JsonDatabase class first. Cannot get the JObject until an instance of the JsonDatabaseModel is created/re-opened");
                }
                else
                {
                    return jsonDatabaseModel.jobject;
                }
            }
            set
            {
                jsonDatabaseModel.jobject = value;
            }
        }

        public void Connect()
        {
            jsonDatabaseModel = new JsonDatabaseModel(_databasePath, _databaseContinuity);
        }
        public void CreateTable(Type tableDataType, object schema)
        {
            if(DatabaseJObject[tableDataType.Name] == null)
                DatabaseJObject[tableDataType.Name] = new JArray();
            //else table already exists so do nothing
        }
        public void AddToTable(Type tableDataType, object objectInstance)
        {
            ((JArray)DatabaseJObject[tableDataType.Name]).Add(JToken.FromObject(objectInstance));
        }
        public void PrintContentsOfTable(Type tableDataType)
        {
            JToken tableAsJToken = DatabaseJObject.SelectToken(tableDataType.Name);
            foreach(JToken child in tableAsJToken.Children())
            {
                System.Console.WriteLine(child.ToString());
            }
        }

        public bool Save()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            System.Console.WriteLine("Database viewable @\n" + Directory.GetCurrentDirectory() + "\\" + _databasePath.Trim(new char[] {'\\', '.' }));
            Console.ForegroundColor = ConsoleColor.Gray;
            return jsonDatabaseModel.Save();
        }
    }
}
