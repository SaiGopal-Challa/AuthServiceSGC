using AuthServiceSGC.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace AuthServiceSGC.Infrastructure.Database
{
    public class OracleDbContext : DbContext
    {
        public OracleDbContext(DbContextOptions<OracleDbContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; } // Assuming the User entity exists

        /*protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("UserTable"); // Ensure this is correct
            base.OnModelCreating(modelBuilder);
        }*/
    }
}
