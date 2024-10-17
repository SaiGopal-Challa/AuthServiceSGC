using AuthServiceSGC.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthServiceSGC.Infrastructure.Database
{
    public class PostgreSqlDbContext : DbContext
    {
        public PostgreSqlDbContext(DbContextOptions<PostgreSqlDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuring entity to use a specific table name if needed
            modelBuilder.Entity<User>().ToTable("UserTable");
            base.OnModelCreating(modelBuilder);
        }
    }
}
