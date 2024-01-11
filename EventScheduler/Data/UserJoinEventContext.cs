using EventScheduler.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace EventScheduler.Data
{
    public class UserJoinEventContext : DbContext
    {
        public UserJoinEventContext(DbContextOptions<UserJoinEventContext> options) : base(options)
        {
        }

        public DbSet<UserJoinEvent> UserEventJoins { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("dbo");

            modelBuilder.Entity<UserJoinEvent>()
                .HasKey(ue => new { ue.UserId, ue.EventId });

            modelBuilder.Entity<UserJoinEvent>()
                .HasOne(ue => ue.User)
                .WithMany()
                .HasForeignKey(ue => ue.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserJoinEvent>()
                .HasOne(ue => ue.Event)
                .WithMany()
                .HasForeignKey(ue => ue.EventId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Ignore<User>();
            modelBuilder.Ignore<Event>();
            // Other configurations...

            base.OnModelCreating(modelBuilder);
        }
    }
}
