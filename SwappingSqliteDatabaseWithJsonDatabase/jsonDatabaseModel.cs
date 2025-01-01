using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace SwappingSqliteDatabaseWithJsonDatabase
{
    public class JsonDatabaseModel
    {
        readonly private bool _databaseContinuity;
        readonly private string _databasePath;
        public Newtonsoft.Json.Linq.JObject jobject { get; set; }
        public JsonDatabaseModel(string databasePath = @".\jsonDatabase.json", bool databaseContinuity = false)
        {
            _databasePath = databasePath;
            _databaseContinuity = databaseContinuity;
            if (File.Exists(_databasePath))
            {
                if (_databaseContinuity == false)
                {
                    File.Delete(_databasePath);
                    File.Create(_databasePath).Close();
                    jobject = new Newtonsoft.Json.Linq.JObject();
                }
                else
                {
                    jobject = (Newtonsoft.Json.Linq.JObject)JsonConvert.DeserializeObject(File.ReadAllText(_databasePath));
                }
            }
            else
            {
                File.Create(_databasePath).Close();
                jobject = new Newtonsoft.Json.Linq.JObject(); //other constructor takes an object
            }
            Save();
        }
            
        public bool Save()
        {
            try
            {
                string serializedJson = JsonConvert.SerializeObject(jobject, Formatting.Indented);
                File.WriteAllText(_databasePath, serializedJson);
                return true; //success
            }
            catch(Exception e)
            {
                System.Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine("Failed to save database." + e.Message);
                System.Console.ForegroundColor = ConsoleColor.Gray;
                return false; //fail
            }
        }
    }
}
