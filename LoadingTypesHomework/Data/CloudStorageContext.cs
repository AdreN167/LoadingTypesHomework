using LoadingTypesHomework.Models;
using Microsoft.EntityFrameworkCore;

namespace LoadingTypesHomework.Data
{
    public class CloudStorageContext : DbContext
    {
        public CloudStorageContext()
        {
            Database.EnsureCreated();
        }

        public DbSet<Directory> Directories { get; set; }
        public DbSet<File> Files { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies()
                .UseSqlServer("Server=WW\\MSSQLSERVER2017; Database=CloudStorageDb; Trusted_Connection=True;");
        }
    }
}
