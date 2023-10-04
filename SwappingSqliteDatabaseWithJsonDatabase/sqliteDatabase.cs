using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Threading;
using SwappingSqliteDatabaseWithJsonDatabase.SqliteDatabase;
using Microsoft.Data.Sqlite;

namespace SwappingSqliteDatabaseWithJsonDatabase.SqliteDatabase
{
    class SqliteDatabase : IDatabase
    {
        private const string dbName = "sqliteproductiondb.db";
        private sqlitedbContext dbContext;
        
        public async void Connect()
        {
            System.Console.WriteLine("Connecting...");
            if (File.Exists(dbName))
                File.Delete(dbName);

            dbContext = new sqlitedbContext();
            await dbContext.Database.EnsureCreatedAsync();
        }
        public void CreateTable(Type tableDataType, object schema)
        {
            //attach/ entry
            //dbContext.Entry()
           // dbContext.Attach(schema);
            //dbContext.SaveChanges();
        }
        public async void AddToTable(Type tableDataType, object objectInstance)
        {
            try
            {
                dbContext.Entry(
                    new Movie(){id = 1, title = "Men In Black", releaseYear = 2001}
                );
                //dbContext.Attach(new Movie[]
                //{
                //    new Movie(){id = 1, title = "Men In Black", releaseYear = 2001},
                //    new Movie(){id = 2,  title = "Shrek", releaseYear = 2005 }
                //});
                //await dbContext.Movies.AddRangeAsync(entities: new Movie[]
                //    {
                //        new Movie(){id = 1, title = "Men In Black", releaseYear = 2001},
                //        new Movie(){id = 2,  title = "Shrek", releaseYear = 2005 }
                //    });
                await dbContext.SaveChangesAsync();
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException e)
            {
                System.Console.WriteLine(e.Message);
                System.Console.WriteLine(e.InnerException);
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
            }

        }
        public void PrintContentsOfTable(Type tableDataType)
        {
            Console.WriteLine("getting database data");
            dbContext.Movies?.ToList().ForEach(movie =>
            {
                System.Console.WriteLine($"{movie.title} was released in {movie.releaseYear}");
            });

            System.Console.ReadLine();
        }
        public bool Save()
        {
            try
            {
                dbContext.SaveChangesAsync();
                return true; //true = success
            }
            catch(Exception e)
            {
                System.Console.WriteLine(e.Message);
                return false; //false = fail
            }
        }
    }
    internal class sqlitedbContext : DbContext
    {
        public DbSet<Movie> Movies { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("FileName=sqliteproductiondb.db", sqliteOptionsAction => {
                sqliteOptionsAction.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);

            });
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Movie>().ToTable("Movies", schema: "test");
            modelBuilder.Entity<Movie>(entity =>
            {
                entity.HasKey(movie => movie.id);
                entity.HasIndex(index => index.title).IsUnique();
            });
            base.OnModelCreating(modelBuilder);
        }

    }
}
