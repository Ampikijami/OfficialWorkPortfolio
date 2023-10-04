using System;

namespace SwappingSqliteDatabaseWithJsonDatabase
{
    public interface IDatabase
    {
        public void Connect();
        public void CreateTable(Type tableDataType, object schema);
        public void AddToTable(Type tableDataType, object objectInstance);
        public void PrintContentsOfTable(Type tableDataType);
        public bool Save();
    }
}