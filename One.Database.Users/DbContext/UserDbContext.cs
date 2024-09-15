using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using One.Database.Users.Models;

namespace One.Database.Users.DbContext
{
    public class UserDbContext(DbContextOptions<UserDbContext> options)
        : Microsoft.EntityFrameworkCore.DbContext(options)
    {
        public DbSet<User > Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Define unique index for Email
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}
