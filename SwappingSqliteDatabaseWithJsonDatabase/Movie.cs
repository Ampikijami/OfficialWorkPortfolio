using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwappingSqliteDatabaseWithJsonDatabase
{
    public class Movie
    {
        public int id { get; set; }
        public string title { get; set; }
        public int releaseYear { get; set; }
        public Movie(int id, string title, int releaseYear)
        {
            this.id = id;
            this.title = title;
            this.releaseYear = releaseYear;
        }
        public Movie()
        { }
    }
}
