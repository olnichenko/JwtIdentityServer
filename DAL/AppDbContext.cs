using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class AppDbContext : DbContext
    {
        private string _connectionString;

        public DbSet<User> Users => Set<User>();

        public AppDbContext(string connectionString)
        {
            _connectionString = connectionString;
            Database.EnsureCreated();
            if (Database.GetPendingMigrations().Any())
            {
                Database.Migrate();
            }
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(_connectionString);
        }
    }
}
