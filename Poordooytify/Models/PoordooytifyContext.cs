using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Poordooytify.Models
{
    public class PoordooytifyContext : DbContext
    {
        public PoordooytifyContext() : base("DbConnSqlAzure")
        {
        }

        public DbSet<Song> Songs { get; set; }
        public DbSet<CloudToken> CloudTokens { get; set; }
        public DbSet<Genre> Genres { get; set; }
    }

    //public class AccessPoordooytifyContext : DbContext
    //{
    //    public AccessPoordooytifyContext() : base("DbConnJetAccess")
    //    {
    //    }

    //    public DbSet<Song> Songs { get; set; }
    //    public DbSet<CloudToken> CloudTokens { get; set; }
    //    public DbSet<Genre> Genres { get; set; }
    //}
}