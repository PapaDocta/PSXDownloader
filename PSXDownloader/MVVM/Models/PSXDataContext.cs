using Microsoft.EntityFrameworkCore;
using System.IO;

namespace PSXDownloader.MVVM.Models
{
    public class PSXDataContext : DbContext
    {
        public DbSet<PSXDatabase>? PSXDatabases { get; set; }

        public PSXDataContext()
        {
            Database.Migrate();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            if (!Directory.Exists("Database"))
            {
                Directory.CreateDirectory("Database");
            }
            string dbPath = Path.Combine("Database", "PSXDatabase.db");
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }
    }
}
