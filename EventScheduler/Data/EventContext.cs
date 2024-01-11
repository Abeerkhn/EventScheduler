using EventScheduler.Models;
using Microsoft.EntityFrameworkCore;

public class EventContext : DbContext
{
    public EventContext(DbContextOptions<EventContext> options) : base(options)
    {
    }

    public DbSet<Event> Events { get; set; }
    public DbSet<RecurrencePattern> RecurrencePatterns { get; set; }
   

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Event>()
            .HasOne(e => e.RecurrencePattern)
            .WithOne(rp => rp.Event)
            .HasForeignKey<RecurrencePattern>(rp => rp.EventId)
            .IsRequired(false);


        // Other configurations...

          base.OnModelCreating(modelBuilder);
    }

}
