using Microsoft.EntityFrameworkCore;
using NOVA.Core.Models;

namespace NOVA.Infrastructure.Data
{
    public class NovaDbContext : DbContext
    {
        public NovaDbContext(DbContextOptions<NovaDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<ChatSession> ChatSessions { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User → Sessions
            modelBuilder.Entity<User>()
                .HasMany(u => u.Sessions)
                .WithOne()
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Session → Messages
            modelBuilder.Entity<ChatSession>()
                .HasMany(s => s.Messages)
                .WithOne()
                .HasForeignKey(m => m.SessionId)
                .OnDelete(DeleteBehavior.Cascade);

            // ChatMessage settings
            modelBuilder.Entity<ChatMessage>()
                .Property(m => m.Content)
                .HasMaxLength(2000);
        }
    }
}
