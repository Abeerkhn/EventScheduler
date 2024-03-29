﻿using Microsoft.EntityFrameworkCore;

namespace EventScheduler
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options) : base(options)

        {

        }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                 .IsUnique();
            // Configure the ID as a database-generated identity

          base.OnModelCreating(modelBuilder);


        }
    }
}
